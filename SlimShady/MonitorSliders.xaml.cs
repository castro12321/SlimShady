using System.Windows.Controls;
using SlimShady.MonitorManagers;

namespace SlimShady
{
    public partial class MonitorSliders : UserControl
    {
        public Monitor OwnedMonitor;

        public MonitorSliders(Monitor ownedMonitor)
        {
            OwnedMonitor = ownedMonitor;
            InitializeComponent();

            MonitorNameLabel.Content = ownedMonitor.Name;

            if (ownedMonitor.SupportsBrightness)
            {
                BrightnessSlider.Minimum = ownedMonitor.MinBrightness;
                BrightnessSlider.Maximum = ownedMonitor.MaxBrightness;
                BrightnessSlider.Value = ownedMonitor.Brightness;
                BrightnessSlider.ValueChanged += (x, y) => ownedMonitor.Brightness = (int)y.NewValue;
            }
            else
            {
                //BrightnessSliderText.IsEnabled = false;
                //BrightnessSlider.IsEnabled = false;
                //BrightnessSlider.ToolTip = "This monitor doesn't support Brightness changing";
                BrightnessSliderLabel.Visibility = System.Windows.Visibility.Collapsed;
                BrightnessSlider.Visibility = System.Windows.Visibility.Collapsed;
                BrightnessSliderText.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (ownedMonitor.SupportsContrast)
            {
                ContrastSlider.Minimum = ownedMonitor.MinContrast;
                ContrastSlider.Maximum = ownedMonitor.MaxContrast;
                ContrastSlider.Value = ownedMonitor.Contrast;
                ContrastSlider.ValueChanged += (x, y) => ownedMonitor.Contrast = (int)y.NewValue;
            }
            else
            {
                //ContrastSliderText.IsEnabled = false;
                //ContrastSlider.IsEnabled = false;
                //ContrastSlider.ToolTip = "This monitor doesn't support Contrast changing";
                ContrastSliderLabel.Visibility = System.Windows.Visibility.Collapsed;
                ContrastSlider.Visibility = System.Windows.Visibility.Collapsed;
                ContrastSliderText.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (ownedMonitor.SupportsTemperature)
            {
                TemperatureSlider.Minimum = ownedMonitor.MinTemperature;
                TemperatureSlider.Maximum = ownedMonitor.MaxTemperature;
                if (ownedMonitor.MaxTemperature <= 100)
                {
                    TemperatureSlider.TickFrequency = 1;
                    TemperatureSlider.LargeChange = 5;
                }
                if (ownedMonitor.SupportedColorTemperaturesList != null)
                    TemperatureSlider.Ticks = ownedMonitor.SupportedColorTemperaturesList;
                TemperatureSlider.Value = ownedMonitor.Brightness;
                TemperatureSlider.ValueChanged += (x, y) => ownedMonitor.Temperature = (int)y.NewValue;
            }
            else
            {
                //TemperatureSliderText.IsEnabled = false;
                //TemperatureSlider.IsEnabled = false;
                //TemperatureSlider.ToolTip = "This monitor doesn't support Temperature changing";
                TemperatureSliderLabel.Visibility = System.Windows.Visibility.Collapsed;
                TemperatureSlider.Visibility = System.Windows.Visibility.Collapsed;
                TemperatureSliderText.Visibility = System.Windows.Visibility.Collapsed;
            }

            //BrightnessSlider.ValueChanged += (x, y) => MainWindow.Instance.AutoCheckBox.IsChecked = false;
            //ContrastSlider.ValueChanged += (x, y) => MainWindow.Instance.AutoCheckBox.IsChecked = false;
            //TemperatureSlider.ValueChanged += (x, y) => MainWindow.Instance.AutoCheckBox.IsChecked = false;

            ownedMonitor.PropertyChanged += (x, y) =>
                {
                    Monitor monitor = (Monitor)x;
                    // ReSharper disable CompareOfFloatsByEqualityOperator
                    if (y.PropertyName == Configuration.ConfigKeyMonitorBrightness)
                        if(BrightnessSlider.Value != monitor.Brightness)
                            BrightnessSlider.Value = monitor.Brightness;
                    if (y.PropertyName == Configuration.ConfigKeyMonitorContrast)
                        if(ContrastSlider.Value != monitor.Contrast)
                            ContrastSlider.Value = monitor.Contrast;
                    if (y.PropertyName == Configuration.ConfigKeyMonitorTemperature)
                        if (TemperatureSlider.Value != monitor.Temperature)
                            TemperatureSlider.Value = monitor.Temperature;
                    // ReSharper restore CompareOfFloatsByEqualityOperator
                };
        }
    }
}
