using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SlimShady.Configuration;
using SlimShady.UI;
using SlimShadyCore;
using SlimShadyCore.Monitors;
using SlimShadyCore.Utilities;

namespace SlimShady
{
    // TODO
    // - (W8 4 SZARKY TO DESIGN APP) Implemenent 'manage shortcuts' setting --> shows shortcuts list so far
    // - Add Time feed interface
    // - Implemenent 'manage profiles' setting
    // - https://github.com/MahApps/MahApps.Metro --> http://kawinski.net/szarky/share/22-04-22_24.png
    // - Make Brightness changes smoother (up to 5% per second? and 1% when there is less than 10% left)
    // - window structure { window -> list managers in tabs } { manager -> list monitors vertical stackpanel }
    // - Add refresh & apply buttons in HW manager to refresh monitor settings to real, or force set values from the slimshady 
    //       (maybe recurring task that checks for app/real values difference and asks whether refresh/reapply)
    public partial class MainWindow : Window
    {
        // ReSharper disable once NotAccessedField.Local - It would be otherwise garbage collected. ShortcutsMgr is standalone
        private readonly SlimApp slimMain;
        private SlimCore slimCore => slimMain.core;
        private ConfigurationNew slimConfig => slimMain.Config;

        private Dictionary<string, TabItem> MonitorManagersById { get; set; } = new Dictionary<string, TabItem>();

        public MainWindow()
        {
            slimMain = new SlimApp();
            
            using (NkTrace trace = NkLogger.trace())
            {
                InitializeComponent();
                ConfigurationControl.Core = slimCore;
                
                // Show&Hide just to initialize the window so that other components can just set the position directly without worrying about the initial windows position
                Show();
                Hide();

                // Add monitor sliders
                // TODO: Use ItemsSource with binding on core.ActiveMonitorManagers; See ConfigurationControl and MonitorManagerControl classess for reference
                // This way, we'll be able to dynamically add/remove monitor managers via ConfigurationControl
                foreach (MonitorManager manager in slimCore.ActiveMonitorManagers)
                    AddMonitorManager(manager, slimConfig.GetOrCreateManager(manager));
                
                // Some events
                SizeChanged += (x, y) => Application.Current.Dispatcher.BeginInvoke(new Action(AdjustSize)); // We must delay resize a bit, because the window is not yet resized at the moment of event
                Deactivated += (x, y) => Hide();

                // Add ourselves to the system tray
                System.Windows.Forms.NotifyIcon trayIcon = new System.Windows.Forms.NotifyIcon
                {
                    Text = @"Monitor management",
                    Icon = new System.Drawing.Icon(System.Drawing.SystemIcons.Application, 40, 40),
                    Visible = true
                };
                trayIcon.MouseDown += (x, y) => { if (IsVisible) Hide(); else ShowNearMouse(); };
                Closing += (x, y) => trayIcon.Dispose();

                // Just testing things here. Remove later
                Application.Current.Dispatcher.BeginInvoke(new Action(() => { Show(); Activate(); }));

                InitializeActiveManagerSelection();
            }
        }

        private void InitializeActiveManagerSelection()
        {
            // Select active mgr
            string activeManagerId = slimConfig.UI.ActiveMonitorManager;
            if (activeManagerId != null && MonitorManagersById.ContainsKey(activeManagerId))
                MonitorManagerTabControl.SelectedItem = MonitorManagersById[activeManagerId];

            // Setup auto-save new active mgrs
            MonitorManagerTabControl.SelectionChanged += (x, y) =>
            {
                foreach(var entry in MonitorManagersById)
                {
                    if (entry.Value.IsSelected)
                        slimConfig.UI.ActiveMonitorManager = entry.Key;
                }
            };
        }

        private void AddMonitorManager(MonitorManager manager, MonitorManagerConfiguration managerConfig)
        {
            TabItem tab = new TabItem();
            tab.Header = manager.DispName;
            tab.Content = new MonitorManagerControl(manager, managerConfig);
            MonitorManagersById.Add(manager.Id, tab);
            MonitorManagerTabControl.Items.Add(tab);
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Hide();

            foreach(MonitorManager mm in slimCore.ActiveMonitorManagers)
                mm.Dispose();

            //Application.Current.Shutdown();
            Environment.Exit(0); // Workaround for task cancelled exception bug in .NET
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
