//using System;
//using System.Linq;
//using ColossalFramework;
//using NetworkExtensions.Framework;

//namespace NetworkExtensions.NewNetwork.SmallRoad4L
//{
//    public class SmallRoad4LBuilder : INetInfoBuilder
//    {
//        public int Priority { get { return 15; } }

//        public string PrefabName  { get { return "Small Road"; } } // TODO: Validate
//        public string Name        { get { return "Medium Road"; } }
//        public string CodeName    { get { return "SMALLROAD_4L"; } }
//        public string Description { get { return "A small road updgrading the parkings by vehicule lanes."; } }
//        public string UICategory  { get { return "RoadsHighway"; } } // TODO: Validate

//        public string ThumbnailsPath { get { return string.Empty; } } // TODO: Validate
//        public string InfoTooltipPath { get { return string.Empty; } } // TODO: Validate

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
//            // TODO

//            ///////////////////////////
//            // Set up                //
//            ///////////////////////////
//            var basicRoadInfo = ToolsCSL.FindPrefab<NetInfo>("Small Road");

//            info.m_hasParkingSpaces = false;


//            // Setting up lanes
//            var orderedLanes = info
//                .m_lanes
//                .OrderBy(l => l.m_position)
//                .ToArray();

//            var vehiculeLaneTypes = new[]
//            {
//                NetInfo.LaneType.Vehicle,
//                NetInfo.LaneType.PublicTransport,
//                NetInfo.LaneType.CargoVehicle,
//                NetInfo.LaneType.TransportVehicle
//            };

//            var vehiculeLanes = orderedLanes
//                .Where(l => vehiculeLaneTypes.Contains(l.m_laneType))
//                .ToArray();

//            for (int i = 0; i < orderedLanes.Length; i++)
//            {
//                var lane = orderedLanes[i];

//                if (lane.m_laneType == NetInfo.LaneType.Parking)
//                {
//                    NetInfo.Lane closestVehiculeLane;

//                    if (i - 1 >= 0 && vehiculeLaneTypes.Contains(orderedLanes[i - 1].m_laneType))
//                    {
//                        closestVehiculeLane = orderedLanes[i - 1];
//                    }
//                    else if (i + 1 < orderedLanes.Length && vehiculeLaneTypes.Contains(orderedLanes[i + 1].m_laneType))
//                    {
//                        closestVehiculeLane = orderedLanes[i + 1];
//                    }
//                    else
//                    {
//                        Debug.Log("NExt: Limbo lane");
//                        continue;
//                    }

//                    lane.m_direction = closestVehiculeLane.m_direction;
//                    lane.m_finalDirection = closestVehiculeLane.m_finalDirection;
//                    lane.m_allowConnect = closestVehiculeLane.m_allowConnect;
//                    lane.m_allowStop = closestVehiculeLane.m_allowStop;
//                    lane.m_laneProps = closestVehiculeLane.m_laneProps;
//                    lane.m_laneType = NetInfo.LaneType.Vehicle;
//                    lane.m_position = closestVehiculeLane.m_position;
//                    lane.m_similarLaneCount = closestVehiculeLane.m_similarLaneCount;
//                    lane.m_similarLaneIndex = closestVehiculeLane.m_similarLaneIndex;
//                    lane.m_speedLimit = closestVehiculeLane.m_speedLimit;
//                    lane.m_stopOffset = closestVehiculeLane.m_stopOffset;
//                    lane.m_vehicleType = closestVehiculeLane.m_vehicleType;
//                    lane.m_verticalOffset = closestVehiculeLane.m_verticalOffset;
//                    lane.m_width = closestVehiculeLane.m_width;
//                }
//            }


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
//                var brPlayerNetAI = basicRoadInfo.GetComponent<PlayerNetAI>();
//                var playerNetAI = info.GetComponent<PlayerNetAI>();

//                if (brPlayerNetAI != null && playerNetAI != null)
//                {
//                    playerNetAI.m_constructionCost = brPlayerNetAI.m_constructionCost * 12 / 10; // 20% increase
//                    playerNetAI.m_maintenanceCost = brPlayerNetAI.m_maintenanceCost * 12 / 10; // 20% increase
//                }
//            }
//            else // Same as the original basic road specs
//            {

//            }
//        }
//    }
//}
