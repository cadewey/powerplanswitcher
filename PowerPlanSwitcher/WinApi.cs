using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PowerPlanSwitcher
{
    internal static class WinApi
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct DevMode
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 106)]
            readonly byte[] starPadding;
            public int width;
            public int height;
            public int flags; // We don't really use this, but we need to account for it in the struct layout
            public int frequency;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            readonly byte[] endPadding;
        }

        internal enum WinErrorCode
        {
            ERROR_SUCCESS = 0,
            ERROR_ACCESS_DENIED = 5,
            ERROR_GEN_FAILURE = 31,
            ERROR_NOT_SUPPORTED = 50,
            ERROR_INVALID_PARAMETER = 87
        }

        internal enum DisplayConfigFlags
        {
            QDC_ALL_PATHS,
            QDC_ONLY_ACTIVE_PATHS = 2,
            QDC_DATABASE_CURRENT = 4
        }

        internal enum DisplayConfigVideoOutputTechnology : uint
        {
            //DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = -1,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 = 0,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO = 1,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO = 2,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO = 3,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI = 4,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI = 5,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS = 6,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN = 8,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI = 9,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL = 12,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED = 13,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE = 14,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST = 15,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INDIRECT_WIRED = 16,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INDIRECT_VIRTUAL = 17,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_USB_TUNNEL,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL = 0x80000000
        }

        internal enum DisplayConfigRotation : uint
        {
            DISPLAYCONFIG_ROTATION_IDENTITY = 1,
            DISPLAYCONFIG_ROTATION_ROTATE90 = 2,
            DISPLAYCONFIG_ROTATION_ROTATE180 = 3,
            DISPLAYCONFIG_ROTATION_ROTATE270 = 4
        }

        internal enum DisplayConfigScaling : uint
        {
            DISPLAYCONFIG_SCALING_IDENTITY = 1,
            DISPLAYCONFIG_SCALING_CENTERED = 2,
            DISPLAYCONFIG_SCALING_STRETCHED = 3,
            DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4,
            DISPLAYCONFIG_SCALING_CUSTOM = 5,
            DISPLAYCONFIG_SCALING_PREFERRED = 128
        }

        internal enum DisplayConfigScanlineOrdering : uint
        {
            DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED = 0,
            DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE = 1,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED = 2,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3
        }

        internal enum DisplayConfigTopology : uint
        {
            DISPLAYCONFIG_TOPOLOGY_INTERNAL = 0x00000001,
            DISPLAYCONFIG_TOPOLOGY_CLONE = 0x00000002,
            DISPLAYCONFIG_TOPOLOGY_EXTEND = 0x00000004,
            DISPLAYCONFIG_TOPOLOGY_EXTERNAL = 0x00000008
        }

        internal enum DisplayConfigModeInfoType : uint
        {
            DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,
            DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,
            DISPLAYCONFIG_MODE_INFO_TYPE_DESKTOP_IMAGE = 3
        }

        internal enum DisplayConfigPixelFormat : uint
        {
            DISPLAYCONFIG_PIXELFORMAT_8BPP = 1,
            DISPLAYCONFIG_PIXELFORMAT_16BPP = 2,
            DISPLAYCONFIG_PIXELFORMAT_24BPP = 3,
            DISPLAYCONFIG_PIXELFORMAT_32BPP = 4,
            DISPLAYCONFIG_PIXELFORMAT_NONGDI = 5
        }

        internal enum DisplayConfigDeviceInfoType : uint
        {
            DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME = 1,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3,
            DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME = 4,
            DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE = 5,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE = 6,
            DISPLAYCONFIG_DEVICE_INFO_GET_SUPPORT_VIRTUAL_RESOLUTION = 7,
            DISPLAYCONFIG_DEVICE_INFO_SET_SUPPORT_VIRTUAL_RESOLUTION = 8,
            DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO = 9,
            DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE = 10,
            DISPLAYCONFIG_DEVICE_INFO_GET_SDR_WHITE_LEVEL = 11,
            DISPLAYCONFIG_DEVICE_INFO_GET_MONITOR_SPECIALIZATION,
            DISPLAYCONFIG_DEVICE_INFO_SET_MONITOR_SPECIALIZATION,
            DISPLAYCONFIG_DEVICE_INFO_SET_RESERVED1,
            DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO_2,
            DISPLAYCONFIG_DEVICE_INFO_SET_HDR_STATE,
            DISPLAYCONFIG_DEVICE_INFO_SET_WCG_STATE
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LUID
        {
            internal uint LowPart;
            internal long HightPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DisplayConfigRational
        {
            internal uint Numerator;
            internal uint Denominator;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DisplayConfigPathSourceInfo
        {
            internal LUID AdapterId;
            internal uint Id;
            private uint Union;
            internal uint StatusFlags;

            internal uint ModeInfoIndex => Union;
            internal uint CloneGroupId => (Union >> 16) & 0x0000ffff;
            internal uint SourceModeInfoIndex => Union & 0x0000ffff;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DisplayConfigPathTargetInfo
        {
            internal LUID AdapterId;
            internal uint Id;
            private uint Union;
            internal DisplayConfigVideoOutputTechnology OutputTechnology;
            internal DisplayConfigRotation Rotation;
            internal DisplayConfigScaling Scaling;
            internal DisplayConfigRational RefreshRate;
            internal DisplayConfigScanlineOrdering ScanlineOrdering;
            internal bool TargetAvailable;
            internal uint StatusFlags;

            internal uint ModeInfoIdx => Union;
            internal uint DesktopModeInfoIndex => (Union >> 16) & 0x0000ffff;
            internal uint TargetModeInfoIndex => Union & 0x0000ffff;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DisplayConfigPathInfo
        {
            internal DisplayConfigPathSourceInfo sourceInfo;
            internal DisplayConfigPathTargetInfo targetInfo;
            internal uint Flags;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct DisplayConfigModeInfo
        {
            [FieldOffset(0)]
            internal DisplayConfigModeInfoType InfoType;
            [FieldOffset(1)]
            internal uint Id;
            [FieldOffset(2)]
            internal LUID AdapterId;
            [FieldOffset(5)]
            internal DisplayConfigTargetMode TargetMode;
            [FieldOffset(5)]
            internal DisplayConfigSourceMode SourceMode;
            [FieldOffset(5)]
            internal DisplayConfigDesktopImageInfo DesktopImageInfo;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct DisplayConfigVideoSignalInfo
        {
            [FieldOffset(0)]
            internal ulong PixelRate;
            [FieldOffset(2)]
            internal DisplayConfigRational HSyncFreq;
            [FieldOffset(4)]
            internal DisplayConfigRational VSyncFreq;
            [FieldOffset(6)]
            internal DisplayConfig2DRegion ActiveSize;
            [FieldOffset(8)]
            internal DisplayConfig2DRegion TotalSize;
            [FieldOffset(10)]
            internal uint AdditionalSignalInfo;
            [FieldOffset(10)]
            internal uint VideoStandard;
            [FieldOffset(11)]
            internal DisplayConfigScanlineOrdering ScanLineOrdering;

            internal uint AdditionalSignalInfo_VideoStandard => (AdditionalSignalInfo >> 16) & 0x0000ffff;
            internal uint AdditionalSignalInfo_VSyncFreqProvider => (AdditionalSignalInfo >> 10) & 0x0000003f;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DisplayConfig2DRegion
        {
            internal uint Cx;
            internal uint Cy;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DisplayConfigTargetMode
        {
            internal DisplayConfigVideoSignalInfo TargetVideoSignalInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DisplayConfigSourceMode
        {
            internal uint Width;
            internal uint Height;
            internal DisplayConfigPixelFormat PixelFormat;
            internal PointL Position;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DisplayConfigDesktopImageInfo
        {
            internal PointL PathSourceSize;
            internal RectL DesktopImageRegion;
            internal RectL DesktopImageClip;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PointL
        {
            internal long X;
            internal long Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RectL
        {
            internal long Left;
            internal long Top;
            internal long Right;
            internal long Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DisplayConfigDeviceInfoHeader
        {
            internal DisplayConfigDeviceInfoType Type;
            internal uint Size;
            internal LUID AdapterId;
            internal uint Id;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DisplayConfigTargetDeviceName
        {
            internal DisplayConfigDeviceInfoHeader Header;
            internal DisplayConfigTargetDeviceNameFlags Flags;
            internal DisplayConfigVideoOutputTechnology OutputTechnology;
            internal ushort EdidManufactureId;
            internal ushort EdidProductCodeId;
            internal uint ConnectorInstance;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=64)]
            internal string MonitorFriendlyDeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)]
            internal string MonitorDevicePath;

            public DisplayConfigTargetDeviceName()
            {
                Header = new DisplayConfigDeviceInfoHeader
                {
                    Type = DisplayConfigDeviceInfoType.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME
                };

                Header.Size = (uint)Marshal.SizeOf(typeof(DisplayConfigTargetDeviceName));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DisplayConfigTargetDeviceNameFlags
        {
            internal uint InternalStruct;
            internal uint Value;

            internal bool FriendlyNameFromEdid => ((InternalStruct >> 31) & 0x1) == 0x1;
            internal bool FriendlyNameForces => ((InternalStruct >> 30) & 0x1) == 0x1;
            internal bool EdidIdsValid => ((InternalStruct >> 29) & 0x1) == 0x1;
        }

        [DllImport("user32.dll")]
        internal static extern bool EnumDisplaySettings(string deviceName, int nodeNumber, ref DevMode devMode);
        [DllImport("user32.dll")]
        internal static extern int ChangeDisplaySettings(ref DevMode devMode, int flags);
        [DllImport("user32.dll")]
        internal static extern WinErrorCode GetDisplayConfigBufferSizes(DisplayConfigFlags flags, out int numPathArrayElements, out int numModeInfoArrayElements);

        [DllImport("user32.dll")]
        internal static extern WinErrorCode QueryDisplayConfig(
            DisplayConfigFlags flags,
            ref int numPathArrayElements,
            [Out] DisplayConfigPathInfo[] pathInfoArray,
            ref int numModeInfoArrayElements,
            [Out] DisplayConfigModeInfo[] modeInfoArray,
            IntPtr currentTopologyId
        );

        [DllImport("user32.dll")]
        internal static extern WinErrorCode DisplayConfigGetDeviceInfo(ref DisplayConfigTargetDeviceName requestPacket);
    }
}
