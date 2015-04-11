using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;

namespace SlimShady.MonitorManagers
{
    [SuppressMessage("ReSharper", "InconsistentNaming")] // Names are copied from WinApi
    public class HwmWinApi
    {
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

        // monitor capabilities returned by GetMonitorCapabilities
        public const uint MC_CAPS_NONE = 0x00000000;
        public const uint MC_CAPS_BRIGHTNESS = 0x00000002;
        public const uint MC_CAPS_CONTRAST = 0x00000004;
        public const uint MC_CAPS_COLOR_TEMPERATURE = 0x00000008;
        //private const uint MC_CAPS_RESTORE_FACTORY_DEFAULTS = 0x00000400;
        //private const uint MC_CAPS_RESTORE_FACTORY_COLOR_DEFAULTS = 0x00000800;
        //private const uint MC_RESTORE_FACTORY_DEFAULTS_ENABLES_MONITOR_SETTINGS = 0x00001000;

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

        [DllImport("dxva2.dll", SetLastError = true)]
        public static extern bool GetCapabilitiesStringLength(IntPtr hMonitor, ref uint capabilitiesStringLengthInCharacters);

        [DllImport("dxva2.dll", SetLastError = true)]
        public static extern bool CapabilitiesRequestAndCapabilitiesReply(IntPtr hMonitor, StringBuilder ASCIICapabilitiesString, uint capabilitiesStringLengthInCharacters);

        [DllImport("dxva2.dll", SetLastError = true)]
        public static extern bool GetMonitorCapabilities(IntPtr hMonitor, ref uint monitorCapabilities, ref uint supportedColorTemperatures);

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
    }

    public class HardwareMonitor : Monitor
    {
        private readonly IntPtr hMonitor;

        public HardwareMonitor(IntPtr hMonitor, String name, MonitorManager manager)
            : base(name, manager)
        {
            this.hMonitor = hMonitor;

            uint monitorCapabilities = 0;
            uint supportedColorTemperatures = 0;
            if (!HwmWinApi.GetMonitorCapabilities(hMonitor, ref monitorCapabilities, ref supportedColorTemperatures))
            {
                MainWindow.Win32Error("Monitor " + name + " cannot GetMonitorCapabilities(). However, we'll try to communicate with that monitor anyway.");
                monitorCapabilities = HwmWinApi.MC_CAPS_BRIGHTNESS | HwmWinApi.MC_CAPS_CONTRAST | HwmWinApi.MC_CAPS_COLOR_TEMPERATURE;
                supportedColorTemperatures = 0xFFFFFFFF;
            }

            if ((monitorCapabilities & HwmWinApi.MC_CAPS_BRIGHTNESS) > 0)
            {
                SupportsBrightness = true;

                uint minBrightness = 0u, currBrightness = 0u, maxBrightness = 0u;
                if (HwmWinApi.GetMonitorBrightness(hMonitor, ref minBrightness, ref currBrightness, ref maxBrightness))
                {
                    MinBrightness = (int)minBrightness;
                    MaxBrightness = (int)maxBrightness;
                    mBrightness = (int)currBrightness; // Don't call property. Just update the value. Monitor's Brightness is already set to that value, so no need to waste time calling monitor API
                }
                else
                    MainWindow.Win32Error("HardwareMonitor Cannot GetMonitorBrightness()");
            }

            if ((monitorCapabilities & HwmWinApi.MC_CAPS_CONTRAST) > 0)
            {
                SupportsContrast = true;

                uint minContrast = 0u, currContrast = 0u, maxContrast = 0u;
                if (HwmWinApi.GetMonitorContrast(hMonitor, ref minContrast, ref currContrast, ref maxContrast))
                {
                    MinContrast = (int)minContrast;
                    MaxContrast = (int)maxContrast;
                    mContrast = (int)currContrast; // Don't call property. Just update the value. Monitor's Brightness is already set to that value, so no need to waste time calling monitor API
                }
                else
                    MainWindow.Win32Error("HardwareMonitor Cannot GetMonitorContrast()");
            }

            SupportedColorTemperaturesList = new DoubleCollection();
            if ((monitorCapabilities & HwmWinApi.MC_CAPS_COLOR_TEMPERATURE) > 0)
            {
                SupportsTemperature = true;

                HwmWinApi.MC_COLOR_TEMPERATURE currentColorTemperature = HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_UNKNOWN;
                if (HwmWinApi.GetMonitorColorTemperature(hMonitor, ref currentColorTemperature))
                    mTemperature = ConverWinTemperature(currentColorTemperature);
                else
                    MainWindow.Win32Error("HardwareMonitor Cannot GetMonitorColorTemperature()");

                if ((supportedColorTemperatures & HwmWinApi.MC_SUPPORTED_COLOR_TEMPERATURE_4000K) > 0)
                    SupportedColorTemperaturesList.Add(4000);
                if ((supportedColorTemperatures & HwmWinApi.MC_SUPPORTED_COLOR_TEMPERATURE_5000K) > 0)
                    SupportedColorTemperaturesList.Add(5000);
                if ((supportedColorTemperatures & HwmWinApi.MC_SUPPORTED_COLOR_TEMPERATURE_6500K) > 0)
                    SupportedColorTemperaturesList.Add(6500);
                if ((supportedColorTemperatures & HwmWinApi.MC_SUPPORTED_COLOR_TEMPERATURE_7500K) > 0)
                    SupportedColorTemperaturesList.Add(7500);
                if ((supportedColorTemperatures & HwmWinApi.MC_SUPPORTED_COLOR_TEMPERATURE_8200K) > 0)
                    SupportedColorTemperaturesList.Add(8200);
                if ((supportedColorTemperatures & HwmWinApi.MC_SUPPORTED_COLOR_TEMPERATURE_9300K) > 0)
                    SupportedColorTemperaturesList.Add(9300);
                if ((supportedColorTemperatures & HwmWinApi.MC_SUPPORTED_COLOR_TEMPERATURE_10000K) > 0)
                    SupportedColorTemperaturesList.Add(10000);
                if ((supportedColorTemperatures & HwmWinApi.MC_SUPPORTED_COLOR_TEMPERATURE_11500K) > 0)
                    SupportedColorTemperaturesList.Add(11500);

                if (SupportedColorTemperaturesList.Count == 0)
                    SupportsTemperature = false;
            }

            MainWindow.Trace("DBG HWMON CAP STR " + name + ":");

            uint capabilitiesStringLength = 0;
            if (!HwmWinApi.GetCapabilitiesStringLength(hMonitor, ref capabilitiesStringLength))
            {
                MainWindow.Win32Error("Cannot get GetCapabilitiesStringLength()");
                return;
            }

            StringBuilder capabilities = new StringBuilder((int)capabilitiesStringLength);
            if (!HwmWinApi.CapabilitiesRequestAndCapabilitiesReply(hMonitor, capabilities, capabilitiesStringLength))
            {
                MainWindow.Win32Error("Cannot get CapabilitiesRequestAndCapabilitiesReply()");
                return;
            }

            MainWindow.Trace("CAPABILITIES STR --> " + capabilities);
            MainWindow.Trace(" - " + monitorCapabilities);
            MainWindow.Trace(" - " + supportedColorTemperatures);
        }

