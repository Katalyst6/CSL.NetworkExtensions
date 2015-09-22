using System;
using System.Collections.Generic;
using System.Linq;
using NetworkExtensions.Framework;
using NetworkExtensions.Menus;

namespace NetworkExtensions.NewNetwork.OneWay4L
{
    public class OneWay4LBuilder : ActivablePart, INetInfoBuilder
    {
        public int OptionsPriority { get { return 9; } }
        public int Priority { get { return 30; } }

        public string PrefabName { get { return VanillaNetInfos.ONEWAY_2L; } }
        public string Name { get { return "Oneway4L"; } }
        public string CodeName { get { return "SMALL_ONEWAY_4L"; } }
        public string DisplayName { get { return "Small Four-Lane Oneway"; } }
        public string Description { get { return "A four-lane one-way road without parkings spaces. Supports medium traffic."; } }
        public string UICategory { get { return AdditionnalMenus.ROADS_SMALL_HV; } }

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
            // 3DModeling            //
            ///////////////////////////
            if (version == NetInfoVersion.Ground)
            {
                var segments0 = info.m_segments[0];
                var nodes0 = info.m_nodes[0];

                segments0.m_forwardRequired = NetSegment.Flags.None;
                segments0.m_forwardForbidden = NetSegment.Flags.None;
                segments0.m_backwardRequired = NetSegment.Flags.None;
                segments0.m_backwardForbidden = NetSegment.Flags.None;
                segments0.SetMeshes
                    (@"NewNetwork\SmallHeavyRoads\Meshes\Ground.obj",
                     @"NewNetwork\SmallHeavyRoads\Meshes\Ground_LOD.obj");

                nodes0.SetMeshes
                    (@"NewNetwork\SmallHeavyRoads\Meshes\Ground.obj",
                     @"NewNetwork\SmallHeavyRoads\Meshes\Ground_LOD.obj");

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0 };
            }

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetSegmentsTexture(
                        new TexturesSet
                           (@"NewNetwork\OneWay4L\Textures\Ground_Segment__MainTex.png",
                            @"NewNetwork\OneWay4L\Textures\Ground_Segment__AlphaMap.png"),
                        new TexturesSet
                           (@"NewNetwork\OneWay4L\Textures\Ground_SegmentLOD__MainTex.png",
                            @"NewNetwork\OneWay4L\Textures\Ground_SegmentLOD__AlphaMap.png",
                            @"NewNetwork\OneWay4L\Textures\Ground_SegmentLOD__XYS.png"));
                    break;
            }


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_class = owRoadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL4L_ROAD);
            info.m_pavementWidth = 2;

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
            const float outerlanePosition = 3.7f;
            const float innerlanePosition = 1.3f;

            for (int i = 0; i < 4; i++)
            {
                var lane = templateLane.Clone(string.Format("Carlane {0}", i + 1));
                lane.m_similarLaneIndex = i;
                lane.m_similarLaneCount = 4;

                switch (i)
                {
                    case 0: lane.m_position = -outerlanePosition; break;
                    case 1: lane.m_position = -innerlanePosition; break;
                    case 2: lane.m_position = innerlanePosition; break;
                    case 3: lane.m_position = outerlanePosition; break;
                }

                if (i == 3)
                {
                    lane.m_allowStop = true;
                    lane.m_stopOffset = 1f;
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
                    playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 125 / 10; // 25% increase
                    playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 125 / 10; // 25% increase
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