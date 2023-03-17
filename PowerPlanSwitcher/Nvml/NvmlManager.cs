using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PowerPlanSwitcher.Nvml
{
    internal enum NvmlReturn
    {
        NVML_SUCCESS = 0,
        NVML_ERROR_UNINITIALIZED,
        NVML_ERROR_INVALID_ARGUMENT,
        NVML_ERROR_NOT_SUPPORTED,
        NVML_ERROR_NO_PERMISSION,
        NVML_ERROR_ALREADY_INITIALIZED,
        NVML_ERROR_NOT_FOUND,
        NVML_ERROR_INSUFFICIENT_SIZE,
        NVML_ERROR_INSUFFICIENT_POWER,
        NVML_ERROR_DRIVER_NOT_LOADED,
        NVML_ERROR_TIMEOUT,
        NVML_ERROR_IRQ_ISSUE,
        NVML_ERROR_LIBRARY_NOT_FOUND,
        NVML_ERROR_FUNCTION_NOT_FOUND,
        NVML_ERROR_CORRUPTED_INFOROM,
        NVML_ERROR_GPU_IS_LOST,
        NVML_ERROR_RESET_REQUIRED,
        NVML_ERROR_OPERATING_SYSTEM,
        NVML_ERROR_LIB_RM_VERSION_MISMATCH,
        NVML_ERROR_IN_USE,
        NVML_ERROR_MEMORY,
        NVML_ERROR_NO_DATA,
        NVML_ERROR_VGPU_ECC_NOT_SUPPORTED,
        NVML_ERROR_INSUFFICIENT_RESOURCES,
        NVML_ERROR_UNKNOWN = 999
    }

    internal static class NvmlReturnExtensions
    {
        internal static bool IsSuccess(this NvmlReturn ret)
        {
            return ret == NvmlReturn.NVML_SUCCESS;
        }
    }

    internal class NvmlManager : IGpuManager, IDisposable
    {
        [DllImport("nvml.dll", EntryPoint = "nvmlInit_v2")]
        private static extern NvmlReturn NvmlInitV2();
        [DllImport("nvml.dll", EntryPoint = "nvmlShutdown")]
        private static extern NvmlReturn NvmlShutdown();
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetCount_v2")]
        private static extern NvmlReturn NvmlDeviceGetCount_v2(out uint deviceCount);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetHandleByIndex")]
        private static extern NvmlReturn NvmlDeviceGetHandleByIndex(uint index, out IntPtr device);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetName")]
        private static extern NvmlReturn NvmlDeviceGetName(IntPtr device, IntPtr name, uint length);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetPowerManagementLimitConstraints")]
        private static extern NvmlReturn NvmlDeviceGetPowerManagementLimitConstraints(IntPtr device, out uint minLimit, out uint maxLimit);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetPowerManagementDefaultLimit")]
        private static extern NvmlReturn NvmlDeviceGetPowerManagementDefaultLimit(IntPtr device, out uint defaultLimit);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetPowerManagementLimit")]
        private static extern NvmlReturn NvmlDeviceGetPowerManagementLimit(IntPtr device, out uint limit);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceSetPowerManagementLimit")]
        private static extern NvmlReturn NvmlDeviceSetPowerManagementLimit(IntPtr device, uint limit /* mW */);

        private IntPtr _deviceHandle;
        private string _deviceName;
        private uint _defaultPowerLimit;
        private uint _minPowerLimit;
        private uint _maxPowerLimit;

        private bool _initialized = false;
        private readonly NvmlConfig _config;

        internal NvmlManager(JObject configJson)
        {
            _config = configJson.ToObject<NvmlConfig>();
            
            if (_config == null)
            {
                throw new ArgumentException($"Couldn't convert JObject to {nameof(NvmlConfig)}. JSON: {configJson.ToString(Formatting.None)}");
            }
        }

        public bool Initialize(out string errorMessage)
        {
            if (!NvmlInitV2().IsSuccess())
            {
                errorMessage = "Couldn't initialize NVML API. GPU control will not be available.";
                return false;
            }

            if (!NvmlDeviceGetHandleByIndex(_config.DeviceIndex, out _deviceHandle).IsSuccess())
            {
                errorMessage = $"Couldn't get handle to NVML device with index {_config.DeviceIndex}. GPU control will not be available.";
                return false;
            }

            int maxNameLength = 96; // Per the NVML docs
            IntPtr ptrName = Marshal.AllocHGlobal(maxNameLength);
            if (NvmlDeviceGetName(_deviceHandle, ptrName, (uint)maxNameLength).IsSuccess())
            {
                _deviceName = Marshal.PtrToStringAnsi(ptrName);
                Marshal.FreeHGlobal(ptrName);
            }
            else
            {
                errorMessage = $"Couldn't get device name for NVML device with index {_config.DeviceIndex}. GPU control will not be available.";
                return false;
            }

            if (!NvmlDeviceGetPowerManagementDefaultLimit(_deviceHandle, out _defaultPowerLimit).IsSuccess())
            {
                errorMessage = $"Couldn't get default power limit for NVML device with index {_config.DeviceIndex}. GPU control will not be available.";
                return false;
            }

            if (!NvmlDeviceGetPowerManagementLimitConstraints(_deviceHandle, out _minPowerLimit, out _maxPowerLimit).IsSuccess())
            {
                errorMessage = $"Couldn't get min/max power limits for NVML device with index {_config.DeviceIndex}. GPU control will not be available.";
                return false;
            }

            _initialized = true;

            errorMessage = String.Empty;
            return true;
        }

        public string GetDeviceName()
        {
            EnsureInitialized();

            return _deviceName;
        }

        public double GetPowerLimit()
        {
            EnsureInitialized();

            if (NvmlDeviceGetPowerManagementLimit(_deviceHandle, out uint limit).IsSuccess())
            {
                return limit;
            }

            return _defaultPowerLimit;
        }

        public int GetActivePowerLimitIndex()
        {
            EnsureInitialized();

            if (NvmlDeviceGetPowerManagementLimit(_deviceHandle, out uint limit).IsSuccess())
            {
                double activeLimitScaling = (double)limit / _defaultPowerLimit;
                return Array.IndexOf(_config.AvailablePowerScaling, activeLimitScaling);
            }

            return -1;
        }

        public double[] GetAvailablePowerLimits()
        {
            EnsureInitialized();

            return _config.AvailablePowerScaling;
        }

        public bool SetPowerLimit(int powerLimitIndex)
        {
            EnsureInitialized();

            double scaling = _config.AvailablePowerScaling[powerLimitIndex];
            uint newLimit = (uint)((_defaultPowerLimit / 1000) * scaling) * 1000;

            newLimit = Math.Min(Math.Max(newLimit, _minPowerLimit), _maxPowerLimit);

            return NvmlDeviceSetPowerManagementLimit(_deviceHandle, newLimit).IsSuccess();
        }

        private void EnsureInitialized()
        {
            if (!_initialized)
            {
                throw new ApplicationException($"{nameof(NvmlManager)} instance not initialized!");
            }
        }

        public void Dispose()
        {
            _deviceHandle = IntPtr.Zero;
            NvmlShutdown();
        }
    }
}
