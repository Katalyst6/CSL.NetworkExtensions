using System;
using System.Linq;
using NetworkExtensions.Framework;

namespace NetworkExtensions.NewNetwork.SmallAvenue4L
{
    public class SmallAvenue4LBuilder : INetInfoBuilder
    {
        public int Priority { get { return 20; } }

        public string PrefabName { get { return "Basic Road"; } }
        public string Name { get { return "Small Avenue"; } }
        public string CodeName { get { return "SMALLROAD_4L"; } }
        public string Description { get { return "A small road updgrading the parkings into vehicule lanes."; } }
        public string UICategory { get { return "RoadsHighway"; } } // TODO: Validate

        public string ThumbnailsPath { get { return string.Empty; } } // TODO: Validate
        public string InfoTooltipPath { get { return string.Empty; } } // TODO: Validate

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
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
            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetSegmentsTexture(
                        new TexturesSet
                           (@"NewNetwork\SmallAvenue4L\Textures\Ground_Segment__MainTex.png",
                            @"NewNetwork\SmallAvenue4L\Textures\Ground_Segment__AlphaMap.png"));
                    //info.SetNodesTexture(
                    //    new TexturesSet
                    //       (@"NewNetwork\Highway1L\Textures\Ground_Segment__MainTex.png",
                    //        @"NewNetwork\Highway1L\Textures\Ground_Node__AlphaMap.png"),
                    //    new TexturesSet
                    //       (@"NewNetwork\Highway2L\Textures\Ground_NodeLOD__MainTex.png",
                    //        @"NewNetwork\Highway2L\Textures\Ground_NodeLOD__AlphaMap.png",
                    //        @"NewNetwork\Highway2L\Textures\Ground_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    //info.SetNodesTexture(
                    //    new TexturesSet
                    //       (@"NewNetwork\Highway2L\Textures\Elevated_Node__MainTex.png",
                    //        @"NewNetwork\Highway2L\Textures\Elevated_Node__AlphaMap.png"),
                    //    new TexturesSet
                    //       (@"NewNetwork\Highway2L\Textures\Elevated_NodeLOD__MainTex.png",
                    //        @"NewNetwork\Highway2L\Textures\Elevated_NodeLOD__AlphaMap.png",
                    //        @"NewNetwork\Highway2L\Textures\Elevated_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    //info.SetNodesTexture(
                    //    new TexturesSet
                    //       (@"NewNetwork\Highway2L\Textures\Slope_Node__MainTex.png",
                    //        @"NewNetwork\Highway2L\Textures\Slope_Node__AlphaMap.png"),
                    //    new TexturesSet
                    //       (@"NewNetwork\Highway2L\Textures\Slope_NodeLOD__MainTex.png",
                    //        @"NewNetwork\Highway2L\Textures\Slope_NodeLOD__AlphaMap.png",
                    //        @"NewNetwork\Highway2L\Textures\Slope_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Tunnel:
                    break;
            }


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            var basicRoadInfo = ToolsCSL.FindPrefab<NetInfo>("Basic Road");

            info.m_hasParkingSpaces = false;


            foreach (var l in info.m_lanes)
            {
                switch (l.m_laneType)
                {
                    case NetInfo.LaneType.Parking:
                        l.m_laneType = NetInfo.LaneType.None;
                        break;
                }
            }

            // Setting up lanes
            var vehicleLaneTypes = new[]
            {
                NetInfo.LaneType.Vehicle,
                NetInfo.LaneType.PublicTransport,
                NetInfo.LaneType.CargoVehicle,
                NetInfo.LaneType.TransportVehicle
            };

            var vehicleLanes = info.m_lanes
                .Where(l =>
                    l.m_laneType.HasFlag(NetInfo.LaneType.Parking) ||
                    vehicleLaneTypes.Contains(l.m_laneType))
                .OrderBy(l => l.m_position)
                .ToArray();

            for (int i = 0; i < vehicleLanes.Length; i++)
            {
                var lane = vehicleLanes[i];

                if (lane.m_laneType.HasFlag(NetInfo.LaneType.Parking))
                {
                    NetInfo.Lane closestVehicleLane;

                    if (i - 1 >= 0 && vehicleLaneTypes.Contains(vehicleLanes[i - 1].m_laneType))
                    {
                        closestVehicleLane = vehicleLanes[i - 1];
                    }
                    else if (i + 1 < vehicleLanes.Length && vehicleLaneTypes.Contains(vehicleLanes[i + 1].m_laneType))
                    {
                        closestVehicleLane = vehicleLanes[i + 1];
                    }
                    else
                    {
                        Debug.Log("NExt: Limbo lane");
                        continue;
                    }

                    SetLane(lane, closestVehicleLane);

                    if (lane.m_position < 0)
                    {
                        lane.m_position += 0.1f;
                    }
                    else
                    {
                        lane.m_position -= 0.1f;
                    }
                }
                else
                {
                    if (lane.m_position < 0)
                    {
                        lane.m_position += 0.3f;
                    }
                    else
                    {
                        lane.m_position -= 0.3f;
                    }
                }
            }


            if (version == NetInfoVersion.Ground)
            {
                var brPlayerNetAI = basicRoadInfo.GetComponent<PlayerNetAI>();
                var playerNetAI = info.GetComponent<PlayerNetAI>();

                if (brPlayerNetAI != null && playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = brPlayerNetAI.m_constructionCost * 12 / 10; // 20% increase
                    playerNetAI.m_maintenanceCost = brPlayerNetAI.m_maintenanceCost * 12 / 10; // 20% increase
                }
            }
            else // Same as the original basic road specs
            {

            }
        }

        private static void SetLane(NetInfo.Lane newLane, NetInfo.Lane closestLane)
        {
            newLane.m_direction = closestLane.m_direction;
            newLane.m_finalDirection = closestLane.m_finalDirection;
            newLane.m_allowConnect = closestLane.m_allowConnect;
            newLane.m_allowStop = closestLane.m_allowStop;
            if (closestLane.m_allowStop)
            {
                closestLane.m_allowStop = false;
            }

            newLane.m_laneType = closestLane.m_laneType;
            newLane.m_similarLaneCount = closestLane.m_similarLaneCount = closestLane.m_similarLaneCount + 1;
            newLane.m_similarLaneIndex = closestLane.m_similarLaneIndex + 1;
            newLane.m_speedLimit = closestLane.m_speedLimit;
            newLane.m_stopOffset = closestLane.m_stopOffset;
            newLane.m_vehicleType = closestLane.m_vehicleType;
            newLane.m_verticalOffset = closestLane.m_verticalOffset;
            newLane.m_width = closestLane.m_width;

            // N.B. Don't modify the position
            //lane.m_position = closestLane.m_position;

            NetLaneProps templateLaneProps;
            if (closestLane.m_laneProps != null)
            {
                templateLaneProps = closestLane.m_laneProps;
            }
            else
            {
                templateLaneProps = new NetLaneProps();
            }

            if (templateLaneProps.m_props == null)
            {
                templateLaneProps.m_props = new NetLaneProps.Prop[0];
            }

            if (newLane.m_laneProps == null)
            {
                newLane.m_laneProps = new NetLaneProps();
            }

            newLane.m_laneProps.m_props = templateLaneProps
                .m_props
                .Select(p => p.ShallowClone())
                .ToArray();
        }
    }
}
