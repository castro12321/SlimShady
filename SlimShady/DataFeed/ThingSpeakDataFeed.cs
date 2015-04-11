using System;
using System.Net;
using System.Text;
using System.Xml;
using SlimShady.MonitorManagers;

namespace SlimShady.DataFeed
{
    public class ThingSpeakDataFeed
    {
        public String BaseUrl = null;
        public String Channel = null;

        public static ThingSpeakDataFeed Instance = new ThingSpeakDataFeed();

        private int lastLight = 9999;

        private ThingSpeakDataFeed()
        {
            MainWindow.Trace("ThingSpeakDataFeed ctor");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Last light level from ThingSpeak sent by light sensor (actually sent by Arduino or some other microcontroller that read the value (actually sent by Ethernet or WIFI shield/chip that got the data from microcontroler (actually... nvm))) </returns>
        private int GetLastLightLevel()
        {
            String url = BaseUrl + "/channels/%CHANNEL%/feeds.xml?results=1"
                .Replace("%CHANNEL%", Channel);
            WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 };
            String xmlString = webClient.DownloadString(url);

            xmlString = xmlString.Replace("<feed>", "<feed id=\"feed\">"); // That way we are avoiding traversing the xml tree which results in a shorter code. We can do it because we retrieve only the last feed
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlString);

            XmlElement feed = ConfigurationFile.GetFirstElementById(xml, "feed");
            XmlNode lightValueNode = feed.ChildNodes[2]; // We know that the light level is stored at third position
            String lightValueString = lightValueNode.InnerText;
            return int.Parse(lightValueString);
        }

        public void AdjustMonitorSettings(MonitorManager manager)
        {
            MainWindow.Trace("AMS: " + BaseUrl + "; " + Channel);

            int light;
            try
            {
                light = GetLastLightLevel();
            }
            catch(Exception e)
            {
                MainWindow.Error("Cannot GetLastLightLevel() from ThingSpeakDataFeed.AdjustMonitorSettings() because: " + e);
                return;
            }

            // Ignore small changes that would cause monitor Brightness flickering. Change the Brightness only past certain difference treshold
            if (Math.Abs(light - lastLight) < 2)
                return;

            MainWindow.Trace("Light: " + light + " --> " + ThingSpeakLightConverter.Eval(light));
            lastLight = light;
            manager.MasterMonitor.Brightness    = (int)ThingSpeakLightConverter.Eval(light);
            //Manager.MasterMonitor.Contrast    = monitorSettingsByLightLevel[light].Contrast;
            //Manager.MasterMonitor.Temperature = monitorSettingsByLightLevel[light].Temperature;
        }
    }
}
