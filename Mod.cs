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

        private static IEnumerable<INetInfoBuilder> s_netInfoBuilders;
        public static IEnumerable<INetInfoBuilder> NetInfoBuilders
        {
            get
            {
                if (s_netInfoBuilders == null)
                {
                    var builderType = typeof(INetInfoBuilder);

                    s_netInfoBuilders = typeof(ModInitializer)
                        .Assembly
                        .GetTypes()
                        .Where(t => !t.IsAbstract && !t.IsInterface)
                        .Where(builderType.IsAssignableFrom)
                        .Select(t => (INetInfoBuilder)Activator.CreateInstance(t))
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
                    var builderType = typeof(INetInfoBuilder);

                    s_netInfoModifiers = typeof(ModInitializer)
                        .Assembly
                        .GetTypes()
                        .Where(t => !t.IsAbstract && !t.IsInterface)
                        .Where(builderType.IsAssignableFrom)
                        .Select(t => (INetInfoModifier)Activator.CreateInstance(t))
                        .ToArray();
                }

                return s_netInfoModifiers;
            }
        }
    }
}
