using System;
using SlimShady.DataFeed;
using SlimShady.MonitorManagers;
using Microsoft.Win32;
using System.Windows.Controls;

namespace SlimShady
{
    public class Configuration
    {
        public static ThingSpeakDataFeed DataFeed = ThingSpeakDataFeed.Instance;
        public static MonitorManager MonitorManager;

        private static Configuration instance;
        private readonly ConfigurationFile config;

        //private readonly String defaultMonitorMgrName = typeof(HardwareMonitorManager).Name;
        private readonly String hardwareMonitorMgrName = typeof(HardwareMonitorManager).Name;
        private readonly String gammarampMonitorMgrName = typeof(GammaRampMonitorManager).Name;
        private readonly String wmiMonitorMgrName = typeof(WmiMonitorManager).Name;
        private readonly String softwareMonitorMgrName = typeof(SoftwareMonitorManager).Name;
        
        private readonly String thingspeakFeedName = typeof(ThingSpeakDataFeed).Name;

        private const String ConfigFilePath = "Config.xml";
        private const String ConfigKeyAutoAdjust = "autoAdjust";
        private const String ConfigKeyMonitorManager = "MonitorManager";
        //private const String ConfigKeyDataFeed = "DataFeed";
        public const String ConfigKeyMonitorBrightness = "Brightness"; // This is the same as Brightness property Name in Monitor
        public const String ConfigKeyMonitorContrast = "Contrast"; // This is the same as Brightness property Name in Monitor
        public const String ConfigKeyMonitorTemperature = "Temperature"; // This is the same as Brightness property Name in Monitor

        private String ConfigKeyThingspeakAddress { get { return thingspeakFeedName + ".address"; } }
        private String ConfigKeyThingspeakAddressDefault { get { return "http://kawinski.net:3000"; } }
        private String ConfigKeyThingspeakChannel { get { return thingspeakFeedName + ".Channel"; } }
        private String ConfigKeyThingspeakChannelDefault { get { return "1"; } }
        private String ConfigKeyThingspeakLightFunction { get { return thingspeakFeedName + ".lightfunction"; } }
        private String ConfigKeyThingspeakLightFunctionDefault { get { return "y = 4*Math.Sqrt(x)"; } }

        public static Configuration Get()
        {
            return instance ?? (instance = new Configuration());
        }

        private Configuration()
        {
            config = new ConfigurationFile(ConfigFilePath);
        }

        public void LoadFromConfig(MainWindow mainWindow)
        {
            // Auto
            mainWindow.AutoCheckBox.IsChecked = config.GetBool(ConfigKeyAutoAdjust, false);

            // Monitors
            /* Disabled. Loading previous settings is useless
             * Most likely, user will relaunch the app with different lightning condition at different time,
             * so don't load Brightness/Contrast/Temperature settings from Config.
             * User will adjust settings manually */
            foreach (MonitorManager manager in MainWindow.MonitorManagers)
            {
                LoadMonitorSettingsInto(manager, manager.MasterMonitor);
                foreach (Monitor monitor in manager.GetMonitorsList())
                    LoadMonitorSettingsInto(manager, monitor);
            }

            ConfigurationControl settingsWindow = mainWindow.ConfigurationControl;

            // Monitor managers
            MonitorManager monitorManager = GetMonitorManager();
            MonitorManager = monitorManager;
            if (monitorManager.GetType().Name == hardwareMonitorMgrName)
                mainWindow.MonitorManagerTabControl.SelectedItem = mainWindow.MonitorManagerTabItemHardware;
            if (monitorManager.GetType().Name == gammarampMonitorMgrName)
                mainWindow.MonitorManagerTabControl.SelectedItem = mainWindow.MonitorManagerTabItemGammaRamp;
            if (monitorManager.GetType().Name == wmiMonitorMgrName)
                mainWindow.MonitorManagerTabControl.SelectedItem = mainWindow.MonitorManagerTabItemWmi;
            if (monitorManager.GetType().Name == softwareMonitorMgrName)
                mainWindow.MonitorManagerTabControl.SelectedItem = mainWindow.MonitorManagerTabItemSoftware;
            
            // Data feed
            settingsWindow.ThingSpeakAddressTextBox.Text = config.GetContent(ConfigKeyThingspeakAddress, ConfigKeyThingspeakAddressDefault);
            settingsWindow.ThingSpeakChannelTextBox.Text = config.GetContent(ConfigKeyThingspeakChannel, ConfigKeyThingspeakChannelDefault);
            settingsWindow.PlotFunction.Text = config.GetContent(ConfigKeyThingspeakLightFunction, ConfigKeyThingspeakLightFunctionDefault);

            // Start with Windows
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
            settingsWindow.StartWithWindowsCheckbox.IsChecked = registryKey != null && registryKey.GetValue("SlimShady") != null;
        }

