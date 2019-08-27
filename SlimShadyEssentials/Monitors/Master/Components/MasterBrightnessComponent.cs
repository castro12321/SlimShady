using SlimShadyCore.Monitors.Components;
using SlimShadyCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlimShadyEssentials.MonitorManagers.Master.Components
{
    public class MasterBrightnessComponent : BrightnessMonitorComponent
    {
        private uint currValue = 100;

        public override uint MinValue { get => 0; }
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
                    currValue = value;
                    OnValueChanged(currValue);
                }
            }
        }

        override public void Dispose()
        {
        }
    }
}
