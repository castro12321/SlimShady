using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace SlimShady.MonitorManagers
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class HwmMgrWinApi
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left, top, right, bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szPhysicalMonitorDescription;
        }

        public delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr unusedHdcMonitor, ref Rect unusedLprcMonitor, int unusedDwData);
        [DllImport("user32")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lpRect, MonitorEnumProc callback, int dwData);

        [DllImport("dxva2.dll", SetLastError = true)]
        public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint numberOfPhysicalMonitors);

        [DllImport("dxva2.dll", SetLastError = true)]
        public static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);
    }

    public class HardwareMonitorManager : MonitorManager
    {
        public static HardwareMonitorManager Instance = new HardwareMonitorManager();
        private readonly List<Monitor> monitors = new List<Monitor>();

        private HardwareMonitorManager()
        {
            MainWindow.Trace("Loading DDC/CI Manager");

            HwmMgrWinApi.MonitorEnumProc callback = (IntPtr hMonitor, IntPtr unusedHdcMonitor, ref HwmMgrWinApi.Rect unusedLprcMonitor, int unusedDwData) =>
            {
                uint numberOfPhysicalMonitors = 0u;
                bool resultGetNumberOfPhysicalMonitorsFromHMONITOR = HwmMgrWinApi.GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref numberOfPhysicalMonitors);
                if (!resultGetNumberOfPhysicalMonitorsFromHMONITOR)
                {
                    MainWindow.Error("GetNumberOfPhysicalMonitorsFromHMONITOR failed; Error code: " + Marshal.GetLastWin32Error());
                    return true;
                }

                HwmMgrWinApi.PHYSICAL_MONITOR[] physicalMonitors = new HwmMgrWinApi.PHYSICAL_MONITOR[numberOfPhysicalMonitors];
                if (!HwmMgrWinApi.GetPhysicalMonitorsFromHMONITOR(hMonitor, numberOfPhysicalMonitors, physicalMonitors))
                {
                    MainWindow.Error("GetPhysicalMonitorsFromHMONITOR failed; Error code: " + Marshal.GetLastWin32Error());
                    return true;
                }

                foreach (HwmMgrWinApi.PHYSICAL_MONITOR physicalMonitor in physicalMonitors)
                {
                    monitors.Add(new HardwareMonitor(physicalMonitor.hPhysicalMonitor, physicalMonitor.szPhysicalMonitorDescription, this));
                }
                
                return true;
            };

            if (!HwmMgrWinApi.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, 0))
                MainWindow.Error("An Error occured while enumerating monitors");
        }

        public override List<Monitor> GetMonitorsList()
        {
            return monitors;
        }
    }
}
