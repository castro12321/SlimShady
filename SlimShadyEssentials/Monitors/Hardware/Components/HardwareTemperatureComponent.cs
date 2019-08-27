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
    public class HardwareTemperatureComponent : TemperatureMonitorComponent
    {
        private readonly PhysicalMonitor physicalMonitor;
        private readonly uint minValue, maxValue;
        private readonly List<uint> supportedValues;
        private uint currValue;

        public override uint MinValue { get => minValue; }
        public override uint MaxValue { get => maxValue; }
        public override List<uint> SupportedValues { get => supportedValues; }
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
                    physicalMonitor.SetTemperature(value);
                    currValue = value;
                    OnValueChanged(currValue);
                }
            }
        }

        public HardwareTemperatureComponent(PhysicalMonitor physicalMonitor, uint minValue, uint maxValue, List<uint> supportedValues, uint currValue)
        {
            this.physicalMonitor = physicalMonitor;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.supportedValues = supportedValues;
            this.currValue = currValue;
        }

        override public void Dispose()
        {
        }
    }
}
