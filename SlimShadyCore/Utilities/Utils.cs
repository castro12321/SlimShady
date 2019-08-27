using Microsoft.Win32;
using SlimShadyCore.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace SlimShadyCore
{
    public class Utils
    {
        // For use in property-changed events like:
        // - public event Utils.NkPropertyChangedEventHandler<string> DispNameChanged;
        // - protected void OnDispNameChanged(string newValue, [CallerMemberName] string propertyName = null) => DispNameChanged?.Invoke(propertyName, newValue);
        // - OnDispNameChanged("Diablo");
        public delegate void NkPropertyChangedEventHandler<T>(string propertyName, T newValue);


        /// <summary>Interpolates value</summary>
        /// <param name="from">Minimal value (when value=0)</param>
        /// <param name="to">Maximal value (when value=100)</param>
        /// <param name="value">Pervent value to interpolate between from and to</param>
        /// <returns>Interpolated number that is between 'from' and 'to' based on 'value'</returns>
        public static int Lerp(int from, int to, int value)
        {
            float fValue = (float)value / 100.0f;
            return (int)((to - from) * fValue + from);
        }

        public static uint Lerp(uint from, uint to, uint value)
        {
            float fValue = (float)value / 100.0f;
            return (uint)((to - from) * fValue + from);
        }

        public static string GetLastWin32Error()
        {
            int errorCode = Marshal.GetLastWin32Error();
            string errorMessage = new Win32Exception(errorCode).Message;
            //Console.WriteLine(errorMessage);
            return "Error(" + errorCode + "): " + errorMessage;
        }

        public static double Diff(double lhs, double rhs)
        {
            return Math.Abs(lhs - rhs);
        }

        public static long Diff(long lhs, long rhs)
        {
            return Math.Abs(lhs - rhs);
        }

        public static double FindClosest(double val, List<double> availableVals)
        {
            return availableVals.Aggregate((closest, next) => Diff(closest, val) < Diff(next, val) ? closest : next);
        }

        public static uint FindClosest(uint val, List<uint> availableVals)
        {
            return availableVals.Aggregate((closest, next) => Diff(closest, val) < Diff(next, val) ? closest : next);
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public class Autorun
        {
            private const string AUTORUN_APPS_KEY = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

            public static bool Get(string title)
            {
                RegistryKey key = GetAutorunKey();
                return key.OpenSubKey(title) != null;
            }

            public static void Set(string assembly, string title, bool autorun = true)
            {
                using (NkTrace trace = NkLogger.dbgTrace("title: " + title + ", autorun? " + autorun))
                {
                    RegistryKey key = GetAutorunKey();
                    if (autorun)
                        key.SetValue(title, assembly);
                    else if (key.OpenSubKey(title) != null)
                        key.DeleteValue(title);
                }
            }

            public static void Delete(string title)
            {
                Set(null, title, false);
            }

            private static RegistryKey GetAutorunKey()
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(AUTORUN_APPS_KEY, true);
                if (key == null)
                    throw new Exception("Can't access 'start with Windows' because registry key '" + AUTORUN_APPS_KEY + "' is not available");
                return key;
            }
        }

        
    }
}
