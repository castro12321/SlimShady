using SlimShadyCore.Monitors.Components;
using SlimShadyCore.Utilities;
using SlimShadyEssentials.Monitors.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlimShadyEssentials.MonitorManagers.Hardware.Components
{
    public class HardwareContrastComponent : ContrastMonitorComponent
    {
        private readonly PhysicalMonitor physicalMonitor;
        private readonly uint minValue, maxValue;
        private uint currValue;

        public override uint MinValue { get => minValue; }
        public override uint MaxValue { get => maxValue; }
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
                    physicalMonitor.SetContrast(value);
                    currValue = value;
                    OnValueChanged(currValue);
                }
            }
        }

        public HardwareContrastComponent(PhysicalMonitor physicalMonitor, uint minValue, uint maxValue, uint currValue)
        {
            this.physicalMonitor = physicalMonitor;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.currValue = currValue;
        }

        override public void Dispose()
        {
        }
    }
}
