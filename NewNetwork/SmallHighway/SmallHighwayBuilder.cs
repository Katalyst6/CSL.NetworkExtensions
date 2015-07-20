//using System.Linq;
//using ColossalFramework.UI;
//using NetworkExtensions.Extensions;
//using NetworkExtensions.Framework;

//namespace NetworkExtensions.NewNetwork.SmallHighway
//{
//    public class SmallHighwayBuildery : INetInfoBuilder
//    {
//        public int Priority { get { return 1; } }

//        public string PrefabName  { get { return "Oneway Road"; } }
//        public string Name        { get { return "Small Highway"; } }
//        public string CodeName    { get { return "SMALL_HIGHWAY"; } }
//        public string Description { get { return "An highway with two lanes. 100% as high as the original, but only 66% as way."; } }
//        public string UICategory  { get { return "RoadsHighway"; } }

//        public string ThumbnailsPath { get { return @"NewNetwork\SmallHighway\thumbnails.png"; } }

//        public bool WithElevatedVersion { get { return true; } }
//        public bool WithBridgeVersion   { get { return true; } }


//        public void SetTextures(NetInfo info, NetInfoSubType subType)
//        {
//            info.ReplaceSegmentTexture(
//                @"NewNetwork\SmallHighway\Ground\Segments\_MainTex.png",
//                null,
//                null);
//            //info.ReplaceNodeTexture(
//            //    @"NewNetwork\XLargeHighway\Nodes\_MainTex.png",
//            //    @"NewNetwork\XLargeHighway\Nodes\_APRMap.png");
//        }

//        public void AsyncBuildUp(NetInfo info)
//        {
//            info.m_createPavement = false;
//            info.m_createGravel = true;
//            info.m_averageVehicleLaneSpeed = 2f;
//            info.m_hasParkingSpaces = false;
//            info.m_hasPedestrianLanes = false;
            
//            // Test 
//            info.m_surfaceLevel = 0;

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

//            var roadBaseAI = info.m_netAI as RoadBaseAI;
//            if (roadBaseAI != null)
//            {
//                roadBaseAI.m_highwayRules = true;
//            }

//            var roadAI = info.m_netAI as RoadAI;
//            if (roadAI != null)
//            {
//                roadAI.m_enableZoning = false;
//            }
//        }
//    }
//}
