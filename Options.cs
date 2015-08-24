using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using ColossalFramework;
using NetworkExtensions.Framework;
using UnityEngine;

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
                    s_instance.Save();
                }

                return s_instance;
            }
        }

        public readonly IDictionary<string, bool> PartsEnabled = new Dictionary<string, bool>();

        public bool IsPartEnabled(IModPart part)
        {
            var partName = part.GetSerializableName();

            var isEnabled = true;
            if (PartsEnabled.ContainsKey(partName))
            {
                isEnabled = PartsEnabled[partName];
            }

            return isEnabled;
        }

        public void Save()
        {
            Debug.Log("NExt: Saving config");
            if (Mod.GetPath() == Mod.PATH_NOT_FOUND)
            {
                return;
            }

            var configPath = Path.Combine(Mod.GetPath(), FILENAME);
            Debug.Log(string.Format("NExt: Saving config at {0}", configPath));

            try
            {
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
            catch (Exception ex)
            {
                Debug.Log(string.Format("NExt: Crashed saving config at {0} {1}", configPath, ex));
            }
        }

        private static Options Load()
        {
            Debug.Log("NExt: Loading config" );
            if (Mod.GetPath() == Mod.PATH_NOT_FOUND)
            {
                return new Options();
            }

            var configPath = Path.Combine(Mod.GetPath(), FILENAME);
            Debug.Log(string.Format("NExt: Loading config at {0}", configPath));
            
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
            catch (Exception ex)
            {
                Debug.Log(string.Format("NExt: Crashed load config at {0} {1}", configPath, ex));
                return new Options();
            }
        }
    }
}
