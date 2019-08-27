using SlimShadyCore.Utilities;
using SlimShadyCore.Utilities.Windows;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SlimShadyEssentials.Monitors.Hardware
{
    public static class PhysicalMonitorManager
    {
        private static class Natives
        {
            public delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr unusedHdcMonitor, ref Win.Rect unusedLprcMonitor, int unusedDwData);

            [DllImport("user32")]
            public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lpRect, MonitorEnumProc callback, int dwData);

            [DllImport("dxva2.dll", SetLastError = true)]
            public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint numberOfPhysicalMonitors);

            [DllImport("dxva2.dll", SetLastError = true)]
            public static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] Win.PHYSICAL_MONITOR[] pPhysicalMonitorArray);

            // size of a device name string
            private const int CCHDEVICENAME = 32;

            /// <summary>
            /// The MONITORINFOEX structure contains information about a display monitor.
            /// The GetMonitorInfo function stores information into a MONITORINFOEX structure or a MONITORINFO structure.
            /// The MONITORINFOEX structure is a superset of the MONITORINFO structure. The MONITORINFOEX structure adds a string member to contain a name
            /// for the display monitor.
            /// </summary>
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            internal struct MonitorInfoEx
            {
                /// <summary>
                /// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFOEX) (72) before calling the GetMonitorInfo function.
                /// Doing so lets the function determine the type of structure you are passing to it.
                /// </summary>
                public int Size;

                /// <summary>
                /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
                /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
                /// </summary>
                public Win.Rect Monitor;

                /// <summary>
                /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications,
                /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor.
                /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars.
                /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
                /// </summary>
                public Win.Rect WorkArea;

                /// <summary>
                /// The attributes of the display monitor.
                ///
                /// This member can be the following value:
                ///   1 : MONITORINFOF_PRIMARY
                /// </summary>
                public uint Flags;

                /// <summary>
                /// A string that specifies the device name of the monitor being used. Most applications have no use for a display monitor name,
                /// and so can save some bytes by using a MONITORINFO structure.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
                public string DeviceName;

                public void Init()
                {
                    this.Size = 40 + 2 * CCHDEVICENAME;
                    this.DeviceName = string.Empty;
                }
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

            public static string GetMonitorName(IntPtr hMonitor)
            {
                MonitorInfoEx monitorInfo = new MonitorInfoEx();
                monitorInfo.Size = (int)Marshal.SizeOf(monitorInfo);
                if (!GetMonitorInfo(hMonitor, ref monitorInfo))
                    throw new Exception("GetMonitorInfo failed");
                return monitorInfo.DeviceName;
            }
        }

        public static List<PhysicalMonitor> ReadPhysicalMonitors()
        {
            using (NkTrace trace = NkLogger.dbgTrace())
            {
                List<PhysicalMonitor> monitors = new List<PhysicalMonitor>();
                List<WinDisplayDevice> displayDevices = Win.EnumDisplayDevices();

                Natives.MonitorEnumProc callback = (IntPtr hMonitor, IntPtr unusedHdcMonitor, ref Win.Rect unusedLprcMonitor, int unusedDwData) =>
                {
                    try
                    {
                        NkLogger.dbg("--> hwMonitor callback: " + hMonitor);
                        monitors.AddRange(ReadPhysicalMonitorsFromHMonitor(hMonitor, displayDevices));
                    }
                    catch (Exception ex)
                    {
                        NkLogger.error("Failed to initialize monitor; ex: " + ex);
                    // Continue anyway. Maybe other monitors will load fine
                }

                    return true;
                };

                if (!Natives.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, 0))
                    throw new Exception("EnumDisplayMonitors() failed");

                return monitors;
            }
        }

        private static List<PhysicalMonitor> ReadPhysicalMonitorsFromHMonitor(IntPtr hMonitor, List<WinDisplayDevice> displayDevices)
        {
            using (NkTrace trace = NkLogger.dbgTrace(hMonitor))
            {
                List<PhysicalMonitor> physicalMonitors = new List<PhysicalMonitor>();

                uint numberOfPhysicalMonitors = 0u;
                if (!Natives.GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref numberOfPhysicalMonitors))
                    throw new SlimWin32Exception("GetNumberOfPhysicalMonitorsFromHMONITOR() failed");

                Win.PHYSICAL_MONITOR[] win32PhysicalMonitors = new Win.PHYSICAL_MONITOR[numberOfPhysicalMonitors];
                if (!Natives.GetPhysicalMonitorsFromHMONITOR(hMonitor, numberOfPhysicalMonitors, win32PhysicalMonitors))
                    throw new SlimWin32Exception("GetPhysicalMonitorsFromHMONITOR failed");

                string hMonitorName = Natives.GetMonitorName(hMonitor);
                NkLogger.dbg("Monitor name: " + hMonitorName);

                foreach (Win.PHYSICAL_MONITOR physicalMonitorW32 in win32PhysicalMonitors)
                {
                    WinDisplayDevice dispDD = MatchMonitorToDD(physicalMonitorW32, hMonitorName, displayDevices);
                    PhysicalMonitor physicalMonitor = new PhysicalMonitor(physicalMonitorW32.hPhysicalMonitor, physicalMonitorW32.szPhysicalMonitorDescription, hMonitorName, dispDD);
                    physicalMonitors.Add(physicalMonitor);
                }

                return physicalMonitors;
            }
        }

        private static WinDisplayDevice MatchMonitorToDD(Win.PHYSICAL_MONITOR physicalMonitorW32, string hMonitorName, List<WinDisplayDevice> displayDevices)
        {
            string phDesc = physicalMonitorW32.szPhysicalMonitorDescription;
            foreach (WinDisplayDevice dd in displayDevices)
            {
                // Note: Might not work for setups with 11+ monitors ("DISPLAY1" might get wrongly matched with "DISPLAY11" because of str.Contains() call)
                // But for now I don't know anyone using such enormous number of displays. Anyway, our UI would probably struggle too. 
                // Might fix this in some future version.
                if (phDesc.Contains(dd.DeviceName) || dd.DeviceName.Contains(phDesc)
                    || hMonitorName.Contains(dd.DeviceName) || dd.DeviceName.Contains(hMonitorName))
                    return dd;
            }
            throw new Exception("Can't find DisplayDevice for physical monitor: " + phDesc + ", " + hMonitorName + "; dispDevs: " + string.Join(", ", displayDevices));
        }
    }
}
