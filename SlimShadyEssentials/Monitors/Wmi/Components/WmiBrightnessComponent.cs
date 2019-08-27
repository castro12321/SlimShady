using SlimShadyCore.Monitors.Components;
using SlimShadyCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace SlimShadyEssentials.MonitorManagers.Wmi.Components
{
    public class WmiBrightnessComponent : BrightnessMonitorComponent
    {
        private readonly ManagementObject monitor;
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
                    const int timeoutInSeconds = 5;
                    monitor.InvokeMethod("WmiSetBrightness", new Object[] { timeoutInSeconds, (byte)value });
                    currValue = value;
                    OnValueChanged(currValue);
                }
            }
        }

        public WmiBrightnessComponent(ManagementObject monitor, uint currValue)
        {
            this.monitor = monitor;
            this.currValue = currValue;
        }

        override public void Dispose()
        {
        }
    }
}
