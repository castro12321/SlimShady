using SlimShadyCore;
using SlimShadyCore.Monitors;
using SlimShadyCore.Monitors.Components;
using SlimShadyCore.Utilities;
using SlimShadyEssentials.MonitorManagers.Master.Components;
using System;

namespace SlimShadyEssentials.Monitors.Master
{
    public class MasteredMonitor : Monitor
    {
        private Monitor slave;

        public MasteredMonitor(MasterMonitor master, Monitor slave)
            : base(slave.Id, slave.DispName)
        {
            using (NkTrace trace = NkLogger.dbgTrace("slave: " + slave))
            {
                this.slave = slave;

                foreach(MonitorComponent slaveRawComponent in slave.GetComponents())
                {
                    if (slaveRawComponent is BrightnessMonitorComponent)
                    {
                        BrightnessMonitorComponent masterComponent = master.GetSingleComponent<BrightnessMonitorComponent>();
                        BrightnessMonitorComponent slaveComponent = (BrightnessMonitorComponent)slaveRawComponent;
                        MasteredBrightnessComponent component = new MasteredBrightnessComponent(masterComponent, slaveComponent);
                        masterComponent.ValueChanged += (property, newValue) => component.RefreshSlaveValue();
                        AddComponent(component);
                    }
                    else if (slaveRawComponent is ContrastMonitorComponent)
                    {
                        ContrastMonitorComponent masterComponent = master.GetSingleComponent<ContrastMonitorComponent>();
                        ContrastMonitorComponent slaveComponent = (ContrastMonitorComponent)slaveRawComponent;
                        MasteredContrastComponent component = new MasteredContrastComponent(masterComponent, slaveComponent);
                        masterComponent.ValueChanged += (property, newValue) => component.RefreshSlaveValue();
                        AddComponent(component);
                    }
                    else
                    {
                        AddComponent(slaveRawComponent);
                    }
                }
            }
        }

        public override void Dispose()
        {
            slave.Dispose();
        }
    }
}
