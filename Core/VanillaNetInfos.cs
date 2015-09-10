using System;
using NetworkExtensions.Framework;

namespace NetworkExtensions
{
    public static class VanillaNetInfos
    {
        public const string ROAD_2L = "Basic Road";
        public const string ROAD_6L = "Large Road";

        public const string AVENUE_4L = "Medium Road";

        public const string ONEWAY_2L = "Oneway Road";
        public const string ONEWAY_6L = "Large Oneway";

        public const string HIGHWAY_3L = "Highway";

        public static string GetPrefabName(string groundName, NetInfoVersion version)
        {
            switch (groundName)
            {
                case ROAD_2L:
                case ROAD_6L:

                case AVENUE_4L:

                case ONEWAY_2L:

                case HIGHWAY_3L:
                    switch (version)
                    {
                        case NetInfoVersion.Ground:
                            return groundName;
                        case NetInfoVersion.Elevated:
                        case NetInfoVersion.Bridge:
                        case NetInfoVersion.Tunnel:
                        case NetInfoVersion.Slope:
                            return groundName + " " + version;
                        default:
                            throw new NotImplementedException();
                    }

                case ONEWAY_6L:
                    switch (version)
                    {
                        case NetInfoVersion.Ground:
                            return groundName;
                        case NetInfoVersion.Elevated:
                            return groundName + " " + NetInfoVersion.Elevated;
                        case NetInfoVersion.Bridge:
                            return groundName + " " + NetInfoVersion.Bridge;
                        case NetInfoVersion.Tunnel:
                            return groundName + " Road Tunnel";
                        case NetInfoVersion.Slope:
                            return groundName + " Road Slope";
                        default:
                            throw new NotImplementedException();
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
