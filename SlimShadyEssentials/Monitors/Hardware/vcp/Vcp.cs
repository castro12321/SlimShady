using SlimShadyCore.Utilities;
using System;
using System.Runtime.InteropServices;

namespace SlimShadyEssentials.Monitors.Hardware.vcp
{
    public class Vcp
    {
        public static byte CODE_INPUT_SOURCE = 0x60;
        public static string CODE_INPUT_SOURCE_STR = CODE_INPUT_SOURCE.ToString("X");

        private static class Natives
        {
            public enum MC_VCP_CODE_TYPE
            {
                MC_MOMENTARY,
                MC_SET_PARAMETER
            };

            [DllImport("dxva2.dll", SetLastError = true)]
            public static extern bool GetVCPFeatureAndVCPFeatureReply(IntPtr hMonitor, byte vcpCode, ref MC_VCP_CODE_TYPE codeType, ref uint value, ref uint maxValue);

            [DllImport("dxva2.dll", SetLastError = true)]
            public static extern bool SetVCPFeature(IntPtr hMonitor, byte vcpCode, uint value);

            public static bool GetInputSource(IntPtr hMonitor, ref uint inputSource, ref uint inputSourceMaxVal)
            {
                MC_VCP_CODE_TYPE codeType = MC_VCP_CODE_TYPE.MC_MOMENTARY;
                return GetVCPFeatureAndVCPFeatureReply(hMonitor, CODE_INPUT_SOURCE, ref codeType, ref inputSource, ref inputSourceMaxVal);
            }

            public static bool SetInputSource(IntPtr hMonitor, uint inputSource)
            {
                return SetVCPFeature(hMonitor, CODE_INPUT_SOURCE, inputSource);
            }
        }

        internal static InputSource GetInputSource(IntPtr handle)
        {
            using (NkTrace trace = NkLogger.dbgTrace())
            {
                uint val = 0, maxVal = 0;
                Natives.GetInputSource(handle, ref val, ref maxVal);
                InputSource ret = InputSourceExt.FromInt(val);
                trace.appendExit("input source: " + ret);
                return ret;
            }
        }

        public static void SetInputSource(IntPtr handle, InputSource value)
        {
            using (NkTrace trace = NkLogger.trace("input source: " + value))
            {
                if(!Natives.SetInputSource(handle, value.ToInt()))
                {
                    trace.appendErrorExit("Failed");
                    throw new Exception("SetInputSource('" + value + "') failed");
                }
            }
        }
    }
}
