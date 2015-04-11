using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace SlimShady.MonitorManagers
{
    [SuppressMessage("ReSharper", "InconsistentNaming")] // Names are copied from WinApi
    public partial class WinApi
    {
        public const int WINAPI_GAMMARAMP_RESULT_SUCCESS = 1;

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern bool ReleaseDC(IntPtr hwnd, IntPtr dc);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);

        [DllImport("gdi32.dll")]
        public static extern bool SetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);

        public const int GammaMax = 0xFFFF;
        public const int GammaBase = 128;
        public const int GammaRampArrayLength = 256;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RAMP
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = GammaRampArrayLength)]
            public UInt16[] Red;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = GammaRampArrayLength)]
            public UInt16[] Green;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = GammaRampArrayLength)]
            public UInt16[] Blue;
        }
    }

    public class GammaRampMonitor : Monitor
    {
        public GammaRampMonitor(String name, MonitorManager manager)
            : base(name, manager)
        {
            //MainWindow.Trace("Creating GRAMP mon; " + screen.DeviceName);
            SupportsBrightness = true;
            MinBrightness = 1;
            MaxBrightness = 256;
            mBrightness = WinApi.GammaBase; // The system is always loaded with full Brightness

            SupportsContrast = true;
            MaxContrast = 255;
            mContrast = 100; // Similarly as mBrightness

            SupportsTemperature = true;
            MinTemperature = 2000;
            MaxTemperature = 12000;
            mTemperature = 3000; // TODO: What's default Temperature?

            try
            {
                WinApi.RAMP ramp = new WinApi.RAMP();
                if (WinApi.GetDeviceGammaRamp(WinApi.GetDC(IntPtr.Zero), ref ramp) != WinApi.WINAPI_GAMMARAMP_RESULT_SUCCESS)
                {
                    MainWindow.Error("Getting gamma ramp failed");
                    SupportsBrightness = false;
                    SupportsContrast = false;
                    SupportsTemperature = false;
                    return;
                }
                mBrightness = ramp.Red[1] - WinApi.GammaBase;
            }
            catch(NotSupportedException)
            {
                SupportsBrightness = false;
            }
        }

        private int mBrightness;
        public override int Brightness
        {
            get { return mBrightness; }
            set
            {
                mBrightness = value;
                int gamma = Lerp(MinBrightness, Math.Min(Brightness, MaxBrightness), Manager.MasterMonitor.Brightness);

                WinApi.RAMP ramp = new WinApi.RAMP() 
                {
                    Red = new ushort[WinApi.GammaRampArrayLength],
                    Green = new ushort[WinApi.GammaRampArrayLength],
                    Blue = new ushort[WinApi.GammaRampArrayLength] 
                };

                for (int i = 1; i < WinApi.GammaRampArrayLength; i++)
                {
                    var val = (ushort)Math.Min(i * (gamma + WinApi.GammaBase), WinApi.GammaMax);
                    ramp.Red[i] = ramp.Blue[i] = ramp.Green[i] = val;
                }

                if (!WinApi.SetDeviceGammaRamp(WinApi.GetDC(IntPtr.Zero), ref ramp))
                    MainWindow.Error("Setting gamma ramp failed");

                NotifyPropertyChanged();
            }
        }

        private int mContrast;
        public override int Contrast
        {
            get { return mContrast; }
            set
            {
                mContrast = value;
                // ...
                NotifyPropertyChanged();
            }
        }

        private int mTemperature;
        public override int Temperature
        {
            get
            {
                return mTemperature;
            }
            set
            {
                mTemperature = value;
                // ...
                NotifyPropertyChanged();
            }
        }
    }
}
