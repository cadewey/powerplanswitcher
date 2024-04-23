using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PowerPlanSwitcher.Nvidia
{
    internal enum NvApiStatus
    {
        NVAPI_OK = 0,
        NVAPI_ERROR = -1,
        NVAPI_LIBRARY_NOT_FOUND = -2,
        NVAPI_NO_IMPLEMENTATION = -3,
        NVAPI_API_NOT_INITIALIZED = -4,
        NVAPI_INVALID_ARGUMENT = -5,
        NVAPI_NVIDIA_DEVICE_NOT_FOUND = -6,
        NVAPI_END_ENUMERATION = -7,
        NVAPI_INVALID_HANDLE = -8,
        NVAPI_INCOMPATIBLE_STRUCT_VERSION = -9,
        NVAPI_HANDLE_INVALIDATED = -10,
        NVAPI_OPENGL_CONTEXT_NOT_CURRENT = -11,
        NVAPI_INVALID_POINTER = -14,
        NVAPI_NO_GL_EXPERT = -12,
        NVAPI_INSTRUMENTATION_DISABLED = -13,
        NVAPI_NO_GL_NSIGHT = -15,
        NVAPI_EXPECTED_LOGICAL_GPU_HANDLE = -100,
        NVAPI_EXPECTED_PHYSICAL_GPU_HANDLE = -101,
        NVAPI_EXPECTED_DISPLAY_HANDLE = -102,
        NVAPI_INVALID_COMBINATION = -103,
        NVAPI_NOT_SUPPORTED = -104,
        NVAPI_PORTID_NOT_FOUND = -105,
        NVAPI_EXPECTED_UNATTACHED_DISPLAY_HANDLE = -106,
        NVAPI_INVALID_PERF_LEVEL = -107,
        NVAPI_DEVICE_BUSY = -108,
        NVAPI_NV_PERSIST_FILE_NOT_FOUND = -109,
        NVAPI_PERSIST_DATA_NOT_FOUND = -110,
        NVAPI_EXPECTED_TV_DISPLAY = -111,
        NVAPI_EXPECTED_TV_DISPLAY_ON_DCONNECTOR = -112,
        NVAPI_NO_ACTIVE_SLI_TOPOLOGY = -113,
        NVAPI_SLI_RENDERING_MODE_NOTALLOWED = -114,
        NVAPI_EXPECTED_DIGITAL_FLAT_PANEL = -115,
        NVAPI_ARGUMENT_EXCEED_MAX_SIZE = -116,
        NVAPI_DEVICE_SWITCHING_NOT_ALLOWED = -117,
        NVAPI_TESTING_CLOCKS_NOT_SUPPORTED = -118,
        NVAPI_UNKNOWN_UNDERSCAN_CONFIG = -119,
        NVAPI_TIMEOUT_RECONFIGURING_GPU_TOPO = -120,
        NVAPI_DATA_NOT_FOUND = -121,
        NVAPI_EXPECTED_ANALOG_DISPLAY = -122,
        NVAPI_NO_VIDLINK = -123,
        NVAPI_REQUIRES_REBOOT = -124,
        NVAPI_INVALID_HYBRID_MODE = -125,
        NVAPI_MIXED_TARGET_TYPES = -126,
        NVAPI_SYSWOW64_NOT_SUPPORTED = -127,
        NVAPI_IMPLICIT_SET_GPU_TOPOLOGY_CHANGE_NOT_ALLOWED = -128,
        NVAPI_REQUEST_USER_TO_CLOSE_NON_MIGRATABLE_APPS = -129,
        NVAPI_OUT_OF_MEMORY = -130,
        NVAPI_WAS_STILL_DRAWING = -131,
        NVAPI_FILE_NOT_FOUND = -132,
        NVAPI_TOO_MANY_UNIQUE_STATE_OBJECTS = -133,
        NVAPI_INVALID_CALL = -134,
        NVAPI_D3D10_1_LIBRARY_NOT_FOUND = -135,
        NVAPI_FUNCTION_NOT_FOUND = -136,
        NVAPI_INVALID_USER_PRIVILEGE = -137,
        NVAPI_EXPECTED_NON_PRIMARY_DISPLAY_HANDLE = -138,
        NVAPI_EXPECTED_COMPUTE_GPU_HANDLE = -139,
        NVAPI_STEREO_NOT_INITIALIZED = -140,
        NVAPI_STEREO_REGISTRY_ACCESS_FAILED = -141,
        NVAPI_STEREO_REGISTRY_PROFILE_TYPE_NOT_SUPPORTED = -142,
        NVAPI_STEREO_REGISTRY_VALUE_NOT_SUPPORTED = -143,
        NVAPI_STEREO_NOT_ENABLED = -144,
        NVAPI_STEREO_NOT_TURNED_ON = -145,
        NVAPI_STEREO_INVALID_DEVICE_INTERFACE = -146,
        NVAPI_STEREO_PARAMETER_OUT_OF_RANGE = -147,
        NVAPI_STEREO_FRUSTUM_ADJUST_MODE_NOT_SUPPORTED = -148,
        NVAPI_TOPO_NOT_POSSIBLE = -149,
        NVAPI_MODE_CHANGE_FAILED = -150,
        NVAPI_D3D11_LIBRARY_NOT_FOUND = -151,
        NVAPI_INVALID_ADDRESS = -152,
        NVAPI_STRING_TOO_SMALL = -153,
        NVAPI_MATCHING_DEVICE_NOT_FOUND = -154,
        NVAPI_DRIVER_RUNNING = -155,
        NVAPI_DRIVER_NOTRUNNING = -156,
        NVAPI_ERROR_DRIVER_RELOAD_REQUIRED = -157,
        NVAPI_SET_NOT_ALLOWED = -158,
        NVAPI_ADVANCED_DISPLAY_TOPOLOGY_REQUIRED = -159,
        NVAPI_SETTING_NOT_FOUND = -160,
        NVAPI_SETTING_SIZE_TOO_LARGE = -161,
        NVAPI_TOO_MANY_SETTINGS_IN_PROFILE = -162,
        NVAPI_PROFILE_NOT_FOUND = -163,
        NVAPI_PROFILE_NAME_IN_USE = -164,
        NVAPI_PROFILE_NAME_EMPTY = -165,
        NVAPI_EXECUTABLE_NOT_FOUND = -166,
        NVAPI_EXECUTABLE_ALREADY_IN_USE = -167,
        NVAPI_DATATYPE_MISMATCH = -168,
        NVAPI_PROFILE_REMOVED = -169,
        NVAPI_UNREGISTERED_RESOURCE = -170,
        NVAPI_ID_OUT_OF_RANGE = -171,
        NVAPI_DISPLAYCONFIG_VALIDATION_FAILED = -172,
        NVAPI_DPMST_CHANGED = -173,
        NVAPI_INSUFFICIENT_BUFFER = -174,
        NVAPI_ACCESS_DENIED = -175,
        NVAPI_MOSAIC_NOT_ACTIVE = -176,
        NVAPI_SHARE_RESOURCE_RELOCATED = -177,
        NVAPI_REQUEST_USER_TO_DISABLE_DWM = -178,
        NVAPI_D3D_DEVICE_LOST = -179,
        NVAPI_INVALID_CONFIGURATION = -180,
        NVAPI_STEREO_HANDSHAKE_NOT_DONE = -181,
        NVAPI_EXECUTABLE_PATH_IS_AMBIGUOUS = -182,
        NVAPI_DEFAULT_STEREO_PROFILE_IS_NOT_DEFINED = -183,
        NVAPI_DEFAULT_STEREO_PROFILE_DOES_NOT_EXIST = -184,
        NVAPI_CLUSTER_ALREADY_EXISTS = -185,
        NVAPI_DPMST_DISPLAY_ID_EXPECTED = -186,
        NVAPI_INVALID_DISPLAY_ID = -187,
        NVAPI_STREAM_IS_OUT_OF_SYNC = -188,
        NVAPI_INCOMPATIBLE_AUDIO_DRIVER = -189,
        NVAPI_VALUE_ALREADY_SET = -190,
        NVAPI_TIMEOUT = -191,
        NVAPI_GPU_WORKSTATION_FEATURE_INCOMPLETE = -192,
        NVAPI_STEREO_INIT_ACTIVATION_NOT_DONE = -193,
        NVAPI_SYNC_NOT_ACTIVE = -194,
        NVAPI_SYNC_MASTER_NOT_FOUND = -195,
        NVAPI_INVALID_SYNC_TOPOLOGY = -196,
        NVAPI_ECID_SIGN_ALGO_UNSUPPORTED = -197,
        NVAPI_ECID_KEY_VERIFICATION_FAILED = -198,
        NVAPI_FIRMWARE_OUT_OF_DATE = -199,
        NVAPI_FIRMWARE_REVISION_NOT_SUPPORTED = -200,
        NVAPI_LICENSE_CALLER_AUTHENTICATION_FAILED = -201,
        NVAPI_D3D_DEVICE_NOT_REGISTERED = -202,
        NVAPI_RESOURCE_NOT_ACQUIRED = -203,
        NVAPI_TIMING_NOT_SUPPORTED = -204,
        NVAPI_HDCP_ENCRYPTION_FAILED = -205,
        NVAPI_PCLK_LIMITATION_FAILED = -206,
        NVAPI_NO_CONNECTOR_FOUND = -207,
        NVAPI_HDCP_DISABLED = -208,
        NVAPI_API_IN_USE = -209,
        NVAPI_NVIDIA_DISPLAY_NOT_FOUND = -210,
        NVAPI_PRIV_SEC_VIOLATION = -211,
        NVAPI_INCORRECT_VENDOR = -212,
        NVAPI_DISPLAY_IN_USE = -213,
        NVAPI_UNSUPPORTED_CONFIG_NON_HDCP_HMD = -214,
        NVAPI_MAX_DISPLAY_LIMIT_REACHED = -215,
        NVAPI_INVALID_DIRECT_MODE_DISPLAY = -216,
        NVAPI_GPU_IN_DEBUG_MODE = -217,
        NVAPI_D3D_CONTEXT_NOT_FOUND = -218,
        NVAPI_STEREO_VERSION_MISMATCH = -219,
        NVAPI_GPU_NOT_POWERED = -220,
        NVAPI_ERROR_DRIVER_RELOAD_IN_PROGRESS = -221,
        NVAPI_WAIT_FOR_HW_RESOURCE = -222,
        NVAPI_REQUIRE_FURTHER_HDCP_ACTION = -223,
        NVAPI_DISPLAY_MUX_TRANSITION_FAILED = -224,
        NVAPI_INVALID_DSC_VERSION = -225,
        NVAPI_INVALID_DSC_SLICECOUNT = -226,
        NVAPI_INVALID_DSC_OUTPUT_BPP = -227,
        NVAPI_FAILED_TO_LOAD_FROM_DRIVER_STORE = -228,
        NVAPI_NO_VULKAN = -229,
        NVAPI_REQUEST_PENDING = -230,
        NVAPI_RESOURCE_IN_USE = -231,
        NVAPI_INVALID_IMAGE = -232,
        NVAPI_INVALID_PTX = -233,
        NVAPI_NVLINK_UNCORRECTABLE = -234,
        NVAPI_JIT_COMPILER_NOT_FOUND = -235,
        NVAPI_INVALID_SOURCE = -236,
        NVAPI_ILLEGAL_INSTRUCTION = -237,
        NVAPI_INVALID_PC = -238,
        NVAPI_LAUNCH_FAILED = -239,
        NVAPI_NOT_PERMITTED = -240,
        NVAPI_CALLBACK_ALREADY_REGISTERED = -241,
        NVAPI_CALLBACK_NOT_FOUND = -242,
        NVAPI_INVALID_OUTPUT_WIRE_FORMAT = -243
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

        NvAPI_Disp_HdrColorControl = 0x351DA224,
        NvAPI_EnumPhysicalGPUs = 0xE5AC921F,
        NvAPI_GPU_GetAllDisplayIds = 0x785210A2,
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

    internal enum NvDynamicRange
    {
        NV_DYNAMIC_RANGE_VESA,
        NV_DYNAMIC_RANGE_CEA,
        NV_DYNAMIC_RANGE_AUTO = 0xFF
    }

    internal enum NvHdrCmd
    {
        NV_HDR_CMD_GET,
        NV_HDR_CMD_SET
    }

    internal enum NvHdrMode
    {
        NV_HDR_MODE_OFF = 0,
        NV_HDR_MODE_UHDA = 2,
        NV_HDR_MODE_UHDBD = 2,
        NV_HDR_MODE_EDR = 3,
        NV_HDR_MODE_SDR = 4,
        NV_HDR_MODE_UHDA_PASSTHROUGH = 5,
        NV_HDR_MODE_UHDA_NB = 6,
        NV_HDR_MODE_DOLBY_VISION = 7
    }

    internal enum NvStaticMetadataDescriptorId
    {
        NV_STATIC_METADATA_TYPE_1
    }

    internal enum NvMonitorConnType
    {
        NV_MONITOR_CONN_TYPE_UNINITIALIZED,
        NV_MONITOR_CONN_TYPE_VGA,
        NV_MONITOR_CONN_TYPE_COMPONENT,
        NV_MONITOR_CONN_TYPE_SVIDEO,
        NV_MONITOR_CONN_TYPE_HDMI,
        NV_MONITOR_CONN_TYPE_DVI,
        NV_MONITOR_CONN_TYPE_LVDS,
        NV_MONITOR_CONN_TYPE_DP,
        NV_MONITOR_CONN_TYPE_COMPOSITE,
        NV_MONITOR_CONN_TYPE_UNKNOWN
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

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NvMasteringDisplayDataV2
    {
        internal ushort DisplayPrimaryX0;
        internal ushort DisplayPrimaryY0;
        internal ushort DisplayPrimaryX1;
        internal ushort DisplayPrimaryY1;
        internal ushort DisplayPrimaryX2;
        internal ushort DisplayPrimaryY2;
        internal ushort DisplayWhitePointX;
        internal ushort DisplayWhitePointY;
        internal ushort MaxDisplayMasteringLuminance;
        internal ushort MinDisplayMasteringLuminance;
        internal ushort MaxContentLightLevel;
        internal ushort MaxFrameAverageLightLevel;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NvHdrColorDataV2
    {
        internal uint Version;
        internal NvHdrCmd Cmd;
        internal NvHdrMode HdrMode;
        internal NvStaticMetadataDescriptorId StaticMetadataDescriptorId;
        internal NvMasteringDisplayDataV2 MasteringDisplayData;
        internal int HdrColorFormat;
        internal NvDynamicRange DynamicRange;
        internal int HdrBpc;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NvGpuDisplayIds
    {
        internal uint Version;
        internal NvMonitorConnType ConnectorType;
        internal uint DisplayId;
        private uint _displayData;

        public NvGpuDisplayIds()
        {
            Version = (uint)Marshal.SizeOf(typeof(NvGpuDisplayIds)) | 0x30000;
        }

        public bool IsDynamic => (_displayData & 0x8000000) == 0x80000000;
        public bool IsMultiStreamRootNode => (_displayData & 0x40000000) == 0x40000000;
        public bool IsActive => (_displayData & 0x20000000) == 0x20000000;
        public bool IsCluster => (_displayData & 0x10000000) == 0x10000000;
        public bool IsOsVisible => (_displayData & 0x08000000) == 0x08000000;
        public bool IsWFD => (_displayData & 0x04000000) == 0x04000000;
        public bool IsConnected => (_displayData & 0x02000000) == 0x02000000;
        public bool IsPhysicallyConnected => (_displayData & 0x00004000) == 0x00004000;
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
        private delegate NvApiStatus NvAPI_GPU_GetAllDisplayIds(IntPtr hPhysicalGpu, [In, Out] NvGpuDisplayIds[] pDisplayIds, ref uint pDisplayIdCount);
        private delegate NvApiStatus NvAPI_GPU_GetAllClockFrequencies(IntPtr hPhysicalGpu, ref NvApiClockFrequenciesV2 frequencies);
        private delegate NvApiStatus NvAPI_Disp_HdrColorControl(uint displayId, ref NvHdrColorDataV2 colorData);

        public static NvApiStatus Initialize() => GetDelegate<NvApi_Initialize>(NvApiFunctionAddress.NvAPI_Initialize)();
        public static NvApiStatus Unload() => GetDelegate<NvAPI_Unload>(NvApiFunctionAddress.NvAPI_Unload)();

        public static NvApiStatus EnumPhysicalGpus([Out] IntPtr[] gpuHandles, out uint gpuCount) =>
            GetDelegate<NvAPI_EnumPhysicalGPUs>(NvApiFunctionAddress.NvAPI_EnumPhysicalGPUs)(gpuHandles, out gpuCount);

        public static NvApiStatus GetAllClockFrequencies(IntPtr handle, ref NvApiClockFrequenciesV2 frequencies) =>
            GetDelegate<NvAPI_GPU_GetAllClockFrequencies>(NvApiFunctionAddress.NvAPI_GPU_GetAllClockFrequencies)(handle, ref frequencies);

        public static NvApiStatus DispHdrColorMode(uint displayId, ref NvHdrColorDataV2 colorData) =>
            GetDelegate<NvAPI_Disp_HdrColorControl>(NvApiFunctionAddress.NvAPI_Disp_HdrColorControl)(displayId, ref colorData);

        public static NvApiStatus GetAllDisplayIds(IntPtr gpuHandle, [In, Out] NvGpuDisplayIds[] gpuDisplayIds, ref uint displayIdCount) =>
            GetDelegate<NvAPI_GPU_GetAllDisplayIds>(NvApiFunctionAddress.NvAPI_GPU_GetAllDisplayIds)(gpuHandle, gpuDisplayIds, ref displayIdCount);

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

        public static uint MakeNvApiVersion<T>(uint version)
        {
            uint sizeOfType = (uint)Marshal.SizeOf(typeof(T));
            return sizeOfType | version << 16;
        }
    }
}
