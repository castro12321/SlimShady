using SlimShadyCore.Utilities;
using SlimShadyCore.Utilities.Windows;
using SlimShadyEssentials.Monitors.Hardware.vcp;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SlimShadyEssentials.Monitors.Hardware
{
    /// <summary>
    /// Thin wrapper on methods accepting PhysicalMonitor handle
    /// </summary>
    public class PhysicalMonitor
    {
        private static class Natives
        {
            #region Capabilities
            // monitor capabilities returned by GetMonitorCapabilities
            public const uint MC_CAPS_NONE = 0x00000000;
            public const uint MC_CAPS_BRIGHTNESS = 0x00000002;
            public const uint MC_CAPS_CONTRAST = 0x00000004;
            public const uint MC_CAPS_COLOR_TEMPERATURE = 0x00000008;
            //private const uint MC_CAPS_RESTORE_FACTORY_DEFAULTS = 0x00000400;
            //private const uint MC_CAPS_RESTORE_FACTORY_COLOR_DEFAULTS = 0x00000800;
            //private const uint MC_RESTORE_FACTORY_DEFAULTS_ENABLES_MONITOR_SETTINGS = 0x00001000;

            [DllImport("dxva2.dll", SetLastError = true)]
            public static extern bool GetCapabilitiesStringLength(IntPtr hMonitor, ref uint capabilitiesStringLengthInCharacters);

            [DllImport("dxva2.dll", SetLastError = true)]
            public static extern bool CapabilitiesRequestAndCapabilitiesReply(IntPtr hMonitor, StringBuilder ASCIICapabilitiesString, uint capabilitiesStringLengthInCharacters);

            [DllImport("dxva2.dll", SetLastError = true)]
            public static extern bool GetMonitorCapabilities(IntPtr hMonitor, ref uint monitorCapabilities, ref uint supportedColorTemperatures);
            #endregion

            #region Brightness_Contrast_Temperature
            public enum MC_COLOR_TEMPERATURE
            {
                MC_COLOR_TEMPERATURE_UNKNOWN,
                MC_COLOR_TEMPERATURE_4000K,
                MC_COLOR_TEMPERATURE_5000K,
                MC_COLOR_TEMPERATURE_6500K,
                MC_COLOR_TEMPERATURE_7500K,
                MC_COLOR_TEMPERATURE_8200K,
                MC_COLOR_TEMPERATURE_9300K,
                MC_COLOR_TEMPERATURE_10000K,
                MC_COLOR_TEMPERATURE_11500K
            };

            public static MC_COLOR_TEMPERATURE MC_COLOR_TEMPERATURE_FromUint(uint val)
            {
                switch (val)
                {
                    case 4000: return MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_4000K;
                    case 5000: return MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_5000K;
                    case 6500: return MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_6500K;
                    case 7500: return MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_7500K;
                    case 8200: return MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_8200K;
                    case 9300: return MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_9300K;
                    case 10000: return MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_10000K;
                    case 11500: return MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_11500K;
                }
                throw new Exception("Invalid value for MC_COLOR_TEMPERATURE: " + val);
            }

            public static uint MC_COLOR_TEMPERATURE_ToUint(MC_COLOR_TEMPERATURE val)
            {
                switch (val)
                {
                    case MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_4000K: return 4000;
                    case MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_5000K: return 5000;
                    case MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_6500K: return 6500;
                    case MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_7500K: return 7500;
                    case MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_8200K: return 8200;
                    case MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_9300K: return 9300;
                    case MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_10000K: return 10000;
                    case MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_11500K: return 11500;
                }
                throw new Exception("Invalid value for MC_COLOR_TEMPERATURE: " + val);
            }

            [DllImport("dxva2.dll", SetLastError = true)]
            public static extern bool GetMonitorBrightness(IntPtr hMonitor, ref uint minimumBrightness, ref uint currentBrightness, ref uint maximumBrightness);

            [DllImport("dxva2.dll", SetLastError = true)]
            public static extern bool SetMonitorBrightness(IntPtr hMonitor, uint newBrightness);

            [DllImport("dxva2.dll", SetLastError = true)]
            public static extern bool GetMonitorContrast(IntPtr hMonitor, ref uint minimumContrast, ref uint currentContrast, ref uint maximumContrast);

            [DllImport("dxva2.dll", SetLastError = true)]
            public static extern bool SetMonitorContrast(IntPtr hMonitor, uint newContrast);

            [DllImport("dxva2.dll", SetLastError = true)]
            public static extern bool GetMonitorColorTemperature(IntPtr hMonitor, ref MC_COLOR_TEMPERATURE currentColorTemperature);

            [DllImport("dxva2.dll", SetLastError = true)]
            public static extern bool SetMonitorColorTemperature(IntPtr hMonitor, MC_COLOR_TEMPERATURE newColorTemperature);
            #endregion

        }

        public class Brightness
        {
            public readonly uint brightness, minBrightness, maxBrightness;

            public Brightness(uint brightness, uint minBrightness, uint maxBrightness)
            {
                this.brightness = brightness;
                this.minBrightness = minBrightness;
                this.maxBrightness = maxBrightness;
            }

            public override string ToString() => "Min: " + minBrightness + ", Max: " + maxBrightness + ", Current: " + brightness;
        }

        public class Contrast
        {
            public readonly uint contrast, minContrast, maxContrast;

            public Contrast(uint contrast, uint minContrast, uint maxContrast)
            {
                this.contrast = contrast;
                this.minContrast = minContrast;
                this.maxContrast = maxContrast;
            }

            public override string ToString() => "Min: " + minContrast + ", Max: " + maxContrast + ", Current: " + contrast;
        }



        private IntPtr handle;
        private string description;
        private string baseName;
        public MonitorCapabilities VcpCapabilities { get; private set; }
        private Edid edid;

        public PhysicalMonitor(IntPtr handle, string description, string baseName, WinDisplayDevice winDD)
        {
            using (NkTrace trace = NkLogger.trace("baseName: " + baseName + ", description: " + description))
            {
                this.handle = handle;
                this.description = description;
                this.baseName = baseName;
                VcpCapabilities = ReadVcpCapabilities();
                string edidStr = winDD.GetEdidString();
                edid = Edid.parse(edidStr);
            }
        }

        private string GetModel()
        {
            string mModel = description;
            if (VcpCapabilities.Model != null)
            {
                if (mModel.Equals("Generic PnP Monitor"))
                    mModel = "";
                else
                    mModel += ", ";
                mModel += VcpCapabilities.Model;
            }
            return mModel;
        }

        /// <summary>
        /// Returns unique identifier for a monitor
        /// </summary>
        /// <returns></returns>
        public string GetId()
        {
            // Ignore first 8 hex-characters (EDID magic number)
            // Read until the end of EDID header structure
            return edid.raw.Substring(16, 40-16);
        }

        public string GetName()
        {
            return GetModel();
        }

        private MonitorCapabilities ReadVcpCapabilities()
        {
            using (NkTrace trace = NkLogger.dbgTrace())
            {
                uint capabilitiesStringLength = 0;
                if (!Natives.GetCapabilitiesStringLength(handle, ref capabilitiesStringLength))
                    throw new SlimWin32Exception("GetCapabilitiesStringLength failed");

                StringBuilder capabilities = new StringBuilder((int)capabilitiesStringLength);
                if (!Natives.CapabilitiesRequestAndCapabilitiesReply(handle, capabilities, capabilitiesStringLength))
                    throw new SlimWin32Exception("CapabilitiesRequestAndCapabilitiesReply failed");
                //MainWindow.Trace("- capabilities (str): " + capabilities);

                return trace.appendExit(MonitorCapabilitiesParser.ParseVcpCapabilities(capabilities.ToString()));
            }
        }

        public Win32MonitorCapabilities GetCapabilities()
        {
            using (NkTrace trace = NkLogger.dbgTrace())
            {
                uint monitorCapabilities = 0;
                uint supportedColorTemperatures = 0;
                if (!Natives.GetMonitorCapabilities(handle, ref monitorCapabilities, ref supportedColorTemperatures))
                    throw new SlimWin32Exception("GetMonitorCapabilities failed");
                return trace.appendExit(new Win32MonitorCapabilities(monitorCapabilities, supportedColorTemperatures));
            }
        }

        public Brightness GetBrightness()
        {
            using (NkTrace trace = NkLogger.dbgTrace())
            {
                uint minBrightness = 0u, currBrightness = 0u, maxBrightness = 0u;
                if (!Natives.GetMonitorBrightness(handle, ref minBrightness, ref currBrightness, ref maxBrightness))
                    throw new SlimWin32Exception("GetMonitorBrightness failed");
                return trace.appendExit(new Brightness(currBrightness, minBrightness, maxBrightness));
            }
        }

        public void SetBrightness(uint newValue)
        {
            using (NkTrace trace = NkLogger.dbgTrace(newValue))
            {
                if (!Natives.SetMonitorBrightness(handle, newValue))
                    throw new SlimWin32Exception("SetMonitorBrightness failed");
            }
        }

        public Contrast GetContrast()
        {
            using (NkTrace trace = NkLogger.dbgTrace())
            {
                uint minContrast = 0u, currContrast = 0u, maxContrast = 0u;
                if (!Natives.GetMonitorBrightness(handle, ref minContrast, ref currContrast, ref maxContrast))
                    throw new SlimWin32Exception("GetMonitorContrast failed");
                return trace.appendExit(new Contrast(currContrast, minContrast, maxContrast));
            }
        }

        public void SetContrast(uint newValue)
        {
            using (NkTrace trace = NkLogger.dbgTrace(newValue))
            {
                if (!Natives.SetMonitorContrast(handle, newValue))
                    throw new SlimWin32Exception("SetMonitorContrast failed");
            }
        }

        public uint GetTemperature()
        {
            using (NkTrace trace = NkLogger.dbgTrace())
            {
                Natives.MC_COLOR_TEMPERATURE temperature = Natives.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_UNKNOWN;
                if (!Natives.GetMonitorColorTemperature(handle, ref temperature))
                    throw new SlimWin32Exception("GetMonitorContrast failed");
                return trace.appendExit(Natives.MC_COLOR_TEMPERATURE_ToUint(temperature));
            }
        }

        public void SetTemperature(uint newValue)
        {
            using (NkTrace trace = NkLogger.dbgTrace(newValue))
            {
                Natives.MC_COLOR_TEMPERATURE temperature = Natives.MC_COLOR_TEMPERATURE_FromUint(newValue);
                if (!Natives.SetMonitorColorTemperature(handle, temperature))
                    throw new SlimWin32Exception("SetMonitorContrast failed");
            }
        }

        public InputSource GetInputSource()
        {
            using (NkTrace trace = NkLogger.dbgTrace())
            {
                return trace.appendExit(Vcp.GetInputSource(handle));
            }
        }

        public void SetInputSource(InputSource value)
        {
            using (NkTrace trace = NkLogger.dbgTrace(value))
            {
                Vcp.SetInputSource(handle, value);
            }
        }

        public override string ToString()
        {
            return GetName() + ", baseName: " + baseName + ", description: " + description + ", ID: " + GetId();
        }

    }
}
