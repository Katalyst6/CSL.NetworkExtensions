using System;
using System.Linq;
using NetworkExtensions.Framework;
using NetworkExtensions.NewNetwork.OneWay1L.Meshes;
using UnityEngine;

namespace NetworkExtensions.NewNetwork.OneWay1L
{
    public class OneWay1LBuilder : ModPart, INetInfoBuilder
    {
        public int OptionsPriority { get { return 1; } }
        public int Priority { get { return 1; } }

        public string PrefabName { get { return "Oneway Road"; } }
        public string Name { get { return "Small Oneway"; } }
        public string CodeName { get { return "ONEWAY_1L"; } }
        public string Description { get { return "A one-lane, one-way road suitable for local traffic moving in one direction. This road is zonable."; } }
        public string UICategory { get { return "RoadsSmall"; } }

        public string ThumbnailsPath { get { return @"NewNetwork\OneWay1L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"NewNetwork\OneWay1L\infotooltip.png"; } }


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
            // 3DModeling            //
            ///////////////////////////
            //if (version == NetInfoVersion.Ground)
            //{
                //info.m_segments[0].m_mesh = (Mesh)Mesh.Instantiate(info.m_segments[0].m_lodMesh);
                //info.m_nodes[0].m_mesh = (Mesh)Mesh.Instantiate(info.m_nodes[0].m_lodMesh);

                //info.m_segments[0].m_mesh = OneWay1Ll.BuildMesh().CreateMesh("OW_1L_Segment0_Grnd");
                //info.m_nodes[0].m_mesh = OneWay1LMeshes.BuildMesh().CreateMesh("OW_1L_Node0_Grnd");
            //}

            if (version == NetInfoVersion.Ground)
            {
                info.m_surfaceLevel = 0;

                var segments0 = info.m_segments[0];
                var nodes0 = info.m_nodes[0];

                segments0.m_backwardForbidden = NetSegment.Flags.None;
                segments0.m_backwardRequired = NetSegment.Flags.None;

                segments0.m_forwardForbidden = NetSegment.Flags.None;
                segments0.m_forwardRequired = NetSegment.Flags.None;

                var nodes1 = nodes0.Clone();

                nodes0.m_flagsForbidden = NetNode.Flags.Transition;
                nodes0.m_flagsRequired = NetNode.Flags.None;

                nodes1.m_flagsForbidden = NetNode.Flags.None;
                nodes1.m_flagsRequired = NetNode.Flags.Transition;

                var grndMesh = OneWay1LMeshes.GetGroundData().CreateMesh("ONEWAY_1L_GROUND");
                var grndTransMesh = OneWay1LMeshes.GetGroundTransitionData().CreateMesh("ONEWAY_1L_GROUND_TRS");

                segments0.m_mesh = grndMesh;
                nodes0.m_mesh = grndMesh;
                nodes1.m_mesh = grndTransMesh;

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0, nodes1 };
            }

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetSegmentsTexture(
                        new TexturesSet
                           (@"NewNetwork\OneWay1L\Textures\Ground_Segment__MainTex.png",
                            @"NewNetwork\OneWay1L\Textures\Ground_Segment__AlphaMap.png"));
                    info.SetNodesTexture(
                        new TexturesSet
                            (@"NewNetwork\OneWay1L\Textures\Ground_Node__MainTex.png",
                             @"NewNetwork\OneWay1L\Textures\Ground_Node__AlphaMap.png"));
                    break;
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_halfWidth = 5.0f;
            info.m_pavementWidth = 2f;
            // Disabling Parkings and Peds
            //foreach (var l in info.m_lanes)
            //{
            //    if (l.m_laneType == NetInfo.LaneType.Parking)
            //    {
            //        l.m_laneType = NetInfo.LaneType.None;
            //    }
            //}

            // Setting up lanes

            var parkingLanes = info.m_lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.Parking)
                .ToList();

            var vehicleLanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .Where(l => l.m_laneType != NetInfo.LaneType.Pedestrian)
                .Where(l => l.m_laneType != NetInfo.LaneType.Parking)
                .ToList();

            var pedestrianLanes = info.m_lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToList();

            var vehicleLane = vehicleLanes[0];
            var parkingLane = parkingLanes[0];
            vehicleLanes[1].m_laneType = NetInfo.LaneType.None;
            parkingLanes[1].m_laneType = NetInfo.LaneType.None;

            vehicleLane.m_width = 3.5f;
            vehicleLane.m_verticalOffset = -0.3f;
            vehicleLane.m_position = -1.25f;
            vehicleLane.m_speedLimit *= 0.7f;

            parkingLane.m_width = 2.5f;
            parkingLane.m_verticalOffset = -0.3f;
            parkingLane.m_position = 1.75f;

            var roadHalfWidth = 3f;
            var pedWidth = 2f;

            for (var i = 0; i < pedestrianLanes.Count; i++)
            {
                var multiplier = pedestrianLanes[i].m_position / Math.Abs(pedestrianLanes[i].m_position);
                pedestrianLanes[i].m_width = pedWidth;
                pedestrianLanes[i].m_position =  multiplier * (roadHalfWidth + (.5f * pedWidth));
                
                foreach (var prop in pedestrianLanes[i].m_laneProps.m_props)
                {
                    prop.m_position.x += multiplier * roadHalfWidth;
                }
            }

            var onewayRoadInfo = ToolsCSL.FindPrefab<NetInfo>("Oneway Road");

            if (version == NetInfoVersion.Ground)
            {
                var playerNetAI = info.GetComponent<PlayerNetAI>();
                var orPlayerNetAI = onewayRoadInfo.GetComponent<PlayerNetAI>();
                if (playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = orPlayerNetAI.m_constructionCost * 2 / 3;
                    playerNetAI.m_maintenanceCost = orPlayerNetAI.m_maintenanceCost * 2 / 3;
                }
            }
            else // Same as the original oneway
            {

            }

            //var roadBaseAI = info.GetComponent<RoadBaseAI>();

            //if (roadBaseAI != null)
            //{
            //    roadBaseAI.m_highwayRules = true;
            //    roadBaseAI.m_trafficLights = false;
            //}

            //var roadAI = info.GetComponent<RoadAI>();

            //if (roadAI != null)
            //{
            //    roadAI.m_enableZoning = false;
            //}

            //info.SetHighwayProps(highwayInfo);
            //info.TrimHighwayProps();


        }
    }
}
