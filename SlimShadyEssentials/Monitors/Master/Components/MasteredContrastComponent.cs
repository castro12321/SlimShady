using SlimShadyCore;
using SlimShadyCore.Monitors.Components;
using SlimShadyCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlimShadyEssentials.MonitorManagers.Master.Components
{
    public class MasteredContrastComponent : ContrastMonitorComponent
    {
        private readonly ContrastMonitorComponent master;
        private readonly ContrastMonitorComponent slave;
        private uint currValue;

        public override uint MinValue { get => 0; }
        public override uint MaxValue { get => 100; }
        public override uint CurrValue
        {
            get => currValue;
            set
            {
                using (NkTrace trace = NkLogger.dbgTrace("val: " + value))
                {
                    RefreshSlaveValue(value);
                    currValue = value;
                    OnValueChanged(currValue);
                }
            }
        }

        public MasteredContrastComponent(ContrastMonitorComponent master, ContrastMonitorComponent slave)
        {
            this.master = master;
            this.slave = slave;
            currValue = slave.CurrValue;
        }

        public void RefreshSlaveValue() => RefreshSlaveValue(CurrValue);
        private void RefreshSlaveValue(uint baseValue)
        {
            slave.CurrValue = MasteredComponentUtils.GetSlaveValue(slave.MinValue, slave.MaxValue, master.CurrValue, CurrValue);
        }

        override public void Dispose()
        {
        }
    }
}
