using SlimShadyCore.Monitors;
using SlimShadyCore.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SlimShadyEssentials.Monitors.Software
{
    public class SoftwareMonitorManager : MonitorManager
    {
        public SoftwareMonitorManager(string id, string name)
            : base(id, name)
        {
            RefreshMonitors();
        }

        protected override List<Monitor> ReadMonitors()
        {
            using (NkTrace trace = NkLogger.dbgTrace("id: " + Id + ", name: " + DispName))
            {
                return new List<Monitor>(
                    Screen.AllScreens
                    .Select(screen => new SoftwareMonitor(screen))
                );
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
