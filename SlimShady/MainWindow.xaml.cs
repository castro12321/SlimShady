using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using SlimShady.MonitorManagers;
namespace SlimShady
{
    // TODO
    // - (W8 4 SZARKY TO DESIGN APP) Implemenent 'manage shortcuts' setting --> shows shortcuts list so far
    // - Add Time feed interface
    // - Implemenent 'manage profiles' setting
    // - https://github.com/MahApps/MahApps.Metro --> http://kawinski.net/szarky/share/22-04-22_24.png
    // - Add ability to change monitor input http://www.autohotkey.com/board/topic/96884-change-monitor-input-source/
    // - Make Brightness changes smoother (up to 5% per second? and 1% when there is less than 10% left)
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        // ReSharper disable once NotAccessedField.Local - It would be otherwise garbage collected. ShortcutsMgr is standalone
        private Shortcuts shortcutsMgr;
        public static List<MonitorManager> MonitorManagers = new List<MonitorManager> { HardwareMonitorManager.Instance, SoftwareMonitorManager.Instance };
        public static Configuration Config = Configuration.Get();

        public static void Trace(String msg) { Console.WriteLine(@"[TRACE] {0}", msg); }
        public static void Error(String msg) { Console.WriteLine(@"[ERROR] {0}", msg); } // TODO: alter with MessageBox.Show()
        public static void Win32Error(String msg)
        {
            string errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
            Console.WriteLine(errorMessage);
            Error(msg + "; Error: " + errorMessage);
        }

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            
            // Show&Hide just to initialize the window so that other components can just set the position directly without worrying about the initial windows position
            Show();
            Hide();

            // Enable configuration. First load from Config, then enable auto save
            // Doing it in different order, would cause loading 
            Configuration.Get().LoadFromConfig(this); // Load from Config before enabling auto save,
            Configuration.Get().EnableAutoSave(this); // Otherwise, we would 

            // Add monitor sliders
            AddMonitorManager(MonitorManagerTabItemHardware, HardwareMonitorManager.Instance);
            AddMonitorManager(MonitorManagerTabItemGammaRamp, GammaRampMonitorManager.Instance);
            AddMonitorManager(MonitorManagerTabItemWmi, WmiMonitorManager.Instance);
            AddMonitorManager(MonitorManagerTabItemSoftware, SoftwareMonitorManager.Instance);
            HardwareMonitorManager.Instance.LinkMasterToChilds();
            GammaRampMonitorManager.Instance.LinkMasterToChilds();
            WmiMonitorManager.Instance.LinkMasterToChilds();
            SoftwareMonitorManager.Instance.LinkMasterToChilds();

            // Some events
            SizeChanged += (x, y) => Application.Current.Dispatcher.BeginInvoke(new Action(AdjustSize)); // We must delay resize a bit, because the window is not yet resized at the moment of event
            Deactivated += (x, y) => Hide();
            
            // Shortcuts! :D
            shortcutsMgr = new Shortcuts();

            // Add ourselves to the system tray
            System.Windows.Forms.NotifyIcon trayIcon = new System.Windows.Forms.NotifyIcon
            {
                Text = @"Monitor management",
                Icon = new System.Drawing.Icon(System.Drawing.SystemIcons.Application, 40, 40),
                Visible = true
            };
            trayIcon.MouseDown += (x, y) => { if (IsVisible) Hide(); else ShowNearMouse(); };
            Closing += (x, y) => trayIcon.Dispose();

            // Add auto Brightness adjustments using data feed
            DispatcherTimer dispatcher = new DispatcherTimer();
            dispatcher.Tick += (x, y) =>
            {
                if(AutoCheckBox.IsChecked ?? true)
                    Configuration.DataFeed.AdjustMonitorSettings(Configuration.MonitorManager);
            };
            dispatcher.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            dispatcher.Start();

            // Just testing things here. Remove later
            Application.Current.Dispatcher.BeginInvoke(new Action(() => { Show(); Activate(); }));
        }

        private void AddMonitorManager(TabItem tab, MonitorManager manager)
        {
            Trace("");
            Trace("Adding monitor Manager: " + manager.GetType().Name);
            StackPanel tabContent = new StackPanel() { Orientation = Orientation.Vertical };
            tab.Content = tabContent;

            MonitorSliders masterSlider = new MonitorSliders(manager.MasterMonitor);
            tabContent.Children.Add(masterSlider);
            
            int supportBrightness = 0;
            int supportContrast = 0;
            int supportTemperature = 0;
            List<Monitor> monitors = manager.GetMonitorsList();
            foreach (Monitor monitor in monitors)
            {
                Trace("- " + monitor.ToStringDbg());
                MonitorSliders slider = new MonitorSliders(monitor);
                tabContent.Children.Add(slider);
                if (monitor.SupportsBrightness)
                    supportBrightness++;
                if (monitor.SupportsContrast)
                    supportContrast++;
                if (monitor.SupportsTemperature)
                    supportTemperature++;
            }

            // "< 2" or "== 0" whichever works better
            if (supportBrightness == 0)
            {
                masterSlider.BrightnessSliderLabel.Visibility = Visibility.Collapsed;
                masterSlider.BrightnessSlider.Visibility = Visibility.Collapsed;
                masterSlider.BrightnessSliderText.Visibility = Visibility.Collapsed;
            }
            if (supportContrast == 0)
            {
                masterSlider.ContrastSliderLabel.Visibility = Visibility.Collapsed;
                masterSlider.ContrastSlider.Visibility = Visibility.Collapsed;
                masterSlider.ContrastSliderText.Visibility = Visibility.Collapsed;
            }
            if (supportTemperature == 0)
            {
                masterSlider.TemperatureSliderLabel.Visibility = Visibility.Collapsed;
                masterSlider.TemperatureSlider.Visibility = Visibility.Collapsed;
                masterSlider.TemperatureSliderText.Visibility = Visibility.Collapsed;
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShowNearMouse()
        {
            System.Drawing.Point mousePos = System.Windows.Forms.Control.MousePosition;
            //System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromPoint(mousePos);
            
            Left = mousePos.X - Width / 2;
            Top = mousePos.Y - Height;
            AdjustSize();

            Show();
            Activate();
        }

        private void AdjustSize()
        {
            System.Drawing.Point mousePos = System.Windows.Forms.Control.MousePosition;
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromPoint(mousePos);

            if (Top < screen.WorkingArea.Top)
                Top = screen.WorkingArea.Top;
            if (Top + Height > screen.WorkingArea.Bottom)
                Top = screen.WorkingArea.Bottom - Height;
            if (Left < screen.WorkingArea.Left)
                Left = screen.WorkingArea.Left;
            if (Left + Width > screen.WorkingArea.Right)
                Left = screen.WorkingArea.Right - Width;
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            if (MonitorManagerTabControl.Visibility == Visibility.Visible)
            {
                MonitorManagerTabControl.Visibility = Visibility.Collapsed;
                ConfigurationControl.Visibility = Visibility.Visible;
            }
            else
            {
                ConfigurationControl.Visibility = Visibility.Collapsed;
                MonitorManagerTabControl.Visibility = Visibility.Visible;
            }
            AdjustSize();
        }
    }
}
