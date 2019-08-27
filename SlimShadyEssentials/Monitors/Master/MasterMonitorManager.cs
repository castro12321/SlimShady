using SlimShadyCore.Monitors;
using SlimShadyCore.Monitors.Components;
using SlimShadyCore.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace SlimShadyEssentials.Monitors.Master
{
    public class MasterMonitorManager : MonitorManager
    {
        private readonly MonitorManager slaveManager;

        public MasterMonitorManager(string id, MonitorManager slaveManager)
            : base(id, slaveManager.DispName)
        {
            this.slaveManager = slaveManager;
            slaveManager.Monitors.CollectionChanged += (sender, ev) => base.RefreshMonitors();

            base.RefreshMonitors();
        }

        public override void RefreshMonitors()
        {
            // Call slave refresh only.
            // Then slave's monitor collection changed event 
            // causes actual Refresh in master manager.
            slaveManager.RefreshMonitors();
        }

        protected override List<Monitor> ReadMonitors()
        {
            using (NkTrace trace = NkLogger.dbgTrace("id: " + Id + ", name: " + DispName))
            {
                List<Monitor> ret = new List<Monitor>();

                ICollection<Monitor> slaveMonitors = slaveManager.Monitors;

                bool masterSupportsBrightness = slaveMonitors.All(slave => slave.HasComponent<BrightnessMonitorComponent>());
                bool masterSupportsContrast = slaveMonitors.All(slave => slave.HasComponent<ContrastMonitorComponent>());
                MasterMonitor masterMonitor = new MasterMonitor(masterSupportsBrightness, masterSupportsContrast);

                List<MasteredMonitor> masteredMonitors = new List<MasteredMonitor>(
                    slaveMonitors.Select(slaveMonitor => new MasteredMonitor(masterMonitor, slaveMonitor))
                );

                if (masteredMonitors.Count > 0)
                {
                    if(masterMonitor.GetComponents().Count() > 0)
                        ret.Add(masterMonitor);
                    foreach (var mm in masteredMonitors)
                    {
                        if(mm.GetComponents().Count() > 0)
                            ret.Add(mm);
                    }
                }

                return ret;
            }
        }

        public override void Dispose()
        {
            // Don't call base.Dispose
            // Calling it would cause double disposal of slave monitors
            // as base.Dispose() calls Dispose for each Monitor, which calls slaveMonitor.Dispose()
            // but we also need to call slaveManager.Dispose() which disposes all slaveManager.Monitors
            slaveManager.Dispose();
            Monitors.Clear();
        }
    }
}
