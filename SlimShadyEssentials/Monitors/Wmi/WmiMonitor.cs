using SlimShadyCore.Monitors.Components;
using System;
using System.Management;
using System.Linq;
using SlimShadyCore.Monitors;
using SlimShadyCore.Utilities;
using SlimShadyEssentials.MonitorManagers.Wmi.Components;

namespace SlimShadyEssentials.Monitors.Wmi
{
    // Tip: "WMI code creator" is really useful tool

    public class WmiMonitor : Monitor
    {
        

        private readonly ManagementObject monitor;

        private static string GetMonitorName(ManagementObject monitor)
        {
            return monitor.Properties["InstanceName"].Value.ToString();
        }

        public WmiMonitor(ManagementObject monitor)
            : base(GetMonitorName(monitor), GetMonitorName(monitor))
        {
            using (NkTrace trace = NkLogger.trace("monitor: " + monitor))
            {
                this.monitor = monitor;

                try
                {
                    ManagementScope scope = new ManagementScope("root\\WMI");
                    String sQuery = "SELECT * FROM WmiMonitorBrightness WHERE InstanceName='" + Id.Replace("\\", "\\\\") + "'";
                    SelectQuery query = new SelectQuery(sQuery);
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                    using (var items = searcher.Get())
                    {
                        var item = items.Cast<ManagementBaseObject>().SingleOrDefault();
                        uint mBrightness = (byte)item.Properties["CurrentBrightness"].Value;
                        AddComponent(new WmiBrightnessComponent(monitor, mBrightness));
                    }
                }
                catch (ManagementException e)
                {
                    NkLogger.error("WMI monitor cannot read current Brightness: " + e.Message);
                }
            }
        }

        public override void Dispose()
        {
            monitor.Dispose();
        }
    }
}
