using SlimShadyCore.Utilities;
using SlimShadyCore.Monitors.Components;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using SlimShadyCore.Monitors;
using SlimShadyEssentials.MonitorManagers.Gamma.Components;
using SlimShadyEssentials.MonitorManagers.Gamma;
using SlimShadyCore.Utilities.Windows;

namespace SlimShadyEssentials.Monitors.Gamma
{
    public class GammaRampMonitor : Monitor
    {
        private readonly IntPtr hWnd;
        private readonly IntPtr hDC;
        
        public GammaRampMonitor(IntPtr hWnd, IntPtr hDC, string name)
            : base(name, name)
        {
            using (NkTrace trace = NkLogger.trace("name: " + name))
            {
                this.hWnd = hWnd;
                this.hDC = hDC;

                try
                {
                    Win.D3DGAMMARAMP gammaRamp = FromDevice(hDC);
                    AddComponent(new GammaBrightnessComponent(hDC, gammaRamp));
                }
                catch (NotSupportedException)
                {
                    NkLogger.info("GammaRamp is not supported on this system");
                }

            }
        }

        public override void Dispose()
        {
            if (!Win.ReleaseDC(hWnd, hDC))
            {
                NkLogger.error("GammaRamp ReleaseDC failed; hwnd: " + hWnd + ", hdc: " + hDC);
                //throw new Exception("GammaRamp ReleaseDC failed; hwnd: " + hWnd + ", hdc: " + hDC);
            }
                
        }

        public static Win.D3DGAMMARAMP FromDevice(IntPtr hDC)
        {
            Win.D3DGAMMARAMP ramp = Win.CreateD3DGAMMARAMP();
            if (!Win.GetDeviceGammaRamp(hDC, ref ramp))
                throw new NotSupportedException("GammaRamp is not supported on this device");
            return ramp;
        }
    }
}

