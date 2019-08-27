using SlimShadyCore.Monitors;
using SlimShadyCore.Utilities;
using SlimShadyCore.Utilities.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

namespace SlimShadyEssentials.Monitors.Gamma
{
    public class GammaRampMonitorManager : MonitorManager
    {
        public GammaRampMonitorManager(string id, string name)
            : base(id, name)
        {
            RefreshMonitors();
        }

        protected override List<Monitor> ReadMonitors()
        {
            using (NkTrace trace = NkLogger.dbgTrace("id: " + Id + ", name: " + DispName))
            {
                List<Monitor> ret = new List<Monitor>();

                // We're retrieving device context for the entire screen.
                // TODO: We could retrieve context for specific screen though
                IntPtr hWnd = IntPtr.Zero;
                IntPtr dc = Win.GetDC(hWnd);
                if (dc == null || dc == IntPtr.Zero)
                    throw new Exception("Failed to get gamma-ramp DC");

                var monitor = new GammaRampMonitor(hWnd, dc, "Gamma");
                if (monitor.GetComponents().Count() > 0)
                    ret.Add(monitor);

                //foreach (Screen screen in Screen.AllScreens)
                //  monitors.Add(new GammaRampMonitor(screen, this));

                return ret;
            }
        }
    }
}
