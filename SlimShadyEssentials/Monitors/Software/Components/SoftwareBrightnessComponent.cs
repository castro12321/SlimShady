using SlimShadyCore.Monitors.Components;
using SlimShadyCore.Utilities;
using SlimShadyEssentials.Monitors.Software;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlimShadyEssentials.MonitorManagers.Software.Components
{
    public class SoftwareBrightnessComponent : BrightnessMonitorComponent
    {
        private readonly SoftwareMonitorWindowOverlay overlay;
        private uint currValue;

        public override uint MinValue { get => 20; }
        public override uint MaxValue { get => 100; }
        public override uint CurrValue
        {
            get => currValue;
            set
            {
                if (currValue == value)
                {
                    NkLogger.dbg("Change not needed, already at: " + value);
                    return;
                }
                
                using (NkTrace trace = NkLogger.trace("val: " + value))
                {
                    
                    uint opacity = 100 - value;
                    overlay.Opacity = (float)opacity / 100.0;
                    currValue = value;
                    OnValueChanged(currValue);
                }
            }
        }

        public SoftwareBrightnessComponent(SoftwareMonitorWindowOverlay overlay, uint currValue)
        {
            this.overlay = overlay;
            this.currValue = currValue;
        }

        override public void Dispose()
        {
        }
    }
}