        private int mBrightness;
        public override int Brightness
        {
            get { return mBrightness; }
            set
            {
                mBrightness = value;
                int mastered = Lerp(MinBrightness, Math.Min(Brightness, MaxBrightness), Manager.MasterMonitor.Brightness);
                if (!HwmWinApi.SetMonitorBrightness(hMonitor, (uint)mastered))
                    MainWindow.Win32Error("HardwareMonitor Cannot SetMonitorBrightness()");
                NotifyPropertyChanged();
            }
        }

        private int mContrast;
        public override int Contrast
        {
            get { return mContrast; }
            set
            {
                mContrast = value;
                int mastered = Lerp(MinContrast, Math.Min(Contrast, MaxContrast), Manager.MasterMonitor.Contrast);
                if (!HwmWinApi.SetMonitorContrast(hMonitor, (uint)mastered))
                    MainWindow.Win32Error("HardwareMonitor Cannot SetMonitorContrast()");
                NotifyPropertyChanged();
            }
        }
        
        int mTemperature;
        public override int Temperature
        {
            get { return mTemperature; }
            set
            {
                mTemperature = value;
                int mastered = Lerp(MinTemperature, Math.Min(Temperature, MaxTemperature), Manager.MasterMonitor.Temperature);
                if (!HwmWinApi.SetMonitorColorTemperature(hMonitor, FindClosestSupportedTemperature(mastered)))
                    MainWindow.Win32Error("HardwareMonitor Cannot SetMonitorColorTemperature()");
                NotifyPropertyChanged();
            }
        }

        private int ConverWinTemperature(HwmWinApi.MC_COLOR_TEMPERATURE value)
        {
            switch(value)
            {
                case HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_4000K: return 4000;
                case HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_5000K: return 5000;
                case HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_6500K: return 6500;
                case HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_7500K: return 7500;
                case HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_8200K: return 8200;
                case HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_9300K: return 9300;
                case HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_10000K: return 10000;
                case HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_11500K: return 11500;
                case HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_UNKNOWN: return 0;
                default: throw new ArgumentException();
            }
        }

        private HwmWinApi.MC_COLOR_TEMPERATURE FindClosestSupportedTemperature(int value)
        {
            double closest = SupportedColorTemperaturesList[0];
            foreach(double supportedTemp in SupportedColorTemperaturesList)
            {
                double newDiff = Math.Abs(supportedTemp - value);
                double oldDiff = Math.Abs(closest - value);
                if (newDiff < oldDiff)
                    closest = supportedTemp;
            }

            if (closest >= 15000) throw new ArgumentException("Temperature cannot go above 15000");
            if (closest >= 10750) return HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_11500K;
            if (closest >= 9650) return HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_10000K;
            if (closest >= 8750) return HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_9300K;
            if (closest >= 7850) return HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_8200K;
            if (closest >= 7000) return HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_7500K;
            if (closest >= 5750) return HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_6500K;
            if (closest >= 4500) return HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_5000K;
            if (closest >= 3500) return HwmWinApi.MC_COLOR_TEMPERATURE.MC_COLOR_TEMPERATURE_4000K;
            throw new ArgumentException("Temperature cannot go below 3500");
        }
    }
}
