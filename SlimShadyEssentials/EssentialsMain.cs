using SlimShadyEssentials.Monitors.Hardware;
using SlimShadyEssentials.Monitors.Software;
using SlimShadyEssentials.Monitors.Wmi;
using SlimShadyCore.Monitors;
using SlimShadyCore;
using SlimShadyEssentials.Monitors.Gamma;
using SlimShadyEssentials.Monitors.Master;

namespace SlimShadyEssentials
{
    public class EssentialsMain
    {
        private class HardwareMonitorManagerProvider : MonitorManagerProvider
        {
            public override string Id => "HW";
            public override string DispName => "Hardware";
            protected override MonitorManager Create() => new HardwareMonitorManager(Id, DispName);
        }

        private class GammaMonitorManagerProvider : MonitorManagerProvider
        {
            public override string Id => "GR";
            public override string DispName => "GammaRamp";
            protected override MonitorManager Create() => new GammaRampMonitorManager(Id, DispName);
        }

        private class WmiMonitorManagerProvider : MonitorManagerProvider
        {
            public override string Id => "WMI";
            public override string DispName => "WMI";
            protected override MonitorManager Create() => new WmiMonitorManager(Id, DispName);
        }

        private class SoftwareMonitorManagerProvider : MonitorManagerProvider
        {
            public override string Id => "SW";
            public override string DispName => "Software";
            protected override MonitorManager Create() => new SoftwareMonitorManager(Id, DispName);
        }

        private class MasterMonitorManagerProvider : MonitorManagerProvider
        {
            private readonly MonitorManagerProvider slaveProvider;
            public override string Id => "master_" + slaveProvider.Id;
            public override string DispName => slaveProvider.DispName;
            protected override MonitorManager Create() => new MasterMonitorManager(Id, slaveProvider.GetOrCreate());

            public MasterMonitorManagerProvider(MonitorManagerProvider slaveProvider)
            {
                this.slaveProvider = slaveProvider;
            }
        }

        private static void AddSupportedMasterMonitorManager(SlimCore app, MonitorManagerProvider slaveProvider)
        {
            app.AddSupportedMonitorManager(new MasterMonitorManagerProvider(slaveProvider));
        }

        public static void Initialize(SlimCore app)
        {
            AddSupportedMasterMonitorManager(app, new HardwareMonitorManagerProvider());
            AddSupportedMasterMonitorManager(app, new GammaMonitorManagerProvider());
            AddSupportedMasterMonitorManager(app, new WmiMonitorManagerProvider());
            AddSupportedMasterMonitorManager(app, new SoftwareMonitorManagerProvider());
        }
    }
}
