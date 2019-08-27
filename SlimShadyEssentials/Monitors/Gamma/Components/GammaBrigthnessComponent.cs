using SlimShadyCore.Monitors.Components;
using SlimShadyCore.Utilities;
using SlimShadyCore.Utilities.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlimShadyEssentials.MonitorManagers.Gamma.Components
{
    public class GammaBrightnessComponent : BrightnessMonitorComponent
    {
        private readonly IntPtr dc;
        private Win.D3DGAMMARAMP ramp;

        public override uint MinValue { get => 1; }
        public override uint MaxValue { get => 256; }
        public override uint CurrValue
        {
            get => GetRampBrightness(ramp);
            set
            {
                if (CurrValue == value)
                {
                    NkLogger.dbg("Change not needed, already at: " + value);
                    return;
                }

                using (NkTrace trace = NkLogger.trace("val: " + value))
                {
                    SetRampBrightness(ref ramp, value);
                    if (!Win.SetDeviceGammaRamp(dc, ref ramp))
                        throw new Exception("Setting gamma-ramp failed");
                    OnValueChanged(value);
                }
            }
        }

        public GammaBrightnessComponent(IntPtr dc, Win.D3DGAMMARAMP currGammaRamp)
        {
            this.dc = dc;
            this.ramp = currGammaRamp;
        }

        private static uint GetRampBrightness(Win.D3DGAMMARAMP ramp)
        {
            return ramp.Red[1] - Win.D3DGAMMARAMP_BASE;
        }

        private static void SetRampBrightness(ref Win.D3DGAMMARAMP ramp, uint value)
        {
            for (int i = 1; i < Win.D3DGAMMARAMP_ARR_LEN; i++)
            {
                var val = (ushort)Math.Min(i * (value + Win.D3DGAMMARAMP_BASE), Win.D3DGAMMARAMP_MAX);
                ramp.Red[i] = ramp.Blue[i] = ramp.Green[i] = val;
            }
        }

        public override void Dispose()
        {
        }
    }
}
