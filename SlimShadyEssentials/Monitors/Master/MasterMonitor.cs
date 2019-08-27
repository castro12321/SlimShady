using SlimShadyCore.Monitors;
using SlimShadyCore.Monitors.Components;
using SlimShadyCore.Utilities;
using SlimShadyEssentials.MonitorManagers.Master.Components;

namespace SlimShadyEssentials.Monitors.Master
{
    public class MasterMonitor : Monitor
    {
        public static readonly string ID = "Master";

        public MasterMonitor(bool supportsBrightness, bool supportsContrast)
            : base(ID, ID)
        {
            using (NkTrace trace = NkLogger.dbgTrace("ID: " + ID))
            {
                if (supportsBrightness)
                    AddComponent(new MasterBrightnessComponent());
                if (supportsContrast)
                    AddComponent(new MasterContrastComponent());
            }
        }

        public override void Dispose()
        {
        }

    }
}
