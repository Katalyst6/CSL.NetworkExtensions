 //TODO: COMING SOON

using System;
using System.Linq;
using NetworkExtensions.Framework;

namespace NetworkExtensions.NewNetwork.Highway2L
{
    public class Highway2LBuilder : INetInfoBuilder
    {
        public int Priority { get { return 15; } }

        public string PrefabName  { get { return "Oneway Road"; } }
        public string Name        { get { return "Small Highway"; } }
        public string CodeName    { get { return "HIGHWAY_2L"; } }
        public string Description { get { return "An highway with two lanes (33% less than the original)."; } }
        public string UICategory  { get { return "RoadsHighway"; } }

        public string ThumbnailsPath  { get { return string.Empty; } }
        public string InfoTooltipPath { get { return string.Empty; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetSegmentsTexture(
                        new TexturesSet(
                            @"NewNetwork\Highway2L\Ground\Segments\_MainTex.png"));
                    info.SetNodesTexture(
                        new TexturesSet(
                            @"NewNetwork\Highway2L\Ground\Nodes\_MainTex.png"));
                    break;
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                case NetInfoVersion.Tunnel:
                case NetInfoVersion.Slope:
                case NetInfoVersion.All:
                    // TODO: Remove crossings
                    break;
                default:
                    throw new ArgumentOutOfRangeException("version");
            }


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            var highwayInfo = ToolsCSL.FindPrefab<NetInfo>("Highway");

            info.m_createPavement = false;
            info.m_createGravel = (version == NetInfoVersion.Ground);
            info.m_averageVehicleLaneSpeed = 2f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;

            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;

            // Activate with a new mesh
            //info.m_class = highwayInfo.m_class;

            // Test 
            //info.m_surfaceLevel = 0;


            // Disabling Parkings and Peds
            foreach (var l in info.m_lanes)
            {
                switch (l.m_laneType)
                {
                    case NetInfo.LaneType.Parking:
                        l.m_laneType = NetInfo.LaneType.None;
                        break;
                    case NetInfo.LaneType.Pedestrian:
                        l.m_laneType = NetInfo.LaneType.None;
                        break;
                }
            }

            // Setting up lanes
            var vehiculeLanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToArray();

            for (int i = 0; i < vehiculeLanes.Length; i++)
            {
                var l = vehiculeLanes[i];
                l.m_allowStop = false;
                l.m_speedLimit = 2f;
            }


            if (version == NetInfoVersion.Ground)
            {
                var hwPlayerNetAI = highwayInfo.GetComponent<PlayerNetAI>();
                var playerNetAI = info.GetComponent<PlayerNetAI>();

                if (hwPlayerNetAI != null && playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost * 2 / 3;
                    playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost * 2 / 3;
                }
            }
            else // Same as the original oneway
            {

            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_highwayRules = true;
                roadBaseAI.m_trafficLights = false;
            }

            var roadAI = info.GetComponent<RoadAI>();

            if (roadAI != null)
            {
                roadAI.m_enableZoning = false;
                roadAI.m_trafficLights = false;
            }

            info.SetHighwayProps(highwayInfo);
            info.TrimHighwayProps();
        }
    }
}
