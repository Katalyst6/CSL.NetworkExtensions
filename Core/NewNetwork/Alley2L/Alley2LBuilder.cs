﻿using System;
using System.Linq;
using NetworkExtensions.Framework;
using UnityEngine;

namespace NetworkExtensions.NewNetwork.Alley2L
{
    public class Alley2LBuilder : ModPart, INetInfoBuilder
    {
        public int OptionsPriority { get { return 1; } }
        public int Priority { get { return 40; } }

        public string PrefabName { get { return "Basic Road"; } }
        public string Name { get { return "Alley2L"; } }
        public string DisplayName { get { return "Two-Lane Alley"; } }
        public string CodeName { get { return "Alley_2L"; } }
        public string Description { get { return "A two-lane, tight Alley suitable for neighborhood traffic. This road is zonable."; } }
        public string UICategory { get { return "RoadsSmall"; } }

        public string ThumbnailsPath { get { return @"NewNetwork\Alley2L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"NewNetwork\Alley2L\infotooltip.png"; } }


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

            if (version == NetInfoVersion.Ground)
            {
                info.m_surfaceLevel = 0;

                var segments0 = info.m_segments[0];
                var nodes0 = info.m_nodes[0];

                segments0.SetMeshes
                    (@"NewNetwork\Alley2L\Meshes\Ground.obj",
                     @"NewNetwork\Alley2L\Meshes\Ground_LOD.obj");

                nodes0.SetMeshes
                    (@"NewNetwork\Alley2L\Meshes\Ground.obj",
                     @"NewNetwork\Alley2L\Meshes\Ground_Node_LOD.obj");

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
                           (@"NewNetwork\Alley2L\Textures\Ground_Segment__MainTex.png",
                            @"NewNetwork\Alley2L\Textures\Ground_Segment__AlphaMap.png"));
                    //info.SetNodesTexture(
                    //    new TexturesSet
                    //        (@"NewNetwork\Alley2L\Textures\Ground_Node__MainTex.png",
                    //         @"NewNetwork\Alley2L\Textures\Ground_Node__AlphaMap.png"));
                    break;
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_halfWidth = 4f;
            info.m_pavementWidth = 2f;

            var vehicleLanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .Where(l => l.m_laneType != NetInfo.LaneType.Pedestrian)
                .Where(l => l.m_laneType != NetInfo.LaneType.Parking)
                .ToList();

            var pedestrianLanes = info.m_lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToList();

            var parkingLanes = info.m_lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.Parking)
                .ToList();

            foreach (var parkingLane in parkingLanes)
            {
                parkingLane.m_laneType = NetInfo.LaneType.None;
            }

            var roadHalfWidth = 2f;
            var pedWidth = 2f;

            for (var i = 0; i < vehicleLanes.Count; i++)
            {
                var multiplier = vehicleLanes[i].m_position / Math.Abs(vehicleLanes[i].m_position);
                vehicleLanes[i].m_width = roadHalfWidth;
                vehicleLanes[i].m_position = multiplier * 0.5f * roadHalfWidth;
                vehicleLanes[i].m_speedLimit = 0.5f;
                foreach (var prop in vehicleLanes[i].m_laneProps.m_props)
                {
                    prop.m_position.x =  multiplier * 0.4f;
                }
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

            var onewayRoadInfo = ToolsCSL.FindPrefab<NetInfo>("Basic Road");

            if (version == NetInfoVersion.Ground)
            {
                var playerNetAI = info.GetComponent<PlayerNetAI>();
                var orPlayerNetAI = onewayRoadInfo.GetComponent<PlayerNetAI>();
                if (playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = orPlayerNetAI.m_constructionCost * 1 / 2;
                    playerNetAI.m_maintenanceCost = orPlayerNetAI.m_maintenanceCost * 1 / 2;
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
