using SlimShadyCore.Utilities;
using System;
using System.IO;
using System.Xml.Serialization;

namespace SlimShady.Configuration
{
    public class ConfigurationHelper
    {
        private static string DEFAULT_PATH = "configuration.xml";
        private static ConfigurationSaver saver = new ConfigurationSaver(DEFAULT_PATH);
        private static ConfigurationNew instance = null;

        public static ConfigurationNew GetConfig()
        {
            if(instance == null)
            {
                if (!File.Exists(DEFAULT_PATH))
                    instance = new ConfigurationNew();
                else
                    instance = FromFile(DEFAULT_PATH);
                instance.PropertyChanged += (property, newValue) => Save(instance);
            }
            return instance;
        }

        private static ConfigurationNew FromFile(string path)
        {
            using (NkTrace trace = NkLogger.trace("path: " + path))
            {
                string configRaw = File.ReadAllText(path);
                if (String.IsNullOrWhiteSpace(configRaw))
                    return new ConfigurationNew();

                XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationNew));
                using (TextReader reader = new StringReader(configRaw))
                {
                    ConfigurationNew cfg = (ConfigurationNew)serializer.Deserialize(reader);
                    cfg.InitializeAfterDeserialization();
                    return cfg;
                }
            }
        }

        private static void Save()
        {
            Save(GetConfig());
        }

        private static void Save(ConfigurationNew configuration)
        {
            saver.Save(configuration);
        }

    }
}
