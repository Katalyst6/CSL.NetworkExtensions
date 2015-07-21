//using NetworkExtensions.Framework;

//#if DEBUG
//using Debug = NetworkExtensions.Framework.Debug;
//#endif

//namespace NetworkExtensions.NewNetwork.MediumOneWay
//{
//    public class MediumOneWayBuilder : INetInfoBuilder
//    {
//        public int Priority { get { return 99; } }

//        public string PrefabName  { get { return "Medium Road"; } }
//        public string Name        { get { return "Medium OneWay"; } }
//        public string CodeName    { get { return string.Empty; } }
//        public string Description { get { return "A four-lane, oneway road with parking spaces. Supports medium traffic."; } }
//        public string UICategory  { get { return "RoadsMedium"; } }

//        public string ThumbnailsPath  { get { return string.Empty; } }
//        public string InfoTooltipPath { get { return string.Empty; } }

//        public NetInfoVersion SupportedVersions
//        {
//            get { return NetInfoVersion.Ground; }
//        }

//        public void BuildUp(NetInfo info, NetInfoVersion version)
//        {
//            ///////////////////////////
//            // Properties
//            ///////////////////////////
//            info.m_hasBackwardVehicleLanes = false;

//            for (int i = 0; i < info.m_lanes.Length; i++)
//            {
//                if (info.m_lanes[i].m_laneType == NetInfo.LaneType.Vehicle)
//                {
//                    info.m_lanes[i].m_direction = NetInfo.Direction.Forward;
//                    info.m_lanes[i].m_finalDirection = NetInfo.Direction.Forward;
//                }
//            }
//        }
//    }
//}
