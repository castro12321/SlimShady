using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlimShadyCore.Monitors
{
    public class EmptyMonitor : Monitor
    {
        public EmptyMonitor(string name)
            : base("empty_monitor", name)
        {
            DispNameChanged += (string propertyName, string newValue) =>
            {
                if(DispName != name)
                    DispName = name;
            };
        }

        public override void Dispose()
        {
        }
    }
}
