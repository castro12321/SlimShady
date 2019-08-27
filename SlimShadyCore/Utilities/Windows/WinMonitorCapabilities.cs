using System.Collections.Generic;

namespace SlimShadyCore.Utilities.Windows
{
    public class Win32MonitorCapabilities
    {
        private static class Natives
        {
            // supported color temperatures returned by GetMonitorCapabilities
            public const uint MC_SUPPORTED_COLOR_TEMPERATURE_NONE = 0x00000000;
            public const uint MC_SUPPORTED_COLOR_TEMPERATURE_4000K = 0x00000001;
            public const uint MC_SUPPORTED_COLOR_TEMPERATURE_5000K = 0x00000002;
            public const uint MC_SUPPORTED_COLOR_TEMPERATURE_6500K = 0x00000004;
            public const uint MC_SUPPORTED_COLOR_TEMPERATURE_7500K = 0x00000008;
            public const uint MC_SUPPORTED_COLOR_TEMPERATURE_8200K = 0x00000010;
            public const uint MC_SUPPORTED_COLOR_TEMPERATURE_9300K = 0x00000020;
            public const uint MC_SUPPORTED_COLOR_TEMPERATURE_10000K = 0x00000040;
            public const uint MC_SUPPORTED_COLOR_TEMPERATURE_11500K = 0x00000080;
        }

        // We ignore all caps except for temperature presets because they are not reliable
        // Some monitors fail to provide caps even though they *do* support e.g. brightness and contrast.
        // private uint monitorCapabilities;

        public List<uint> SupportedColorTemperatures { get; private set; }

        public Win32MonitorCapabilities(uint monitorCapabilities, uint supportedColorTemperaturesRaw)
        {
            //this.monitorCapabilities = monitorCapabilities;
            SupportedColorTemperatures = ExtractSupportedTemperatures(supportedColorTemperaturesRaw);
        }

        private List<uint> ExtractSupportedTemperatures(uint supportedColorTemperaturesRaw)
        {
            List<uint> supportedTemperaturesList = new List<uint>();

            if ((supportedColorTemperaturesRaw & Natives.MC_SUPPORTED_COLOR_TEMPERATURE_4000K) > 0)
                supportedTemperaturesList.Add(4000);
            if ((supportedColorTemperaturesRaw & Natives.MC_SUPPORTED_COLOR_TEMPERATURE_5000K) > 0)
                supportedTemperaturesList.Add(5000);
            if ((supportedColorTemperaturesRaw & Natives.MC_SUPPORTED_COLOR_TEMPERATURE_6500K) > 0)
                supportedTemperaturesList.Add(6500);
            if ((supportedColorTemperaturesRaw & Natives.MC_SUPPORTED_COLOR_TEMPERATURE_7500K) > 0)
                supportedTemperaturesList.Add(7500);
            if ((supportedColorTemperaturesRaw & Natives.MC_SUPPORTED_COLOR_TEMPERATURE_8200K) > 0)
                supportedTemperaturesList.Add(8200);
            if ((supportedColorTemperaturesRaw & Natives.MC_SUPPORTED_COLOR_TEMPERATURE_9300K) > 0)
                supportedTemperaturesList.Add(9300);
            if ((supportedColorTemperaturesRaw & Natives.MC_SUPPORTED_COLOR_TEMPERATURE_10000K) > 0)
                supportedTemperaturesList.Add(10000);
            if ((supportedColorTemperaturesRaw & Natives.MC_SUPPORTED_COLOR_TEMPERATURE_11500K) > 0)
                supportedTemperaturesList.Add(11500);

            return supportedTemperaturesList;
        }

        public override string ToString()
        {
            return "Supported temperatures: " + string.Join(", ", SupportedColorTemperatures);
        }
    }
}
