using SlimShadyCore.Utilities;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace SlimShadyEssentials.Monitors.Software
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public partial class GammaRampNatives
    {
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = -20;
        public const int HWND_TOPMOST = -1;
        public const uint SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(int windowHandle, int hWndInsertAfter, int posX, int posY, int width, int height, uint uFlags);
    }

    public class SoftwareMonitorWindowOverlay
    {
        private readonly Window window;
        private DispatcherTimer keepOverlayOnTopWorker = new DispatcherTimer();

        public SoftwareMonitorWindowOverlay(Screen screen)
        {
            using (NkTrace trace = NkLogger.dbgTrace("screen: " + screen))
            {
                window = new Window
                {
                    // Make semi-transparent black window that is hidden from the taskbar
                    WindowStyle = WindowStyle.None,
                    WindowState = WindowState.Normal,
                    ResizeMode = ResizeMode.NoResize,
                    AllowsTransparency = true,
                    Background = Brushes.Black,
                    Opacity = 0.00,
                    ShowInTaskbar = false,

                    // Positioning
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Top = screen.Bounds.Top,
                    Left = screen.Bounds.Left,
                    Width = screen.Bounds.Width,
                    Height = screen.Bounds.Height,
                    Topmost = true,
                };

                // Make sure that window is ALWAYS on top. Even on top of other 'always on top' windows
                // TODO: move it to a thread (only one for all monitors; don't make thread for all monitors!)? It needs to be refreshed as often as possible, so a thread would work best i think
                keepOverlayOnTopWorker.Tick += (x, y) => BringToFrontWithoutActivating(window);
                keepOverlayOnTopWorker.Interval = new TimeSpan(0, 0, 0, 0, 1);
                keepOverlayOnTopWorker.Start();

                // Make OS ignore mouse events over this window (mouse must go through)
                window.SourceInitialized += (x, y) => DisableMouseInteractionFor(window);

                // Everything ready? Let's go
                window.Show();
            }
        }

        public void Close()
        {
            keepOverlayOnTopWorker.Stop();
            window.Close();
        }

        public double Opacity
        {
            set
            {
                window.Opacity = value;
            }
        }

        private static void DisableMouseInteractionFor(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var currentExtendedStyle = GammaRampNatives.GetWindowLong(hwnd, GammaRampNatives.GWL_EXSTYLE);
            GammaRampNatives.SetWindowLong(hwnd, GammaRampNatives.GWL_EXSTYLE, currentExtendedStyle | GammaRampNatives.WS_EX_TRANSPARENT);
        }

        private static void BringToFrontWithoutActivating(Window window)
        {
            var hwndSource = PresentationSource.FromVisual(window) as HwndSource;
            if (hwndSource != null)
            {
                IntPtr handle = hwndSource.Handle;
                GammaRampNatives.SetWindowPos(handle.ToInt32(), GammaRampNatives.HWND_TOPMOST, (int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height, GammaRampNatives.SWP_NOACTIVATE);
            }
        }
    }
}
