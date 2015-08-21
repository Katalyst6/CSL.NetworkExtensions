using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColossalFramework.IO;
using ColossalFramework.Steamworks;
using ICities;
using NetworkExtensions.Framework;
using UnityEngine;

#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions
{
    public partial class Mod : IUserMod
    {
        private const UInt64 WORKSHOP_ID = 478820060;

        public const string NEXT_OBJECT_NAME = "Network Extensions";
        public const string NEXT_CATEGORY_NAME = "Network Extensions";

        public const string ROAD_NETCOLLECTION = "Road";
        public const string NEWROADS_NETCOLLECTION = "NewRoad";

        public string Name
        {
            get { return "Network Extensions"; }
        }

        public string Description
        {
            get { return "An addition of highways and roads"; }
        }

        public const string PATH_NOT_FOUND = "NOT_FOUND";

        private static string s_path = null;
        public static string GetPath()
        {
            if (s_path == null)
            {
                s_path = CheckForPath();

                if (s_path != PATH_NOT_FOUND)
                {
                    Debug.Log("NExt: Mod path " + s_path);
                }
                else
                {
                    Debug.Log("NExt: Path not found");
                }
            }

            return s_path;
        }

        private static string CheckForPath()
        {
            // 1. Check Local path (CurrentUser\Appdata\Local\Colossal Order\Cities_Skylines\Addons\Mods)
            var localPath = DataLocation.modsPath + "/NetworkExtensions";
            if (Directory.Exists(localPath))
            {
                return localPath;
            }

            // 2. Check Steam
            foreach (var mod in Steam.workshop.GetSubscribedItems())
            {
                if (mod.AsUInt64 == WORKSHOP_ID)
                {
                    var workshopPath = Steam.workshop.GetSubscribedItemPath(mod);
                    if (Directory.Exists(workshopPath))
                    {
                        return workshopPath;
                    }
                }
            }

            // 3. Check Cities Skylines files folder
            var csFolderPath = DataLocation.gameContentPath + "/Mods/NetworkExtensions";
            if (Directory.Exists(csFolderPath))
            {
                return csFolderPath;
            }

            return PATH_NOT_FOUND;
        }

        private static IEnumerable<IModPart> s_parts;
        public static IEnumerable<IModPart> Parts
        {
            get
            {
                if (s_parts == null)
                {
                    var builderType = typeof(IModPart);

                    s_parts = typeof(ModInitializer)
                        .Assembly
                        .GetTypes()
                        .Where(t => !t.IsAbstract && !t.IsInterface)
                        .Where(builderType.IsAssignableFrom)
                        .Select(t =>
                        {
                            var part = (IModPart) Activator.CreateInstance(t);
                            part.IsEnabled = Options.Instance.IsPartEnabled(part);
                            return part;
                        })
                        .ToArray();
                }

                return s_parts;
            }
        }

        private static IEnumerable<INetInfoBuilder> s_netInfoBuilders;
        public static IEnumerable<INetInfoBuilder> NetInfoBuilders
        {
            get
            {
                if (s_netInfoBuilders == null)
                {
                    s_netInfoBuilders = s_parts
                        .Where(p => p.IsEnabled)
                        .OfType<INetInfoBuilder>()
                        .ToArray();
                }

                return s_netInfoBuilders;
            }
        }

        private static IEnumerable<INetInfoModifier> s_netInfoModifiers;
        public static IEnumerable<INetInfoModifier> NetInfoModifiers
        {
            get
            {
                if (s_netInfoModifiers == null)
                {
                    s_netInfoModifiers = s_parts
                        .Where(p => p.IsEnabled)
                        .OfType<INetInfoModifier>()
                        .ToArray();
                }

                return s_netInfoModifiers;
            }
        }
    }
}
