using System;
using System.Collections.Generic;
using System.Linq;
using NetworkExtensions.Framework;

namespace NetworkExtensions.NewNetwork.OneWay3L
{
    public class OneWay3LBuilder : ActivablePart, INetInfoBuilder
    {
        public int OptionsPriority { get { return 8; } }
        public int Priority { get { return 31; } }

        public string PrefabName { get { return VanillaNetInfos.ONEWAY_2L; } }
        public string Name { get { return "Oneway3L"; } }
        public string CodeName { get { return "ONEWAY_3L"; } }
        public string DisplayName { get { return "Three-Lane Oneway"; } }
        public string Description { get { return "A three-lane one-way road without parkings spaces. Supports medium traffic."; } }
        public string UICategory { get { return "RoadsOW"; } }

        public string ThumbnailsPath    { get { return @"NewNetwork\OneWay3L\thumbnails.png"; } }
        public string InfoTooltipPath   { get { return @"NewNetwork\OneWay3L\infotooltip.png"; } }

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
                           (@"NewNetwork\OneWay3L\Textures\Ground_Segment__MainTex.png",
                            @"NewNetwork\OneWay3L\Textures\Ground_Segment__AlphaMap.png"));
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

            var templateLane = info.m_lanes
                .Where(l =>
                    vehicleLaneTypes.Contains(l.m_laneType))
                .OrderBy(l => l.m_position)
                .First();

            var vehicleLanes = new List<NetInfo.Lane>();
            const float outerlanePosition = 2.8f;

            for (int i = 0; i < 3; i++)
            {
                var lane = templateLane.Clone(string.Format("Carlane {0}", i + 1));
                lane.m_similarLaneIndex = i;
                lane.m_similarLaneCount = 3;

                switch (i)
                {
                    case 0: lane.m_position = -outerlanePosition; break;
                    case 1: lane.m_position = 0f; break;
                    case 2: lane.m_position = outerlanePosition; break;
                }

                if (i == 2)
                {
                    lane.m_allowStop = true;
                    lane.m_stopOffset = 1.9f;
                }

                vehicleLanes.Add(lane);
            }

            var nonVehicleLanes = info.m_lanes
                .Where(l =>
                    !l.m_laneType.HasFlag(NetInfo.LaneType.Parking) &&
                    !vehicleLaneTypes.Contains(l.m_laneType))
                .ToArray();

            var allLanes = new List<NetInfo.Lane>();
            allLanes.AddRange(vehicleLanes);
            allLanes.AddRange(nonVehicleLanes);

            info.m_lanes = allLanes
                .OrderBy(l => l.m_position)
                .ToArray();


            if (version == NetInfoVersion.Ground)
            {
                var owPlayerNetAI = owRoadInfo.GetComponent<PlayerNetAI>();
                var playerNetAI = info.GetComponent<PlayerNetAI>();

                if (owPlayerNetAI != null && playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 11 / 10; // 10% increase
                    playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 11 / 10; // 10% increase
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
    }
}
