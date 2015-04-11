using System.Collections.Generic;
using System.Windows.Forms;

namespace SlimShady.MonitorManagers
{
    public class SoftwareMonitorManager : MonitorManager
    {
        public static SoftwareMonitorManager Instance = new SoftwareMonitorManager();
        
        private readonly List<Monitor> monitors = new List<Monitor>();

        private SoftwareMonitorManager()
        {
            MainWindow.Trace("Loading Software Manager");

            foreach (Screen screen in Screen.AllScreens)
                monitors.Add(new SoftwareMonitor(screen, this));
        }

        public override List<Monitor> GetMonitorsList()
        {
            return monitors;
        }
    }
}
