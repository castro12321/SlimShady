using System.Collections.Generic;
using System.Management;

namespace SlimShady.MonitorManagers
{
    // Tip: "WMI code creator" is really useful tool

    public class WmiMonitorManager : MonitorManager
    {
        public static WmiMonitorManager Instance = new WmiMonitorManager();
        private readonly List<Monitor> monitors = new List<Monitor>();

        private WmiMonitorManager()
        {
            MainWindow.Trace("Loading WMI Manager");

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
                    monitors.Add(new WmiMonitor(mObj, this));
                }
            }
            catch (ManagementException e)
            {
                MainWindow.Error("WMI monitor Manager cannot list monitors: " + e.Message);
            }
        }

        public override List<Monitor> GetMonitorsList()
        {
            return monitors;
        }
    }
}
