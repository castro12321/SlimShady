using System.Collections.Generic;

namespace SlimShady.MonitorManagers
{
    public class GammaRampMonitorManager : MonitorManager
    {
        public static GammaRampMonitorManager Instance = new GammaRampMonitorManager();
        
        private readonly List<Monitor> monitors = new List<Monitor>();

        private GammaRampMonitorManager()
        {
            MainWindow.Trace("Loading Gamma ramp Manager");

            monitors.Add(new GammaRampMonitor("All in one monitor", this)); // We don't support multiple monitors
            //foreach (Screen screen in Screen.AllScreens)
              //  monitors.Add(new GammaRampMonitor(screen, this));
        }

        public override List<Monitor> GetMonitorsList()
        {
            return monitors;
        }
    }
}
