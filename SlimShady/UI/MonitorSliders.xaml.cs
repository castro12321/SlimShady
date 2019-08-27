using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SlimShady.Configuration;
using SlimShadyCore.Monitors;
using SlimShadyCore.Monitors.Components;
using SlimShadyEssentials.Monitors.Hardware;

namespace SlimShady
{
    public partial class MonitorSliders : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly DependencyProperty OwnedMonitorProperty = DependencyProperty.Register("OwnedMonitor", typeof(Monitor), typeof(MonitorSliders), new FrameworkPropertyMetadata(null, OnOwnedMonitorChanged));
        public Monitor OwnedMonitor
        {
            get => (Monitor)GetValue(OwnedMonitorProperty);
            set => SetValue(OwnedMonitorProperty, value);
        }

        private static void OnOwnedMonitorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MonitorSliders)d).RefreshUI();
        }

        public static readonly DependencyProperty OwnedConfigProperty = DependencyProperty.Register("OwnedConfig", typeof(MonitorConfiguration), typeof(MonitorSliders), new FrameworkPropertyMetadata(null, OnOwnedConfigChanged));
        public MonitorConfiguration OwnedConfig
        {
            get => (MonitorConfiguration)GetValue(OwnedConfigProperty);
            set => SetValue(OwnedConfigProperty, value);
        }

        private static void OnOwnedConfigChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MonitorSliders)d).NotifyPropertyChanged(nameof(Order));
        }

        public int Order
        {
            get => OwnedConfig?.Order ?? 1;
            set => OwnedConfig.Order = value;
        }

        private BrightnessMonitorComponent mBrightnessComponent;
        public BrightnessMonitorComponent BrightnessComponent
        {
            get => mBrightnessComponent;
            private set
            {
                mBrightnessComponent = value;
                NotifyPropertyChanged();
            }
        }

        private ContrastMonitorComponent mContrastComponent;
        public ContrastMonitorComponent ContrastComponent
        {
            get => mContrastComponent;
            private set
            {
                mContrastComponent = value;
                NotifyPropertyChanged();
            }
        }

        private TemperatureMonitorComponent mTemperatureComponent;
        public TemperatureMonitorComponent TemperatureComponent
        {
            get => mTemperatureComponent;
            private set
            {
                mTemperatureComponent = value;
                NotifyPropertyChanged();
            }
        }

        private DoubleCollection mTemperatureComponentSupportedValues;
        public DoubleCollection TemperatureComponentSupportedValues
        {
            get => mTemperatureComponentSupportedValues;
            private set
            {
                mTemperatureComponentSupportedValues = value;
                NotifyPropertyChanged();
            }
        }

        private InputSourceMonitorComponent mInputSourceComponent;
        public InputSourceMonitorComponent InputSourceComponent
        {
            get => mInputSourceComponent;
            private set
            {
                mInputSourceComponent = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility mBrightnessRowVisibility = Visibility.Collapsed;
        public Visibility BrightnessRowVisibility
        {
            get => mBrightnessRowVisibility;
            private set
            {
                mBrightnessRowVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility mContrastRowVisibility = Visibility.Collapsed;
        public Visibility ContrastRowVisibility
        {
            get => mContrastRowVisibility;
            private set
            {
                mContrastRowVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility mTemperatureRowVisibility = Visibility.Collapsed;
        public Visibility TemperatureRowVisibility
        {
            get => mTemperatureRowVisibility;
            private set
            {
                mTemperatureRowVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility mInputSourceRowVisibility = Visibility.Collapsed;
        public Visibility InputSourceRowVisibility
        {
            get => mInputSourceRowVisibility;
            private set
            {
                mInputSourceRowVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public MonitorSliders()
        {
            //DataContext = this;
            InitializeComponent();
        }

        private void RefreshUI()
        {
            if (OwnedMonitor.HasComponent<BrightnessMonitorComponent>())
            {
                BrightnessComponent = OwnedMonitor.GetSingleComponent<BrightnessMonitorComponent>();
                BrightnessRowVisibility = Visibility.Visible;
            }

            if (OwnedMonitor.HasComponent<ContrastMonitorComponent>())
            {
                ContrastComponent = OwnedMonitor.GetSingleComponent<ContrastMonitorComponent>();
                ContrastRowVisibility = Visibility.Visible;
            }

            if (OwnedMonitor.HasComponent<TemperatureMonitorComponent>())
            {
                TemperatureComponent = OwnedMonitor.GetSingleComponent<TemperatureMonitorComponent>();
                if (TemperatureComponent.SupportedValues != null)
                    TemperatureComponentSupportedValues = new DoubleCollection(TemperatureComponent.SupportedValues.Select(val => (double)val));
                TemperatureRowVisibility = Visibility.Visible;
            }

            if (OwnedMonitor.HasComponent<InputSourceMonitorComponent>())
            {
                InputSourceComponent = OwnedMonitor.GetSingleComponent<InputSourceMonitorComponent>();
                InputSourceRowVisibility = Visibility.Visible;
            }
        }
    }
}