        public void EnableAutoSave(MainWindow mainWindow)
        {
            // 'Auto'
            mainWindow.AutoCheckBox.Checked += (x, y) => config.Set(ConfigKeyAutoAdjust, true);
            mainWindow.AutoCheckBox.Unchecked += (x, y) => config.Set(ConfigKeyAutoAdjust, false);

            // Monitors
            foreach (MonitorManager manager in MainWindow.MonitorManagers)
            {
                ListenToChanges(manager, manager.MasterMonitor);
                foreach (Monitor monitor in manager.GetMonitorsList())
                    ListenToChanges(manager, monitor);
            }

            ConfigurationControl settingsWindow = mainWindow.ConfigurationControl;

            // Monitor Manager
            mainWindow.MonitorManagerTabControl.SelectionChanged += (x, y) =>
                {
                    if (mainWindow.MonitorManagerTabItemHardware.IsSelected) SetMonitorManager(HardwareMonitorManager.Instance);
                    if (mainWindow.MonitorManagerTabItemGammaRamp.IsSelected) SetMonitorManager(GammaRampMonitorManager.Instance);
                    if (mainWindow.MonitorManagerTabItemWmi.IsSelected) SetMonitorManager(WmiMonitorManager.Instance);
                    if (mainWindow.MonitorManagerTabItemSoftware.IsSelected) SetMonitorManager(SoftwareMonitorManager.Instance);
                };
            
            // Data feed
            settingsWindow.ThingSpeakAddressTextBox.TextChanged += (x, y) => config.Set(ConfigKeyThingspeakAddress, ((TextBox)x).Text);
            settingsWindow.ThingSpeakChannelTextBox.TextChanged += (x, y) => config.Set(ConfigKeyThingspeakChannel, ((TextBox)x).Text);
            settingsWindow.PlotFunction.TextChanged += (x, y) => config.Set(ConfigKeyThingspeakLightFunction, ((TextBox)x).Text);

            // Start with windows
            settingsWindow.StartWithWindowsCheckbox.Checked += (x, y) => SetRunWithWindows(true);
            settingsWindow.StartWithWindowsCheckbox.Unchecked += (x, y) => SetRunWithWindows(false);
        }

        private void SetMonitorManager(MonitorManager manager)
        {
            MonitorManager = manager;
            config.Set(ConfigKeyMonitorManager, manager.GetType().Name);
        }

        private String GetConfigPrefix(MonitorManager monitorManager, Monitor monitor)
        {
            return monitorManager.GetType().Name + "." + monitor.Name + ".";
        }

        private void SetRunWithWindows(bool shouldStartWithWindows)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (shouldStartWithWindows)
            {
                if (registryKey != null)
                    registryKey.SetValue("SlimShady", System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                if (registryKey != null) 
                    registryKey.DeleteValue("SlimShady");
            }
        }

        private MonitorManager GetMonitorManager()
        {
            String monitorManager = config.GetContent(ConfigKeyMonitorManager, hardwareMonitorMgrName);
            if (monitorManager == softwareMonitorMgrName)
                return SoftwareMonitorManager.Instance;
            if (monitorManager == gammarampMonitorMgrName)
                return GammaRampMonitorManager.Instance;
            if (monitorManager == wmiMonitorMgrName)
                return WmiMonitorManager.Instance;
            if (monitorManager == hardwareMonitorMgrName)
                return HardwareMonitorManager.Instance;
            MainWindow.Error("Cannot recognize monitor Manager: " + monitorManager);
            return HardwareMonitorManager.Instance;
        }
        
        private void LoadMonitorSettingsInto(MonitorManager monitorManager, Monitor monitor)
        {
            String prefix = GetConfigPrefix(monitorManager, monitor);

            if (monitor.SupportsBrightness)
            {
                int brightness = config.GetInt(prefix + ConfigKeyMonitorBrightness, -1);
                if (brightness != -1)
                    monitor.Brightness = brightness;
            }

            if (monitor.SupportsContrast)
            {
                int contrast = config.GetInt(prefix + ConfigKeyMonitorContrast, -1);
                if (contrast != -1)
                    monitor.Contrast = contrast;
            }

            if (monitor.SupportsTemperature)
            {
                int temperature = config.GetInt(prefix + ConfigKeyMonitorTemperature, -1);
                if (temperature != -1)
                    monitor.Temperature = temperature;
            }
        }

        private void ListenToChanges(MonitorManager manager, Monitor monitor)
        {
            monitor.PropertyChanged += (x, y) =>
            {
                String prefix = GetConfigPrefix(manager, monitor);
                int value;
                switch (y.PropertyName)
                {
                    case ConfigKeyMonitorBrightness: value = monitor.Brightness; break;
                    case ConfigKeyMonitorContrast: value = monitor.Contrast; break;
                    case ConfigKeyMonitorTemperature: value = monitor.Temperature; break;
                    default: value = -1; MainWindow.Error("Unsupported monitor property '" + y.PropertyName + "'"); break;
                }
                if (value != -1)
                    config.Set(prefix + y.PropertyName, value);
            };
        }
    }
}