

namespace SlimShadyCore
{
    /*
    public class ConfigurationOld
    {
        private readonly String thingspeakFeedName = typeof(ThingSpeakDataFeed).Name;

        private String ConfigKeyThingspeakAddress { get { return thingspeakFeedName + ".address"; } }
        private String ConfigKeyThingspeakAddressDefault { get { return "http://kawinski.net:3000"; } }
        private String ConfigKeyThingspeakChannel { get { return thingspeakFeedName + ".Channel"; } }
        private String ConfigKeyThingspeakChannelDefault { get { return "1"; } }
        private String ConfigKeyThingspeakLightFunction { get { return thingspeakFeedName + ".lightfunction"; } }
        private String ConfigKeyThingspeakLightFunctionDefault { get { return "y = 4*Math.Sqrt(x)"; } }

        public void LoadFromConfig(MainWindow mainWindow)
        {
            ConfigurationControl settingsWindow = mainWindow.ConfigurationControl;

            // Data feed
            settingsWindow.ThingSpeakAddressTextBox.Text = config.GetContent(ConfigKeyThingspeakAddress, ConfigKeyThingspeakAddressDefault);
            settingsWindow.ThingSpeakChannelTextBox.Text = config.GetContent(ConfigKeyThingspeakChannel, ConfigKeyThingspeakChannelDefault);
            settingsWindow.PlotFunction.Text = config.GetContent(ConfigKeyThingspeakLightFunction, ConfigKeyThingspeakLightFunctionDefault);
        }

        public void EnableAutoSave(MainWindow mainWindow)
        {
            ConfigurationControl settingsWindow = mainWindow.ConfigurationControl;

            // Data feed
            settingsWindow.ThingSpeakAddressTextBox.TextChanged += (x, y) => config.Set(ConfigKeyThingspeakAddress, ((TextBox)x).Text);
            settingsWindow.ThingSpeakChannelTextBox.TextChanged += (x, y) => config.Set(ConfigKeyThingspeakChannel, ((TextBox)x).Text);
            settingsWindow.PlotFunction.TextChanged += (x, y) => config.Set(ConfigKeyThingspeakLightFunction, ((TextBox)x).Text);
        }
    }
    */
}