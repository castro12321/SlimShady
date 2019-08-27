using SlimShadyCore;
using SlimShadyCore.Monitors;
using SlimShadyCore.Utilities;
using System;
using System.Net;
using System.Text;
using System.Xml;

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
            using (NkTrace trace = NkLogger.trace())
            {
            }
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

            /*XmlElement feed = xml.GetFirstElementById(xml, "feed");
            XmlNode lightValueNode = feed.ChildNodes[2]; // We know that the light level is stored at third position
            String lightValueString = lightValueNode.InnerText;
            return int.Parse(lightValueString);*/
            return 0;
        }

        public void AdjustMonitorSettings(MonitorManager manager)
        {
            using (NkTrace trace = NkLogger.trace())
            {

                NkLogger.info("AMS: " + BaseUrl + "; " + Channel);

                int tsLight;
                try
                {
                    tsLight = GetLastLightLevel();
                }
                catch (Exception e)
                {
                    NkLogger.error("Cannot GetLastLightLevel() from ThingSpeakDataFeed.AdjustMonitorSettings() because: " + e);
                    return;
                }
                int newLight = (int)ThingSpeakLightConverter.Eval(tsLight);

                // Ignore small changes that would cause monitor Brightness flickering. Change the Brightness only past certain difference treshold
                if (Utils.Diff(newLight, lastLight) < 2)
                    return;

                NkLogger.info("Light: " + tsLight + " --> " + newLight);
                lastLight = newLight;

                // TODO: Send event for application to set new brightness
                //manager.MasterMonitor.Brightness    = (int)ThingSpeakLightConverter.Eval(light);

                //Manager.MasterMonitor.Contrast    = monitorSettingsByLightLevel[light].Contrast;
                //Manager.MasterMonitor.Temperature = monitorSettingsByLightLevel[light].Temperature;
            }
        }
    }
}
