using Microsoft.Win32;
using SlimShadyCore;
using SlimShadyCore.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SlimShadyCore.Utilities.Windows
{
    public class WinDisplayDevice
    {
        /* public int cb; Structure size; Not needed for managed code */
        public string DeviceName { get; set; }
        public string DeviceString { get; set; }
        /* public WinDisplayDeviceStateFlags StateFlags; Not implemented yet */
        public string DeviceID { get; set; }
        public string DeviceKey { get; set; }

        public override string ToString()
        {
            return String.Format("devName: {0}, devStr: {1}, devId: {2}, devKey: {3}", DeviceName, DeviceString, DeviceID, DeviceKey);
        }

        public string GetEdidString()
        {
            // HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Enum\DISPLAY\DEL40E7\5&2a8179fe&0&UID4353
            string[] devIdSplit = DeviceID.Split(new char[] { '\\' }, 3);
            string displayKeyPart = devIdSplit[1];
            string driver = devIdSplit[2];
            string displayKeyPath = @"SYSTEM\CurrentControlSet\Enum\DISPLAY\" + displayKeyPart;
            RegistryKey displayKey = Registry.LocalMachine.OpenSubKey(displayKeyPath);

            RegistryKey monitorKey = null;
            foreach (string subKeyName in displayKey.GetSubKeyNames())
            {
                RegistryKey otherMonitorKey = displayKey.OpenSubKey(subKeyName);
                string otherMonitorDriver = (string)otherMonitorKey.GetValue("Driver");
                if(driver == otherMonitorDriver)
                {
                    monitorKey = otherMonitorKey;
                    break;
                }
            }
            if (monitorKey == null)
                throw new Exception("monitor key not found for disp device: " + this);

            RegistryKey deviceParamKey = monitorKey.OpenSubKey("Device Parameters");
            byte[] edidBA = (byte[])deviceParamKey.GetValue("EDID");
            string edid = Utils.ByteArrayToHexString(edidBA);
            return edid;
        }
    }

}