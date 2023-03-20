using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PowerPlanSwitcher.Nvidia
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

    internal enum NvmlClockType
    {
        NVML_CLOCK_GRAPHICS = 0,
        NVML_CLOCK_SM = 1,
        NVML_CLOCK_MEM = 2,
        NVML_CLOCK_VIDEO = 3
    }

    internal enum NvmlClockId
    {
        NVML_CLOCK_ID_CURRENT = 0,
        NVML_CLOCK_ID_APP_CLOCK_TARGET = 1,
        NVML_CLOCK_ID_APP_CLOCK_DEFAULT = 2,
        NVML_CLOCK_ID_CUSTOMER_BOOST_MAX = 3
    }

    internal enum NvmlTempteratureSensor
    {
        NVML_TEMPERATURE_GPU = 0
    }

    internal enum NvmlTemperatureThreshold
    {
        NVML_TEMPERATURE_THRESHOLD_SHUTDOWN = 0,
        NVML_TEMPERATURE_THRESHOLD_SLOWDOWN = 1,
        NVML_TEMPERATURE_THRESHOLD_MEM_MAX = 2,
        NVML_TEMPERATURE_THRESHOLD_GPU_MAX = 3
    }

    internal static class NvmlReturnExtensions
    {
        internal static bool IsSuccess(this NvmlReturn ret)
        {
            return ret == NvmlReturn.NVML_SUCCESS;
        }
    }

    // NB: the docs list these struct properties in alphabetical order, but in memory they're ordered as below
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    internal struct NvmlMemory
    {
        /* All values are in bytes */
        public ulong Total;
        public ulong Free;
        public ulong Used;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    internal struct NvmlUtilization
    {
        public uint Gpu;
        public uint Memory;
    }

    internal class NvmlApi
    {
        [DllImport("nvml.dll", EntryPoint = "nvmlInit_v2")]
        internal static extern NvmlReturn NvmlInitV2();
        [DllImport("nvml.dll", EntryPoint = "nvmlShutdown")]
        internal static extern NvmlReturn NvmlShutdown();

        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetCount_v2")]
        internal static extern NvmlReturn NvmlDeviceGetCount_v2(out uint deviceCount);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetHandleByIndex")]
        internal static extern NvmlReturn NvmlDeviceGetHandleByIndex(uint index, out IntPtr device);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetName")]
        internal static extern NvmlReturn NvmlDeviceGetName(IntPtr device, StringBuilder name, uint length);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetPowerManagementLimitConstraints")]
        internal static extern NvmlReturn NvmlDeviceGetPowerManagementLimitConstraints(IntPtr device, out uint minLimit /* mW */, out uint maxLimit /* mW */);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetPowerManagementDefaultLimit")]
        internal static extern NvmlReturn NvmlDeviceGetPowerManagementDefaultLimit(IntPtr device, out uint defaultLimit /* mW */);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetPowerManagementLimit")]
        internal static extern NvmlReturn NvmlDeviceGetPowerManagementLimit(IntPtr device, out uint limit /* mW */);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetTemperature")]
        internal static extern NvmlReturn NvmlDeviceGetTemperature(IntPtr device, NvmlTempteratureSensor sensorType, out uint temp /* degrees C */);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetTemperatureThreshold")]
        internal static extern NvmlReturn NvmlDeviceGetTemperatureThreshold(IntPtr device, NvmlTemperatureThreshold thresholdType, out uint temp /* degrees C */);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetPowerUsage")]
        internal static extern NvmlReturn NvmlDeviceGetPowerUsage(IntPtr device, out uint power /* mW */);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetVbiosVersion")]
        internal static extern NvmlReturn NvmlDeviceGetVbiosVersion(IntPtr device, StringBuilder version, uint length);
        [DllImport("nvml.dll", EntryPoint = "nvmlSystemGetDriverVersion")]
        internal static extern NvmlReturn NvmlSystemGetDriverVersion(StringBuilder version, uint length);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetCurrPcieLinkGeneration")]
        internal static extern NvmlReturn NvmlDeviceGetCurrPcieLinkGeneration(IntPtr device, out uint currLinkGen);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetCurrPcieLinkWidth")]
        internal static extern NvmlReturn NvmlDeviceGetCurrPcieLinkWidth(IntPtr device, out uint currLinkWidth);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetNumGpuCores")]
        internal static extern NvmlReturn NvmlDeviceGetNumGpuCores(IntPtr device, out uint numCores);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetMemoryBusWidth")]
        internal static extern NvmlReturn NvmlDeviceGetMemoryBusWidth(IntPtr device, out uint busWidth);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetMemoryInfo")]
        internal static extern NvmlReturn NvmlDeviceGetMemoryInfo(IntPtr device, out NvmlMemory memory);
        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceGetUtilizationRates")]
        internal static extern NvmlReturn NvmlDeviceGetUtilizationRates(IntPtr device, out NvmlUtilization utilization);

        [DllImport("nvml.dll", EntryPoint = "nvmlDeviceSetPowerManagementLimit")]
        internal static extern NvmlReturn NvmlDeviceSetPowerManagementLimit(IntPtr device, uint limit /* mW */);

        // https://docs.nvidia.com/deploy/nvml-api/group__nvmlConstants.html
        internal const uint DeviceNameBufferSize = 96;
        internal const uint DeviceVbiosVersionBufferSize = 32;
        internal const uint SystemDriverVersionBufferSize = 80;
    }
}
