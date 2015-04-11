using System;
using System.Windows.Forms;

namespace SlimShady.MonitorManagers
{
    public class SoftwareMonitor : Monitor
    {
        private readonly SoftwareMonitorWindowOverlay overlay;

        public SoftwareMonitor(Screen screen, MonitorManager manager)
            : base(screen.DeviceName, manager)
        {
            // Initialize Brightness management
            overlay = new SoftwareMonitorWindowOverlay(screen);
            SupportsBrightness = true;
            MinBrightness = 20;
            mBrightness = 100; // The window is always recreated with 100% transparency
        }

        private int mBrightness;
        public override int Brightness
        {
            get { return mBrightness; }
            set
            {
                mBrightness = value;
                int mastered = Lerp(MinBrightness, Math.Min(Brightness, MaxBrightness), Manager.MasterMonitor.Brightness);
                mastered = 100 - mastered;
                overlay.Opacity = (float)mastered / 100.0;
                NotifyPropertyChanged();
            }
        }

        public override int Contrast
        {
            get { return 0; }
            set { /* TODO: Use SetGammaRamp() to support software Contrast changing? */ }
        }

        public override int Temperature
        {
            get
            {
                return 0;
                //return (window.Background as SolidColorBrush).Color.R;
            }
            set
            {
                //window.Background = new SolidColorBrush(Color.FromRgb((byte)((kelvin * 255) / 100), (byte)0, (byte)0));
            }
        }
    }
}
