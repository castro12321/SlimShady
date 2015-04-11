namespace SlimShady
{
    public class MonitorSettingsEntry
    {
        public int Brightness { get; private set; }
        public int Contrast { get; private set; }
        public int Temperature { get; private set; }

        public MonitorSettingsEntry(int brightness, int contrast, int temperature)
        {
            Brightness = brightness;
            Contrast = contrast;
            Temperature = temperature;
        }
    }
}
