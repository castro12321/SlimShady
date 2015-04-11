using System.Collections.Generic;

namespace SlimShady.MonitorManagers
{
    /// <summary>
    /// Known exceptions:
    /// - ArgumentException thrown when Monitor argument is of the wrong type. For example SoftwareMonitor is passed to HardwareBrightnessManager instead to SoftwareBrightnessManager
    /// - NotSuppoertException thrown when Manager or monitor doesn't support specific command
    /// </summary>
    public abstract class MonitorManager
    {
        public MasterMonitor MasterMonitor = new MasterMonitor();

        // This method may become obsolete after, for example, plugging/unplugging monitors
        // When this happens, MonitorManager should recreate the list and classes that use our monitors should update to the changes too
        // Optionally, we could restart the application. That would be easier
        // But for now, we do nothing! yay! It happens so rarely, that we can ignore it for now. Add it when we get more people using this app?
        public abstract List<Monitor> GetMonitorsList();

        public void LinkMasterToChilds()
        {
            MasterMonitor.PropertyChanged += (x, y) =>
            {
                switch (y.PropertyName)
                {
                    case Configuration.ConfigKeyMonitorBrightness:
                        foreach (Monitor monitor in GetMonitorsList())
                            monitor.Brightness = monitor.Brightness;
                        break;
                    case Configuration.ConfigKeyMonitorContrast:
                        foreach (Monitor monitor in GetMonitorsList())
                            monitor.Contrast = monitor.Contrast;
                        break;
                    case Configuration.ConfigKeyMonitorTemperature:
                        foreach (Monitor monitor in GetMonitorsList())
                            monitor.Temperature = monitor.Temperature;
                        break;
                    default: MainWindow.Error("Unsupported monitor property '" + y.PropertyName + "'"); break;
                }
            };
        }
    }
}
