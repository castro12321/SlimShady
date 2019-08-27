using SlimShadyCore.Utilities.Windows.Natives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SlimShadyCore.Utilities.Windows
{
    public partial class Win
    {

        private static WinDisplayDevice EnumDisplayDevices(string lpDevice, uint iDevNum, uint dwFlags)
        {
            WinN.DISPLAY_DEVICE windd = new WinN.DISPLAY_DEVICE();
            windd.cb = Marshal.SizeOf(windd);
            if (!WinN.EnumDisplayDevices(lpDevice, iDevNum, ref windd, 0))
                return null;
            return new WinDisplayDevice()
            {
                DeviceName = windd.DeviceName,
                DeviceString = windd.DeviceString,
                DeviceID = windd.DeviceID,
                DeviceKey = windd.DeviceKey,
            };
        }

        public static List<WinDisplayDevice> EnumDisplayDevices()
        {
            using (NkTrace trace = NkLogger.dbgTrace())
            {
                List<WinDisplayDevice> displayDevices = new List<WinDisplayDevice>();

                for (uint outerId = 0; ; outerId++)
                {
                    WinDisplayDevice outerDD = EnumDisplayDevices(null, outerId, 0);
                    if (outerDD == null)
                        break;
                    NkLogger.dbg("id: " + outerId + ", outerDD: " + outerDD);

                    for (uint innerId = 0; ; innerId++)
                    {
                        WinDisplayDevice innerDD = EnumDisplayDevices(outerDD.DeviceName, innerId, 0);
                        if (innerDD == null)
                            break;
                        NkLogger.dbg(" --> id: " + innerId + ", innerDD: " + innerDD);
                        displayDevices.Add(innerDD);
                    }
                }

                return displayDevices;
            }
        }
    }
}
