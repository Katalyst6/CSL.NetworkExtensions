using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColossalFramework.IO;
using ColossalFramework.Steamworks;
using ICities;
using NetworkExtensions.Framework;

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

        public static string GetPath()
        {
            var localPath = DataLocation.modsPath + "/NetworkExtensions";
            //Debug.Log("NExt: " + localPath);
            if (Directory.Exists(localPath))
            {
                //Debug.Log("NExt: Local path exists, looking for assets here: " + localPath);
                return localPath;
            }

            foreach (var mod in Steam.workshop.GetSubscribedItems())
            {
                if (mod.AsUInt64 == WORKSHOP_ID)
                {
                    var workshopPath = Steam.workshop.GetSubscribedItemPath(mod);
                    //Debug.Log("NExt: Workshop path: " + workshopPath);
                    return workshopPath;
                }
            }

            return ".";
        }

        private static IEnumerable<INExtModPart> s_parts;
        public static IEnumerable<INExtModPart> Parts
        {
            get
            {
                if (s_parts == null)
                {
                    var builderType = typeof(INExtModPart);

                    s_parts = typeof(ModInitializer)
                        .Assembly
                        .GetTypes()
                        .Where(t => !t.IsAbstract && !t.IsInterface)
                        .Where(builderType.IsAssignableFrom)
                        .Select(t => (INExtModPart)Activator.CreateInstance(t))
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
                        .OfType<INetInfoModifier>()
                        .ToArray();
                }

                return s_netInfoModifiers;
            }
        }
    }
}
