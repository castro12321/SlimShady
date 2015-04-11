namespace SlimShady.MonitorManagers
{
    public class MasterMonitor : Monitor
    {
        private int brightnessModifierPercent = 100;
        private int contrastModifierPercent = 100;
        //private int TemperatureModifierPercent = 100;

        public MasterMonitor()
            : base("Master", null)
        {
            MinTemperature = 0;
            MaxTemperature = 100;
            SupportsBrightness = true;
            SupportsContrast = true;
            SupportsTemperature = false;
        }

        public override int Brightness
        {
            get { return brightnessModifierPercent; }
            set { brightnessModifierPercent = value; NotifyPropertyChanged(); }
        }

        public override int Contrast
        {
            get { return contrastModifierPercent; }
            set { contrastModifierPercent = value; NotifyPropertyChanged(); }
        }

        public override int Temperature
        {
            get { return 100; }
            set { }
        }

    }
}
