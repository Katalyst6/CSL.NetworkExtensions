using System;
using System.Linq;
using NetworkExtensions.Framework;
using NetworkExtensions.NewNetwork.OneWay4L.Meshes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NetworkExtensions.NewNetwork.OneWay1L
{
    public class OneWay4LBuilder : ModPart, INetInfoBuilder
    {
        public int OptionsPriority { get { return 7; } }
        public int Priority { get { return 7; } }

        public string PrefabName { get { return "Oneway Road"; } }
        public string Name { get { return "Medium Oneway"; } }
        public string CodeName { get { return "ONEWAY_4L"; } }
        public string Description { get { return "A four-lane, one-way road suitable for medium traffic moving in one direction. This road is zonable."; } }
        public string UICategory { get { return "RoadsMedium"; } }

        public string ThumbnailsPath { get { return @"NewNetwork\OneWay4L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"NewNetwork\OneWay4L\infotooltip.png"; } }


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
                           (@"NewNetwork\OneWay4L\Textures\Ground_Segment__MainTex.png",
                            @"NewNetwork\OneWay4L\Textures\Ground_Segment__AlphaMap.png"));
                    info.SetNodesTexture(
                        new TexturesSet
                            (@"NewNetwork\OneWay4L\Textures\Ground_Node__MainTex.png",
                             @"NewNetwork\OneWay4L\Textures\Ground_Node__AlphaMap.png"));
                    break;
            }
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            if (version == NetInfoVersion.Ground)
            {
                //info.m_segments[0].m_mesh = (Mesh)Mesh.Instantiate(info.m_segments[0].m_lodMesh);
                //info.m_nodes[0].m_mesh = (Mesh)Mesh.Instantiate(info.m_nodes[0].m_lodMesh);

                info.m_segments[0].m_mesh = OneWay4LSegmentModel.BuildMesh().CreateMesh("OW_4L_Segment0_Grnd");
                info.m_nodes[0].m_mesh = OneWay4LNodeModel.BuildMesh().CreateMesh("OW_4L_Node0_Grnd");
            }
            ///////////////////////////
            // Set up                //
            ///////////////////////////
            var numExtraLanes = 2;
            var vehicleLaneWidth = 3f;
            var vehicleSpeedLimit = 1.1f;
            var pedWidth = 4f;
            var roadHalfWidth = 8f;
            var parkingLaneWidth = 2f;
            // Disabling Parkings and Peds
            //foreach (var l in info.m_lanes)
            //{
            //    if (l.m_laneType == NetInfo.LaneType.Parking)
            //    {
            //        l.m_laneType = NetInfo.LaneType.None;
            //    }
            //}

            // Setting up lanes

            info.m_halfWidth = 12.0f;
            info.m_pavementWidth = pedWidth;

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

            for (int i = 0; i < numExtraLanes; i++)
            {
                vehicleLanes.Add(vehicleLanes.First());
            }

            for (var i = 0; i < pedestrianLanes.Count; i++)
            {
                var multiplier = pedestrianLanes[i].m_position / Math.Abs(pedestrianLanes[i].m_position);
                pedestrianLanes[i].m_width = pedWidth;
                pedestrianLanes[i].m_position = multiplier * (roadHalfWidth + (.5f * pedWidth));

                foreach (var prop in pedestrianLanes[i].m_laneProps.m_props)
                {
                    prop.m_position.x += multiplier * roadHalfWidth;
                }
            }

            for (var i = 0; i < vehicleLanes.Count; i++)
            {
                vehicleLanes[i].m_similarLaneCount = vehicleLanes.Count();
                vehicleLanes[i].m_similarLaneIndex = i;
                vehicleLanes[i].m_width = vehicleLaneWidth;
                vehicleLanes[i].m_speedLimit = vehicleSpeedLimit;
                vehicleLanes[i].m_position = (-1 * ((vehicleLanes.Count / 2f) - .5f) + i) * vehicleLaneWidth;
            }

            for (var i = 0; i < parkingLanes.Count; i++)
            {
                var multiplier = parkingLanes[i].m_position / Math.Abs(parkingLanes[i].m_position);
                parkingLanes[i].m_width = parkingLaneWidth;
                parkingLanes[i].m_position = multiplier * (roadHalfWidth - (parkingLaneWidth / 2));
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
        }
    }
}
