using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PowerPlanSwitcher.Nvidia
{
    internal enum NvApiStatus
    {
        NVAPI_OK = 0
    }

    internal static class NvApiStatusExtensions
    {
        internal static bool IsSuccess(this NvApiStatus ret)
        {
            return ret == NvApiStatus.NVAPI_OK;
        }
    }

    internal enum NvApiFunctionAddress : uint
    {
        NvAPI_Unload = 0xD22BDD7E,
        NvAPI_Initialize = 0x150E828,

        NvAPI_EnumPhysicalGPUs = 0xE5AC921F,
        NvAPI_GPU_GetFullName = 0xCEEE8E9F,
        NvAPI_GPU_GetAllClockFrequencies = 0xDCB616C3
    }

    internal enum NvApiClockId
    {
        NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS,
        NVAPI_GPU_PUBLIC_CLOCK_MEMORY = 4,
        NVAPI_GPU_PUBLIC_CLOCK_PROCESSOR = 8,
        NVAPI_GPU_PUBLIC_CLOCK_VIDEO = 12,
        NVAPI_GPU_PUBLIC_CLOCK_UNDEFINED
    }

    internal enum NvApiClockType
    {
        NV_GPU_CLOCK_FREQUENCIES_CURRENT_FREQ,
        NV_GPU_CLOCK_FREQUENCIES_BASE_CLOCK,
        NV_GPU_CLOCK_FREQUENCIES_BOOST_CLOCK,
        NV_GPU_CLOCK_FREQUENCIES_CLOCK_TYPE_NUM
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NvApiClockFrequencyInfo
    {
        internal uint IsPresent;
        internal uint Frequency;

        internal bool Present => (IsPresent & 0x1) == 0x1;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NvApiClockFrequenciesV2
    {
        internal uint Version;
        internal uint ClockType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        internal NvApiClockFrequencyInfo[] Domain;

        public NvApiClockFrequenciesV2(NvApiClockType type)
        {
            Version = (uint)Marshal.SizeOf(typeof(NvApiClockFrequenciesV2)) | 0x20000;
            ClockType = (uint)type;
            Domain = new NvApiClockFrequencyInfo[32];
        }
    }

    internal class NvApi
    {
        public const uint MaxPhysicalGPUs = 64;
        public const int ShortStringMaxLen = 64;

        private static readonly Dictionary<NvApiFunctionAddress, object> _delegates = new Dictionary<NvApiFunctionAddress, object>();

        [DllImport("nvapi64.dll", EntryPoint = "nvapi_QueryInterface", CallingConvention = CallingConvention.Cdecl, PreserveSig = true)]
        private static extern IntPtr NvAPI_QueryInterface(uint interfaceId);

        private delegate NvApiStatus NvApi_Initialize();
        private delegate NvApiStatus NvAPI_Unload();
        private delegate NvApiStatus NvAPI_EnumPhysicalGPUs([Out] IntPtr[] gpuHandles, out uint gpuCount);
        private delegate NvApiStatus NvAPI_GPU_GetAllClockFrequencies(IntPtr hPhysicalGpu, ref NvApiClockFrequenciesV2 frequencies);

        public static NvApiStatus Initialize() => GetDelegate<NvApi_Initialize>(NvApiFunctionAddress.NvAPI_Initialize)();
        public static NvApiStatus Unload() => GetDelegate<NvAPI_Unload>(NvApiFunctionAddress.NvAPI_Unload)();

        public static NvApiStatus EnumPhysicalGpus([Out] IntPtr[] gpuHandles, out uint gpuCount) =>
            GetDelegate<NvAPI_EnumPhysicalGPUs>(NvApiFunctionAddress.NvAPI_EnumPhysicalGPUs)(gpuHandles, out gpuCount);

        public static NvApiStatus GetAllClockFrequencies(IntPtr handle, ref NvApiClockFrequenciesV2 frequencies) =>
            GetDelegate<NvAPI_GPU_GetAllClockFrequencies>(NvApiFunctionAddress.NvAPI_GPU_GetAllClockFrequencies)(handle, ref frequencies);

        private static T GetDelegate<T>(NvApiFunctionAddress address)
        {
            if (_delegates.TryGetValue(address, out object del))
            {
                return (T)del;
            }

            IntPtr functionPtr = NvAPI_QueryInterface((uint)address);

            if (functionPtr != IntPtr.Zero)
            {
                T func = Marshal.GetDelegateForFunctionPointer<T>(functionPtr);
                _delegates.Add(address, func);
                return func;
            }

            throw new ArgumentException($"Couldn't find delegate for function address {address}");
        }
    }
}
