using SlimShadyCore.Monitors;
using SlimShadyCore.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace SlimShadyEssentials.Monitors.Wmi
{
    // Tip: "WMI code creator" is really useful tool

    public class WmiMonitorManager : MonitorManager
    {
        public WmiMonitorManager(string id, string name)
            : base(id, name)
        {
            RefreshMonitors();
        }

        protected override List<Monitor> ReadMonitors()
        {
            List<Monitor> newMonitors = new List<Monitor>();

            using (NkTrace trace = NkLogger.dbgTrace("id: " + Id + ", name: " + DispName))
            {

                try
                {
                    ManagementScope scope = new ManagementScope("root\\WMI");
                    //SelectQuery query = new SelectQuery("SELECT * FROM WmiMonitorBrightness");
                    //SelectQuery query = new SelectQuery("WmiMonitorBrightness");
                    SelectQuery query = new SelectQuery("WmiMonitorBrightnessMethods");
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                    // Need testers with laptops with 2 screens lol
                    foreach (var o in searcher.Get())
                    {
                        ManagementObject mObj = (ManagementObject)o;
                        var wmiMonitor = new WmiMonitor(mObj);
                        if(wmiMonitor.GetComponents().Count() > 0)
                            newMonitors.Add(wmiMonitor);
                    }
                }
                catch (ManagementException e)
                {
                    NkLogger.error("WMI monitor Manager cannot list monitors: " + e.Message);
                }
            }

            return newMonitors;
        }
    }
}
