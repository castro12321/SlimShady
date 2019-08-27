using SlimShady.Configuration;
using SlimShady.DataFeed;
using SlimShadyCore;
using SlimShadyCore.Monitors;
using SlimShadyCore.Utilities;
using SlimShadyCore.Utilities.Collections;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace SlimShady
{
    public partial class ConfigurationControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /*private SlimCore mCore;
        public SlimCore Core
        {
            get => mCore;
            set
            {
                mCore = value;
                NotifyPropertyChanged();
            }
        }*/

        public static readonly DependencyProperty CoreProperty = DependencyProperty.Register("Core", typeof(SlimCore), typeof(ConfigurationControl));
        public SlimCore Core
        {
            get => (SlimCore)GetValue(CoreProperty);
            set => SetValue(CoreProperty, value);
        }

        [Obsolete] // To keep WPF XAML and designer happy. Might contain test/debug data. One should recreate the control with proper constructor arguments.
        public ConfigurationControl()
        {
            DataContext = this;
            InitializeComponent();

            StartWithWindowsCheckbox.IsChecked = ConfigurationHelper.GetConfig().StartWithWindows;
        }

        private void StartWithWindowsCheckbox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            ConfigurationHelper.GetConfig().StartWithWindows = StartWithWindowsCheckbox.IsChecked ?? false;
        }
    }
}
