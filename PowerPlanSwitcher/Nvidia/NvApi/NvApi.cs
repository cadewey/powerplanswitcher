using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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
        NVAPI_INVALID_POINTER = -12,
        NVAPI_NO_GL_EXPERT = -13,
        NVAPI_INSTRUMENTATION_DISABLED = -14,
        NVAPI_NO_GL_NSIGHT = -15,
        NVAPI_EXPECTED_LOGICAL_GPU_HANDLE = -16,
        NVAPI_EXPECTED_PHYSICAL_GPU_HANDLE = -17,
        NVAPI_EXPECTED_DISPLAY_HANDLE = -18,
        NVAPI_INVALID_COMBINATION = -19,
        NVAPI_NOT_SUPPORTED = -20,
        NVAPI_PORTID_NOT_FOUND = -21,
        NVAPI_EXPECTED_UNATTACHED_DISPLAY_HANDLE = -22,
        NVAPI_INVALID_PERF_LEVEL = -23,
        NVAPI_DEVICE_BUSY = -24,
        NVAPI_NV_PERSIST_FILE_NOT_FOUND = -25,
        NVAPI_PERSIST_DATA_NOT_FOUND = -26,
        NVAPI_EXPECTED_TV_DISPLAY = -27,
        NVAPI_EXPECTED_TV_DISPLAY_ON_DCONNECTOR = -28,
        NVAPI_NO_ACTIVE_SLI_TOPOLOGY = -29,
        NVAPI_SLI_RENDERING_MODE_NOTALLOWED = -30,
        NVAPI_EXPECTED_DIGITAL_FLAT_PANEL = -31,
        NVAPI_ARGUMENT_EXCEED_MAX_SIZE = -32,
        NVAPI_DEVICE_SWITCHING_NOT_ALLOWED = -33,
        NVAPI_TESTING_CLOCKS_NOT_SUPPORTED = -34,
        NVAPI_UNKNOWN_UNDERSCAN_CONFIG = -35,
        NVAPI_TIMEOUT_RECONFIGURING_GPU_TOPO = -36,
        NVAPI_DATA_NOT_FOUND = -37,
        NVAPI_EXPECTED_ANALOG_DISPLAY = -38,
        NVAPI_NO_VIDLINK = -39,
        NVAPI_REQUIRES_REBOOT = -40,
        NVAPI_INVALID_HYBRID_MODE = -41,
        NVAPI_MIXED_TARGET_TYPES = -42,
        NVAPI_SYSWOW64_NOT_SUPPORTED = -43,
        NVAPI_IMPLICIT_SET_GPU_TOPOLOGY_CHANGE_NOT_ALLOWED = -44,
        NVAPI_REQUEST_USER_TO_CLOSE_NON_MIGRATABLE_APPS = -45,
        NVAPI_OUT_OF_MEMORY = -46,
        NVAPI_WAS_STILL_DRAWING = -47,
        NVAPI_FILE_NOT_FOUND = -48,
        NVAPI_TOO_MANY_UNIQUE_STATE_OBJECTS = -49,
        NVAPI_INVALID_CALL = -50,
        NVAPI_D3D10_1_LIBRARY_NOT_FOUND = -51,
        NVAPI_FUNCTION_NOT_FOUND = -52,
        NVAPI_INVALID_USER_PRIVILEGE = -53,
        NVAPI_EXPECTED_NON_PRIMARY_DISPLAY_HANDLE = -54,
        NVAPI_EXPECTED_COMPUTE_GPU_HANDLE = -55,
        NVAPI_STEREO_NOT_INITIALIZED = -56,
        NVAPI_STEREO_REGISTRY_ACCESS_FAILED = -57,
        NVAPI_STEREO_REGISTRY_PROFILE_TYPE_NOT_SUPPORTED = -58,
        NVAPI_STEREO_REGISTRY_VALUE_NOT_SUPPORTED = -59,
        NVAPI_STEREO_NOT_ENABLED = -60,
        NVAPI_STEREO_NOT_TURNED_ON = -61,
        NVAPI_STEREO_INVALID_DEVICE_INTERFACE = -62,
        NVAPI_STEREO_PARAMETER_OUT_OF_RANGE = -63,
        NVAPI_STEREO_FRUSTUM_ADJUST_MODE_NOT_SUPPORTED = -64,
        NVAPI_TOPO_NOT_POSSIBLE = -65,
        NVAPI_MODE_CHANGE_FAILED = -66,
        NVAPI_D3D11_LIBRARY_NOT_FOUND = -67,
        NVAPI_INVALID_ADDRESS = -68,
        NVAPI_STRING_TOO_SMALL = -69,
        NVAPI_MATCHING_DEVICE_NOT_FOUND = -70,
        NVAPI_DRIVER_RUNNING = -71,
        NVAPI_DRIVER_NOTRUNNING = -72,
        NVAPI_ERROR_DRIVER_RELOAD_REQUIRED = -73,
        NVAPI_SET_NOT_ALLOWED = -74,
        NVAPI_ADVANCED_DISPLAY_TOPOLOGY_REQUIRED = -75,
        NVAPI_SETTING_NOT_FOUND = -76,
        NVAPI_SETTING_SIZE_TOO_LARGE = -77,
        NVAPI_TOO_MANY_SETTINGS_IN_PROFILE = -78,
        NVAPI_PROFILE_NOT_FOUND = -79,
        NVAPI_PROFILE_NAME_IN_USE = -80,
        NVAPI_PROFILE_NAME_EMPTY = -81,
        NVAPI_EXECUTABLE_NOT_FOUND = -82,
        NVAPI_EXECUTABLE_ALREADY_IN_USE = -83,
        NVAPI_DATATYPE_MISMATCH = -84,
        NVAPI_PROFILE_REMOVED = -85,
        NVAPI_UNREGISTERED_RESOURCE = -86,
        NVAPI_ID_OUT_OF_RANGE = -87,
        NVAPI_DISPLAYCONFIG_VALIDATION_FAILED = -88,
        NVAPI_DPMST_CHANGED = -89,
        NVAPI_INSUFFICIENT_BUFFER = -90,
        NVAPI_ACCESS_DENIED = -91,
        NVAPI_MOSAIC_NOT_ACTIVE = -92,
        NVAPI_SHARE_RESOURCE_RELOCATED = -93,
        NVAPI_REQUEST_USER_TO_DISABLE_DWM = -94,
        NVAPI_D3D_DEVICE_LOST = -95,
        NVAPI_INVALID_CONFIGURATION = -96,
        NVAPI_STEREO_HANDSHAKE_NOT_DONE = -97,
        NVAPI_EXECUTABLE_PATH_IS_AMBIGUOUS = -98,
        NVAPI_DEFAULT_STEREO_PROFILE_IS_NOT_DEFINED = -99,
        NVAPI_DEFAULT_STEREO_PROFILE_DOES_NOT_EXIST = -100,
        NVAPI_CLUSTER_ALREADY_EXISTS = -101,
        NVAPI_DPMST_DISPLAY_ID_EXPECTED = -102,
        NVAPI_INVALID_DISPLAY_ID = -103,
        NVAPI_STREAM_IS_OUT_OF_SYNC = -104,
        NVAPI_INCOMPATIBLE_AUDIO_DRIVER = -105,
        NVAPI_VALUE_ALREADY_SET = -106,
        NVAPI_TIMEOUT = -107,
        NVAPI_GPU_WORKSTATION_FEATURE_INCOMPLETE = -108,
        NVAPI_STEREO_INIT_ACTIVATION_NOT_DONE = -109,
        NVAPI_SYNC_NOT_ACTIVE = -110,
        NVAPI_SYNC_MASTER_NOT_FOUND = -111,
        NVAPI_INVALID_SYNC_TOPOLOGY = -112,
        NVAPI_ECID_SIGN_ALGO_UNSUPPORTED = -113,
        NVAPI_ECID_KEY_VERIFICATION_FAILED = -114,
        NVAPI_FIRMWARE_OUT_OF_DATE = -115,
        NVAPI_FIRMWARE_REVISION_NOT_SUPPORTED = -116,
        NVAPI_LICENSE_CALLER_AUTHENTICATION_FAILED = -117,
        NVAPI_D3D_DEVICE_NOT_REGISTERED = -118,
        NVAPI_RESOURCE_NOT_ACQUIRED = -119,
        NVAPI_TIMING_NOT_SUPPORTED = -120,
        NVAPI_HDCP_ENCRYPTION_FAILED = -121,
        NVAPI_PCLK_LIMITATION_FAILED = -122,
        NVAPI_NO_CONNECTOR_FOUND = -123,
        NVAPI_HDCP_DISABLED = -124,
        NVAPI_API_IN_USE = -125,
        NVAPI_NVIDIA_DISPLAY_NOT_FOUND = -126,
        NVAPI_PRIV_SEC_VIOLATION = -127,
        NVAPI_INCORRECT_VENDOR = -128,
        NVAPI_DISPLAY_IN_USE = -129,
        NVAPI_UNSUPPORTED_CONFIG_NON_HDCP_HMD = -130,
        NVAPI_MAX_DISPLAY_LIMIT_REACHED = -131,
        NVAPI_INVALID_DIRECT_MODE_DISPLAY = -132,
        NVAPI_GPU_IN_DEBUG_MODE = -133,
        NVAPI_D3D_CONTEXT_NOT_FOUND = -134,
        NVAPI_STEREO_VERSION_MISMATCH = -135,
        NVAPI_GPU_NOT_POWERED = -136,
        NVAPI_ERROR_DRIVER_RELOAD_IN_PROGRESS = -137,
        NVAPI_WAIT_FOR_HW_RESOURCE = -138,
        NVAPI_REQUIRE_FURTHER_HDCP_ACTION = -139,
        NVAPI_DISPLAY_MUX_TRANSITION_FAILED = -140,
        NVAPI_INVALID_DSC_VERSION = -141,
        NVAPI_INVALID_DSC_SLICECOUNT = -142,
        NVAPI_INVALID_DSC_OUTPUT_BPP = -143,
        NVAPI_FAILED_TO_LOAD_FROM_DRIVER_STORE = -144,
        NVAPI_NO_VULKAN = -145,
        NVAPI_REQUEST_PENDING = -146,
        NVAPI_RESOURCE_IN_USE = -147,
        NVAPI_INVALID_IMAGE = -148,
        NVAPI_INVALID_PTX = -149,
        NVAPI_NVLINK_UNCORRECTABLE = -150,
        NVAPI_JIT_COMPILER_NOT_FOUND = -151,
        NVAPI_INVALID_SOURCE = -152,
        NVAPI_ILLEGAL_INSTRUCTION = -153,
        NVAPI_INVALID_PC = -154,
        NVAPI_LAUNCH_FAILED = -155,
        NVAPI_NOT_PERMITTED = -156,
        NVAPI_CALLBACK_ALREADY_REGISTERED = -157,
        NVAPI_CALLBACK_NOT_FOUND = -158
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
