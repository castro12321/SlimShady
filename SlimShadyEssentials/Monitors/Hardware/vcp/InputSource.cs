namespace SlimShadyEssentials.Monitors.Hardware.vcp
{
    public enum InputSource
    {
        /// <summary>
        /// Not specified by the VCP docs, but returned on communication failure with a monitor
        /// </summary>
        Unknown = 0x00,
        AnalogRGB1 = 0x01,
        AnalogRGB2,
        DVI1,
        DVI2,
        Composite1,
        Composite2,
        SVideo1,
        SVideo2,
        Tuner1,
        Tuner2,
        Tuner3,
        Component1,
        Component2,
        Component3,
        DisplayPort1,
        DisplayPort2,
        HDMI1,
        HDMI2,
    }

    public static class InputSourceExt
    {
        public static InputSource FromHex(string hex)
        {
            int val = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return (InputSource)val;
        }

        public static InputSource FromInt(uint val)
        {
            return (InputSource)val;
        }

        public static uint ToInt(this InputSource ths)
        {
            return (uint)ths;
        }
    }
}
