using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using ColossalFramework;
using NetworkExtensions.Framework;

namespace NetworkExtensions
{
    public class Options
    {
        private const string FILENAME = "Options.xml";

        private static Options s_instance;
        public static Options Instance 
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = Load();
                    s_instance.UpdateAndSave();
                }

                return s_instance;
            }
        }

        public readonly IDictionary<string, bool> PartsEnabled = new Dictionary<string, bool>();

        {
            var partName = part.GetSerializableName();

            var isEnabled = true;
            if (PartsEnabled.ContainsKey(partName))
            {
                isEnabled = PartsEnabled[partName];
            }

            return isEnabled;
        }

        private void UpdateAndSave()
        {
            var isChanged = false;

            foreach (var part in Mod.Parts.OrderBy(p => p.OptionsPriority))
            {
                var partName = part.GetSerializableName();

                if (!PartsEnabled.ContainsKey(partName))
                {
                    PartsEnabled[partName] = part.IsEnabled = true;
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                Save();
            }
        }

        public void Save()
        {
            if (Mod.GetPath() == Mod.PATH_NOT_FOUND)
            {
                return;
            }

            var configPath = Mod.GetPath().Replace("/", "\\") + "\\" + FILENAME;
            var xDoc = new XmlDocument();
            var settings = xDoc.CreateElement("NetworkExtensionsSettings");

            xDoc.AppendChild(settings);

            foreach (var part in PartsEnabled)
            {
                var xmlElem = xDoc.CreateElement(part.Key);
                xmlElem.InnerText = part.Value.ToString();

                settings.AppendChild(xmlElem);
            }

            xDoc.Save(configPath);
        }

        private static Options Load()
        {
            if (Mod.GetPath() == Mod.PATH_NOT_FOUND)
            {
                return new Options();
            }

            var configPath = Mod.GetPath().Replace("/", "\\") + "\\" + FILENAME;
            
            if (!File.Exists(configPath))
            {
                return new Options();
            }

            try
            {
                var configuration = new Options();
                var xDoc = new XmlDocument();
                xDoc.Load(configPath);

                if (xDoc.DocumentElement == null)
                {
                    return configuration;
                }

                foreach (XmlNode node in xDoc.DocumentElement.ChildNodes)
                {
                    var nodeValue = true;

                    if (!bool.TryParse(node.InnerText, out nodeValue))
                    {
                        nodeValue = true;
                    }

                    configuration.PartsEnabled[node.Name] = nodeValue;
                }

                return configuration;
            }
            catch
            {
                return new Options();
            }
        }
    }
}

