using System;
using System.Linq;
using NetworkExtensions.Framework;

namespace NetworkExtensions.SomeNetwork
{
    public class DarkBasicRoadBuilder : INetInfoBuilder
    {
        public int Priority { get { return 1000; } }

        public string PrefabName  { get { return "Basic Road"; } }
        public string Name        { get { return "Dark Basic Road"; } }
        public string CodeName    { get { return "SOME_DB_ROAD"; } }
        public string Description { get { return "A darker version of the basic two-lane road."; } }
        public string UICategory  { get { return "SomeRoads"; } }

        public string ThumbnailsPath { get { return null; } }
        public string InfoTooltipPath { get { return null; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public string GetPrefabName(NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    return PrefabName;
                case NetInfoVersion.Elevated:
                    return PrefabName + " " + NetInfoVersion.Elevated;
                case NetInfoVersion.Bridge:
                    return PrefabName + " " + NetInfoVersion.Bridge;
                case NetInfoVersion.Tunnel:
                    return PrefabName + " " + NetInfoVersion.Tunnel;
                case NetInfoVersion.Slope:
                    return PrefabName + " " + NetInfoVersion.Slope;
                default:
                    throw new NotImplementedException();
            }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            info.m_color.r = 0.3f;
            info.m_color.g = 0.3f;
            info.m_color.b = 0.3f;
        }
    }
}
