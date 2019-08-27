using SlimShady.Configuration;
using SlimShady.Utilities;
using SlimShadyCore;
using SlimShadyCore.Monitors;
using SlimShadyCore.Monitors.Components;
using SlimShadyCore.Utilities;
using SlimShadyEssentials;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace SlimShady
{
    public class SlimApp
    {
        public SlimCore core;
        public ConfigurationNew Config;
        // private Shortcuts shortcutsMgr;
        // private CommandMgr commandsMgr;

        public SlimApp()
        {
            NkLogger.Configure();
            NkLogger.info("Initializing SlimShady");
            core = new SlimCore();
            NkLogger.info("Core initialized");
            EssentialsMain.Initialize(core);
            NkLogger.info("Essentials initialized");
            Config = ConfigurationHelper.GetConfig();
            Initialize_1_MonitorManagers();
            Initialize_2_DataFeeds();
            NkLogger.info("SlimShady initialized");

            monitorChangeDetector = new MonitorChangeDetector();
            monitorChangeDetector.HardwareMonitorChangeDetected += MonitorChangeDetector_HardwareMonitorChangeDetected;
            NkLogger.info("DEBUG INITIALIZED");
        }

        private MonitorChangeDetector monitorChangeDetector;

        private void MonitorChangeDetector_HardwareMonitorChangeDetected()
        {
            // Must use Dispatcher.BeginInvoke because of WMI manager which dislikes multithreading.
            // Other managers should work fine without it.
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() =>
                {
                    foreach (var amm in core.ActiveMonitorManagers)
                        amm.RefreshMonitors();
                }));
        }


        private void Initialize_2_DataFeeds()
        {
            using (NkTrace trace = NkLogger.trace())
            {
                /*
                // Add auto Brightness adjustments using data feed
                DispatcherTimer dispatcher = new DispatcherTimer();
                dispatcher.Tick += (x, y) =>
                {
                    if (AutoCheckBox.IsChecked ?? true)
                        ConfigurationOld.DataFeed.AdjustMonitorSettings(ConfigurationOld.MonitorManager);
                };
                dispatcher.Interval = new TimeSpan(0, 0, 0, 0, 1000);
                dispatcher.Start();
                */
            }
        }

        private void Initialize_1_MonitorManagers()
        {
            using (NkTrace trace = NkLogger.trace())
            {
                NkLogger.info("Supported managers:");
                foreach (MonitorManagerProvider smm in core.SupportedMonitorManagers)
                    NkLogger.info(" - " + smm.Id);

                NkLogger.info("Activating managers");
                foreach (MonitorManagerProvider smm in core.SupportedMonitorManagers)
                {
                    if (Config.GetOrCreateManager(smm.Id).Enabled)
                        core.ActivateMonitorManager(smm);
                }

                NkLogger.info("Loading and attaching Configuration");
                // Initialize Config
                // - Load properties from configuration
                // - Enable auto-save
                foreach (MonitorManager manager in core.ActiveMonitorManagers)
                {
                    MonitorManagerConfiguration managerConfig = Config.GetOrCreateManager(manager.Id);
                    manager.Monitors.CollectionChanged += (sender, evnt) => Initialize_1a_Monitors(manager.Monitors, managerConfig);
                    Initialize_1a_Monitors(manager.Monitors, managerConfig);
                }

                // Debug logs
                NkLogger.info("");
                NkLogger.info("Initialized MonitorManagers:");
                foreach (MonitorManager mm in core.ActiveMonitorManagers)
                {
                    NkLogger.info("- " + mm);
                    foreach (Monitor m in mm.Monitors)
                        NkLogger.info("    * " + m);
                }
            }
        }

        private void Initialize_1a_Monitors(IEnumerable<Monitor> monitors, MonitorManagerConfiguration managerConfig)
        {
            foreach (var monitor in monitors)
                Initialize_1a_Monitors(monitor, managerConfig);
        }


        private void Initialize_1a_Monitors(Monitor monitor, MonitorManagerConfiguration managerConfig)
        {
            MonitorConfiguration monitorConfig = managerConfig.GetOrCreateMonitor(monitor.Id);
            if (monitorConfig.DispName != null)
                monitor.DispName = monitorConfig.DispName;
            monitor.DispNameChanged += (property, newValue) => monitorConfig.DispName = newValue;

            if (monitor.HasComponent<BrightnessMonitorComponent>())
            {
                BrightnessMonitorComponent component = monitor.GetSingleComponent<BrightnessMonitorComponent>();
                if (monitorConfig.Brightness != null)
                    component.CurrValue = monitorConfig.Brightness.Value;
                component.ValueChanged += (property, newValue) => monitorConfig.Brightness = newValue;
            }

            if (monitor.HasComponent<ContrastMonitorComponent>())
            {
                ContrastMonitorComponent component = monitor.GetSingleComponent<ContrastMonitorComponent>();
                if (monitorConfig.Contrast != null)
                    component.CurrValue = monitorConfig.Contrast.Value;
                component.ValueChanged += (property, newValue) => monitorConfig.Contrast = newValue;
            }

            if (monitor.HasComponent<TemperatureMonitorComponent>())
            {
                TemperatureMonitorComponent component = monitor.GetSingleComponent<TemperatureMonitorComponent>();
                if (monitorConfig.Temperature != null)
                    component.CurrValue = monitorConfig.Temperature.Value;
                component.ValueChanged += (property, newValue) => monitorConfig.Temperature = newValue;
            }
        }

        
    }
}
