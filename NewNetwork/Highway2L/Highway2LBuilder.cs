//using System;
//using NetworkExtensions.Framework;

//namespace NetworkExtensions.NewNetwork.Highway2L
//{
//    public class SmallHighwayBuildery : INetInfoBuilder
//    {
//        public int Priority { get { return 15; } }

//        public string PrefabName  { get { return "Oneway Road"; } }
//        public string Name        { get { return "Small Highway"; } }
//        public string CodeName    { get { return "HIGHWAY_2L"; } }
//        public string Description { get { return "An highway with two lanes (33% less than the original)."; } }
//        public string UICategory  { get { return "RoadsHighway"; } }

//        public string ThumbnailsPath  { get { return string.Empty; } }
//        public string InfoTooltipPath { get { return string.Empty; } }

//        public NetInfoVersion SupportedVersions
//        {
//            get { return NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge; }
//        }

//        public void BuildUp(NetInfo info, NetInfoVersion version)
//        {
//            ///////////////////////////
//            // Texturing             //
//            ///////////////////////////
//            switch (version)
//            {
//                case NetInfoVersion.Ground:
//                    info.SetSegmentsTexture(
//                        @"NewNetwork\Highway2L\Ground\Segments\_MainTex.png");
//                    info.SetNodesTexture(
//                        @"NewNetwork\Highway2L\Ground\Nodes\_MainTex.png");
//                    break;
//                case NetInfoVersion.Elevated:
//                case NetInfoVersion.Bridge:
//                case NetInfoVersion.Tunnel:
//                case NetInfoVersion.Slope:
//                case NetInfoVersion.All:
//                    // TODO: Remove crossings
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException("version");
//            }


//            ///////////////////////////
//            // Set up                //
//            ///////////////////////////
//            info.m_createPavement = false;
//            info.m_createGravel = true;
//            info.m_averageVehicleLaneSpeed = 2f;
//            info.m_hasParkingSpaces = false;
//            info.m_hasPedestrianLanes = false;

//            // Test 
//            //info.m_surfaceLevel = 0;

//            for (int i = 0; i < info.m_lanes.Length; ++i)
//            {
//                var l = info.m_lanes[i];
//                l.m_allowStop = false;
//                if (l.m_laneType == NetInfo.LaneType.Vehicle)
//                {
//                    l.m_speedLimit = 2f;
//                }
//                else if (l.m_laneType == NetInfo.LaneType.Pedestrian)
//                {
//                    l.m_laneType = NetInfo.LaneType.None;
//                }
//                else
//                {
//                    info.m_lanes[i] = null;
//                }
//            }
//            Tools.RemoveNull(ref info.m_lanes);


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
//        }
//    }
//}
