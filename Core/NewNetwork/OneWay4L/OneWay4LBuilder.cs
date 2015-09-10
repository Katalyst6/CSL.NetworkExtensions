using System;
using System.Linq;
using NetworkExtensions.Framework;

namespace NetworkExtensions.NewNetwork.OneWay4L
{
    public class OneWay4LBuilder : ActivablePart, INetInfoBuilder
    {
        public int OptionsPriority { get { return 9; } }
        public int Priority { get { return 32; } }

        public string PrefabName { get { return VanillaNetInfos.ONEWAY_2L; } }
        public string Name { get { return "Small Oneway4L"; } }
        public string CodeName { get { return "SMALL_ONEWAY_4L"; } }
        public string DisplayName { get { return "Small Four-Lane Oneway"; } }
        public string Description { get { return "A four-lane one-way road without parkings spaces. Supports medium traffic."; } }
        public string UICategory { get { return "RoadsSmall"; } }

        public string ThumbnailsPath    { get { return @"NewNetwork\OneWay4L\thumbnails.png"; } }
        public string InfoTooltipPath   { get { return @"NewNetwork\OneWay4L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var owRoadInfo = ToolsCSL.FindPrefab<NetInfo>(VanillaNetInfos.ONEWAY_2L);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetSegmentsTexture(
                        new TexturesSet
                           (@"NewNetwork\OneWay4L\Textures\Ground_Segment__MainTex.png",
                            @"NewNetwork\OneWay4L\Textures\Ground_Segment__AlphaMap.png"));
                    break;
            }


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;

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
                    int closestVehicleLaneId;

                    if (i - 1 >= 0 && vehicleLaneTypes.Contains(vehicleLanes[i - 1].m_laneType))
                    {
                        closestVehicleLaneId = i - 1;
                    }
                    else if (i + 1 < vehicleLanes.Length && vehicleLaneTypes.Contains(vehicleLanes[i + 1].m_laneType))
                    {
                        closestVehicleLaneId = i + 1;
                    }
                    else
                    {
                        continue; // Not supposed to happen
                    }

                    var closestVehicleLane = vehicleLanes[closestVehicleLaneId];

                    SetLane(lane, closestVehicleLane);

                    if (lane.m_position < 0)
                    {
                        lane.m_position += 0.3f;
                    }
                    else
                    {
                        lane.m_position -= 0.3f;
                    }
                }
                else
                {
                    if (lane.m_position < 0)
                    {
                        lane.m_position += 0.2f;
                    }
                    else
                    {
                        lane.m_position -= 0.2f;
                    }
                }
            }


            if (version == NetInfoVersion.Ground)
            {
                var brPlayerNetAI = owRoadInfo.GetComponent<PlayerNetAI>();
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

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
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
                closestLane.m_stopOffset = 0;
            }
            if (newLane.m_allowStop)
            {
                if (newLane.m_position < 0)
                {
                    newLane.m_stopOffset = -1f;
                }
                else
                {
                    newLane.m_stopOffset = 1f;
                }
            }

            newLane.m_laneType = closestLane.m_laneType;
            newLane.m_similarLaneCount = closestLane.m_similarLaneCount = closestLane.m_similarLaneCount + 1;
            newLane.m_similarLaneIndex = closestLane.m_similarLaneIndex + 1;
            newLane.m_speedLimit = closestLane.m_speedLimit;
            newLane.m_vehicleType = closestLane.m_vehicleType;
            newLane.m_verticalOffset = closestLane.m_verticalOffset;
            newLane.m_width = closestLane.m_width;

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
