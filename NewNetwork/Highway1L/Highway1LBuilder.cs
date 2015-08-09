// TODO

//using System;
//using System.Linq;
//using NetworkExtensions.Framework;

//namespace NetworkExtensions.NewNetwork.Highway1L
//{
//    public class Highway1LBuilder : INetInfoBuilder
//    {
//        public int Priority { get { return 9; } }

//        public string PrefabName  { get { return "Basic Road"; } }
//        public string Name        { get { return "Small Rural Highway"; } }
//        public string CodeName    { get { return "HIGHWAY_1L"; } }
//        public string Description { get { return "A two-lane, two-way road suitable for low traffic between rural areas. Highway does not allow zoning next to it!"; } }
//        public string UICategory  { get { return "RoadsHighway"; } }

//        public string ThumbnailsPath { get { return @"NewNetwork\Highway1L\thumbnails.png"; } }
//        public string InfoTooltipPath { get { return @"NewNetwork\Highway1L\infotooltip.png"; } }

//        public NetInfoVersion SupportedVersions
//        {
//            get { return NetInfoVersion.All; }
//        }

//        public string GetPrefabName(NetInfoVersion version)
//        {
//            switch (version)
//            {
//                case NetInfoVersion.Ground:
//                    return PrefabName;
//                case NetInfoVersion.Elevated:
//                    return PrefabName + " " + NetInfoVersion.Elevated;
//                case NetInfoVersion.Bridge:
//                    return PrefabName + " " + NetInfoVersion.Bridge;
//                case NetInfoVersion.Tunnel:
//                    return PrefabName + " " + NetInfoVersion.Tunnel;
//                case NetInfoVersion.Slope:
//                    return PrefabName + " " + NetInfoVersion.Slope;
//                default:
//                    throw new NotImplementedException();
//            }
//        }

//        public void BuildUp(NetInfo info, NetInfoVersion version)
//        {
//            ///////////////////////////
//            // Texturing             //
//            ///////////////////////////
//            //switch (version)
//            //{
//            //    case NetInfoVersion.Ground:
//            //        info.SetSegmentsTexture(
//            //            new TexturesSet
//            //               (@"NewNetwork\Highway1L\Textures\Ground_Segment__MainTex.png",
//            //                @"NewNetwork\Highway1L\Textures\Ground_Segment__AlphaMap.png"));
//            //        info.SetNodesTexture(
//            //            new TexturesSet
//            //               (@"NewNetwork\Highway1L\Textures\Ground_Segment__MainTex.png",
//            //                @"NewNetwork\Highway1L\Textures\Ground_Node__AlphaMap.png"),
//            //            new TexturesSet
//            //                (@"NewNetwork\Highway2L\Textures\Ground_NodeLOD__MainTex.dds"));
//            //        break;

//            //    case NetInfoVersion.Elevated:
//            //    case NetInfoVersion.Bridge:
//            //        info.SetNodesTexture(
//            //            new TexturesSet
//            //               (@"NewNetwork\Highway2L\Textures\Elevated_Node__MainTex.png",
//            //                @"NewNetwork\Highway2L\Textures\Elevated_Node__AlphaMap.png"),
//            //            new TexturesSet
//            //               (@"NewNetwork\Highway2L\Textures\Elevated_NodeLOD__MainTex.png",
//            //                @"NewNetwork\Highway2L\Textures\Elevated_NodeLOD__AlphaMap.png"));
//            //        break;

//            //    case NetInfoVersion.Slope:
//            //        info.SetNodesTexture(
//            //            new TexturesSet
//            //               (@"NewNetwork\Highway2L\Textures\Slope_Node__MainTex.png",
//            //                @"NewNetwork\Highway2L\Textures\Slope_Node__AlphaMap.png"));
//            //        break;

//            //    case NetInfoVersion.Tunnel:
//            //        break;
//            //}


//            ///////////////////////////
//            // Set up                //
//            ///////////////////////////
//            var highwayInfo = ToolsCSL.FindPrefab<NetInfo>("Highway");

//            info.m_createPavement = (version != NetInfoVersion.Ground);;
//            info.m_createGravel = (version == NetInfoVersion.Ground);
//            info.m_averageVehicleLaneSpeed = 2f;
//            info.m_hasParkingSpaces = false;
//            info.m_hasPedestrianLanes = false;

//            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;

//            // Activate with a new mesh
//            //info.m_class = highwayInfo.m_class;

//            // Test 
//            //info.m_surfaceLevel = 0;


//            // Disabling Parkings and Peds
//            foreach (var l in info.m_lanes)
//            {
//                switch (l.m_laneType)
//                {
//                    case NetInfo.LaneType.Parking:
//                        l.m_laneType = NetInfo.LaneType.None;
//                        break;
//                    case NetInfo.LaneType.Pedestrian:
//                        l.m_laneType = NetInfo.LaneType.None;
//                        break;
//                }
//            }

//            // Setting up lanes
//            var vehiculeLanes = info.m_lanes
//                .Where(l => l.m_laneType != NetInfo.LaneType.None)
//                .OrderBy(l => l.m_similarLaneIndex)
//                .ToArray();

//            for (int i = 0; i < vehiculeLanes.Length; i++)
//            {
//                var l = vehiculeLanes[i];
//                l.m_allowStop = false;
//                l.m_speedLimit = 2f;
//            }


//            if (version == NetInfoVersion.Ground)
//            {
//                var hwPlayerNetAI = highwayInfo.GetComponent<PlayerNetAI>();
//                var playerNetAI = info.GetComponent<PlayerNetAI>();

//                if (hwPlayerNetAI != null && playerNetAI != null)
//                {
//                    playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost / 2;
//                    playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost / 2;
//                }
//            }
//            else // Same as the original oneway
//            {

//            }

//            var roadBaseAI = info.GetComponent<RoadBaseAI>();

//            if (roadBaseAI != null)
//            {
//                roadBaseAI.m_highwayRules = true;
//                roadBaseAI.m_trafficLights = false;
//            }

//            var roadAI = info.GetComponent<RoadAI>();

//            if (roadAI != null)
//            {
//                roadAI.m_enableZoning = false;
//                roadAI.m_trafficLights = false;
//            }

//            info.SetHighwayProps(highwayInfo);
//            info.TrimHighwayProps();
//        }
//    }
//}
