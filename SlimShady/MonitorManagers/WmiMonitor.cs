using System;
using System.Management;

namespace SlimShady.MonitorManagers
{
    // Tip: "WMI code creator" is really useful tool

    public class WmiMonitor : Monitor
    {
        private readonly ManagementObject monitor;

        public WmiMonitor(ManagementObject monitor, MonitorManager manager)
            : base(monitor.Properties["InstanceName"].Value.ToString(), manager)
        {
            this.monitor = monitor;
            SupportsBrightness = true;

            try
            {
                ManagementScope scope = new ManagementScope("root\\WMI");
                String sQuery = "SELECT * FROM WmiMonitorBrightness WHERE InstanceName='" + Name.Replace("\\", "\\\\") + "'";
                SelectQuery query = new SelectQuery(sQuery);
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                foreach (var item in searcher.Get())
                    mBrightness = (byte)item.Properties["CurrentBrightness"].Value;
            }
            catch(ManagementException e)
            {
                MainWindow.Error("WMI monitor cannot read current Brightness: " + e.Message);
            }

        }

        private int mBrightness;
        public override int Brightness
        {
            get { return mBrightness; }
            set
            {
                mBrightness = value;
                int mastered = Lerp(MinBrightness, Math.Min(Brightness, MaxBrightness), Manager.MasterMonitor.Brightness);
                const int timeoutInSeconds = 5;
                monitor.InvokeMethod("WmiSetBrightness", new Object[] { timeoutInSeconds, (byte)mastered });
                NotifyPropertyChanged();
            }
        }

        public override int Contrast
        {
            get { return 0; }
            set {  }
        }

        public override int Temperature
        {
            get { return 0; }
            set {  }
        }
    }
}
