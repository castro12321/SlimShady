using SlimShadyCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlimShadyEssentials.MonitorManagers.Master.Components
{
    public static class MasteredComponentUtils
    {
        public static uint GetSlaveValue(uint slaveMin, uint slaveMax, uint masterVal, uint masteredVal)
        {
            uint lerpMin = slaveMin;

            // Usually "lerpMax = masteredVal" assuming that slaveMin/Max = 0/100 and masteredVal = <0; 100>
            uint lerpMax = masteredVal;
            // But it has to be checked against lower/upper bounds so that "slaveMin <= lerpMax <= slaveMax"
            // In case we get a monitor that has different bounds (Like SoftwareMonitor which has MinVal = 20)
            // Otherwise, we would've ignored slave's MinVal and therefore caused chaos and destruction. (Unless these bounds were also checked in slave monitor)
            if (lerpMax < slaveMin)
                lerpMax = slaveMin;
            if (lerpMax > slaveMax)
                lerpMax = slaveMax;

            return Utils.Lerp(lerpMin, lerpMax, masterVal);
        }
    }
}
