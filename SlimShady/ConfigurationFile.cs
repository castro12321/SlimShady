using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Security;

namespace SlimShady
{
    public class ConfigurationFile
    {
        private static String CONFIG_ROOT_ELEMENT = "Configuration";
        private readonly String filePath;
        private readonly XmlDocument mainXmlDocument;
        private readonly XmlElement configRoot;

        public ConfigurationFile(String filePath)
        {
            try
            {
                this.filePath = filePath;
                mainXmlDocument = new XmlDocument();

                // Create empty Config if needed
                if (!File.Exists(filePath))
                {
                    Console.WriteLine(@"no Config file. Creating... {0}", mainXmlDocument);
                    XmlDeclaration xmlDeclaration = mainXmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                    XmlElement root = mainXmlDocument.DocumentElement;
                    mainXmlDocument.InsertBefore(xmlDeclaration, root);
                    mainXmlDocument.AppendChild(mainXmlDocument.CreateElement(CONFIG_ROOT_ELEMENT));
                    mainXmlDocument.Save(filePath);
                }

                // Load the Config from file
                using (XmlReader reader = XmlReader.Create(new StringReader(File.ReadAllText(filePath))))
                {
                    mainXmlDocument.Load(reader);
                    var rootElements = mainXmlDocument.GetElementsByTagName(CONFIG_ROOT_ELEMENT);
                    if (rootElements.Count != 1)
                    {
                        MainWindow.Error("Current configuration is either missing root <" + CONFIG_ROOT_ELEMENT + "> element or contains too many of them. If you cannot fix the file, just delete it and we will recreate configuration");
                        throw new ConfigurationErrorsException("Invalid configuration file format.");
                    }
                    configRoot = rootElements[0] as XmlElement;
                }
            }
            catch(Exception e)
            {
                MainWindow.Error("Cannot read Config: " + e);
            }
        }

        public static XmlElement GetFirstElementById(XmlNode node, String id)
        {
            XmlElement found;

            if(node.Attributes != null && node.Attributes["id"] != null)
            {
                String idAtrribute = node.Attributes["id"].Value;
                if(idAtrribute != null && idAtrribute == id)
                {
                    found = node as XmlElement;
                    if (found == null)
                        MainWindow.Error("Configuration.GetElementById() found node is not XmlElement");
                    return found;
                }
            }

            foreach(XmlNode childNode in node.ChildNodes)
            {
                if (childNode == node) // Don't ask me why. It happens for this line: <?xml version="1.0" encoding="UTF-8"?>
                    break;

                found = GetFirstElementById(childNode, id);
                if (found != null)
                    return found;
            }

            return null;
        }

        public bool Has(String key)
        {
            return GetFirstElementById(mainXmlDocument, key) != null;
        }

        public String GetContent(String elementId, String defaultIfNotFound)
        {
            XmlElement found = GetFirstElementById(mainXmlDocument, elementId);
            if (found == null)
                return defaultIfNotFound;
            return found.InnerXml;
        }

        private void SetContent(String elementId, String content, bool createIfDoesntExist = true)
        {
            XmlElement found = GetFirstElementById(mainXmlDocument, elementId);
            if (found == null)
            {
                if (createIfDoesntExist)
                {
                    found = mainXmlDocument.CreateElement("value");
                    found.SetAttribute("id", elementId);
                    configRoot.AppendChild(found);
                }
                else
                    return;
            }
            found.InnerXml = SecurityElement.Escape(content);
        }

        private void Save()
        {
            try
            {
                mainXmlDocument.Save(filePath);
            }
            catch(IOException e)
            {
                MainWindow.Error("Configuration.Save() Error: " + e.Message);
            }
        }

        public override string ToString()
        {
            return mainXmlDocument.OuterXml;
        }

        public bool GetBool(String key, bool defaultValue)
        {
            String value = GetContent(key, defaultValue.ToString()).ToLowerInvariant();
            if (value == "1" || value == "true")
                return true;
            if (value == "0" || value == "false")
                return false;
            throw new InvalidDataException();
        }

        public int GetInt(String key, int defaultValue)
        {
            String value = GetContent(key, defaultValue.ToString()).ToLowerInvariant();
            return int.Parse(value);
        }

        public void Set(String key, object value)
        {
            SetContent(key, ""+value);
            Save();
        }
    }
}