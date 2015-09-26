﻿using System;
using System.Linq;
using CSL.NetworkExtensions.Framework;
using CSL.NetworkExtensions.Framework.ModParts;

namespace CSL.RoadExtensions.NewNetwork.OneWay1L
{
    public class OneWay1LBuilder : ActivablePart, INetInfoBuilder
    {
        public int OptionsPriority { get { return 5; } }
        public int Priority { get { return 30; } }

        public string TemplatePrefabName { get { return NetInfos.Vanilla.ONEWAY_2L; } }
        public string Name { get { return "Small Oneway"; } }
        public string DisplayName { get { return "Single-Lane Oneway"; } }
        public string CodeName { get { return "ONEWAY_1L"; } }
        public string Description { get { return "A one-lane, one-way road suitable for local traffic moving in one direction. This road is zonable."; } }
        public string UICategory { get { return "RoadsSmall"; } }

        public string ThumbnailsPath { get { return @"NewNetwork\OneWay1L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"NewNetwork\OneWay1L\infotooltip.png"; } }


        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var onewayRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L);


            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            if (version == NetInfoVersion.Ground)
            {
                var segments0 = info.m_segments[0];
                var nodes0 = info.m_nodes[0];

                segments0.SetMeshes
                    (@"NewNetwork\OneWay1L\Meshes\Ground.obj",
                     @"NewNetwork\OneWay1L\Meshes\Ground_LOD.obj");

                nodes0.SetMeshes
                    (@"NewNetwork\OneWay1L\Meshes\Ground.obj",
                     @"NewNetwork\OneWay1L\Meshes\Ground_LOD.obj");

                info.m_segments = new[] { segments0 };
                info.m_nodes = new[] { nodes0 };
            }

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"NewNetwork\OneWay1L\Textures\Ground_Segment__MainTex.png",
                            @"NewNetwork\OneWay1L\Textures\Ground_Segment__AlphaMap.png"),
                        new TexturesSet
                           (@"NewNetwork\OneWay1L\Textures\Ground_SegmentLOD__MainTex.png",
                            @"NewNetwork\OneWay1L\Textures\Ground_SegmentLOD__AlphaMap.png",
                            @"NewNetwork\OneWay1L\Textures\Ground_SegmentLOD__XYSMap.png"));
                    info.SetAllNodesTexture(
                        new TexturesSet
                            (@"NewNetwork\OneWay1L\Textures\Ground_Node__MainTex.png",
                             @"NewNetwork\OneWay1L\Textures\Ground_Node__AlphaMap.png"),
                        new TexturesSet
                            (@"NewNetwork\OneWay1L\Textures\Ground_NodeLOD__MainTex.png",
                             @"NewNetwork\OneWay1L\Textures\Ground_NodeLOD__AlphaMap.png",
                             @"NewNetwork\OneWay1L\Textures\Ground_NodeLOD__XYSMap.png"));
                    break;
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_class = onewayRoadInfo.m_class.Clone("SmallOneway");
            info.m_hasParkingSpaces = false;
            info.m_halfWidth = 4f;
            info.m_pavementWidth = 2f;

            // Setting up lanes
            var parkingLanes = info.m_lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.Parking)
                .ToList();
            foreach (var parkingLane in parkingLanes)
            {
                parkingLane.m_laneType = NetInfo.LaneType.None;
            }

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
            vehicleLanes[1].m_laneType = NetInfo.LaneType.None;

            vehicleLane.m_width = 3f;
            vehicleLane.m_verticalOffset = -0.3f;
            vehicleLane.m_position = 0f;
            vehicleLane.m_speedLimit *= 0.7f;
            foreach (var prop in vehicleLane.m_laneProps.m_props)
            {
                prop.m_position.x = 0f;
            }

            var roadHalfWidth = 2f;
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
        }
    }
}
