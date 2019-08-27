using SlimShadyCore.Monitors;
using SlimShadyCore.Monitors.Components;
using SlimShadyCore.Utilities;
using SlimShadyEssentials.MonitorManagers.Software.Components;
using System.Windows.Forms;

namespace SlimShadyEssentials.Monitors.Software
{
    public class SoftwareMonitor : Monitor
    {
        

        private readonly SoftwareMonitorWindowOverlay overlay;

        public SoftwareMonitor(Screen screen)
            : base(screen.DeviceName, screen.DeviceName)
        {
            using (NkTrace trace = NkLogger.trace("screen: " + screen))
            {
                // Initialize Brightness management
                overlay = new SoftwareMonitorWindowOverlay(screen);
                AddComponent(new SoftwareBrightnessComponent(overlay, 100));
            }
        }

        public override void Dispose()
        {
            overlay.Close();
        }

    }
}
