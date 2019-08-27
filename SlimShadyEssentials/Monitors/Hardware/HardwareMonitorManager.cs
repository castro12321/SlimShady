using SlimShadyCore.Monitors;
using SlimShadyCore.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace SlimShadyEssentials.Monitors.Hardware
{
    public class HardwareMonitorManager : MonitorManager
    {
        public HardwareMonitorManager(string id, string name)
            : base(id, name)
        {
            RefreshMonitors();
        }

        protected override List<Monitor> ReadMonitors()
        {
            using (NkTrace trace = NkLogger.dbgTrace("id: " + Id + ", name: " + DispName))
            {
                return new List<Monitor>(
                    PhysicalMonitorManager.ReadPhysicalMonitors()
                        .Select(phm => new HardwareMonitor(phm))
                        .Where(hwm => hwm.GetComponents().Count() > 0)
                );
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
