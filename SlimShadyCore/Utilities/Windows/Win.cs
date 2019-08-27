using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SlimShadyCore.Utilities.Windows
{
    [SuppressMessage("ReSharper", "InconsistentNaming")] // Names are copied from WinApi
    public partial class Win
    {
        public const int WM_DEVICECHANGE = 0x0219;

        // From Dbt.h header file
        public const int DBT_CONFIGCHANGECANCELED = 0x0019; // A request to change the current configuration(dock or undock) has been canceled.
        public const int DBT_CONFIGCHANGED = 0x0018; // The current configuration has changed, due to a dock or undock.
        public const int DBT_CUSTOMEVENT = 0x8006; // A custom event has occurred.
        public const int DBT_DEVICEARRIVAL = 0x8000; // A device or piece of media has been inserted and is now available.
        public const int DBT_DEVICEQUERYREMOVE = 0x8001; // Permission is requested to remove a device or piece of media.Any application can deny this request and cancel the removal.
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002; // A request to remove a device or piece of media has been canceled.
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004; // A device or piece of media has been removed.
        public const int DBT_DEVICEREMOVEPENDING = 0x8003; // A device or piece of media is about to be removed. Cannot be denied.
        public const int DBT_DEVICETYPESPECIFIC = 0x8005; // A device-specific event has occurred.
        public const int DBT_DEVNODES_CHANGED = 0x0007; // A device has been added to or removed from the system.
        public const int DBT_QUERYCHANGECONFIG = 0x0017; // Permission is requested to change the current configuration(dock or undock).
        public const int DBT_USERDEFINED = 0xFFFF; // User-defined


        #region user32_dll
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern bool ReleaseDC(IntPtr hwnd, IntPtr dc);
        #endregion

        #region gdi32_dll
        [DllImport("gdi32.dll")]
        public static extern bool GetDeviceGammaRamp(IntPtr hDC, ref D3DGAMMARAMP lpRamp);

        [DllImport("gdi32.dll")]
        public static extern bool SetDeviceGammaRamp(IntPtr hDC, ref D3DGAMMARAMP lpRamp);

        public const int WINAPI_GAMMARAMP_RESULT_SUCCESS = 1;

        public const uint D3DGAMMARAMP_MAX = 0xFFFF;
        public const uint D3DGAMMARAMP_BASE = 128;
        public const int D3DGAMMARAMP_ARR_LEN = 256;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct D3DGAMMARAMP
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = D3DGAMMARAMP_ARR_LEN)]
            public ushort[] Red;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = D3DGAMMARAMP_ARR_LEN)]
            public ushort[] Green;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = D3DGAMMARAMP_ARR_LEN)]
            public ushort[] Blue;
        }

        public static D3DGAMMARAMP CreateD3DGAMMARAMP()
        {
            return new D3DGAMMARAMP()
            {
                Red = new ushort[D3DGAMMARAMP_ARR_LEN],
                Green = new ushort[D3DGAMMARAMP_ARR_LEN],
                Blue = new ushort[D3DGAMMARAMP_ARR_LEN]
            };
        }
        #endregion

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left, Top, Right, Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szPhysicalMonitorDescription;
        }
    }
}
