using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using NetworkExtensions.Framework;
using NetworkExtensions.NewNetwork.Highway6L.Meshes;
using UnityEngine;

#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions.NewNetwork.Highway6L
{
    public class Highway6LBuilder : INetInfoBuilder, INetInfoModifier
    {
        public int Priority { get { return 14; } }

        public string PrefabName  { get { return "Large Oneway"; } }
        public string Name        { get { return "Large Highway"; } }
        public string CodeName    { get { return "HIGHWAY_6L"; } }
        public string Description { get { return "A six-lane, one-way road suitable for very high and dense traffic between metropolitan areas. Lanes going the opposite direction need to be built separately. Highway does not allow zoning next to it!"; } }
        public string UICategory  { get { return "RoadsHighway"; } }

        public string ThumbnailsPath  { get { return @"NewNetwork\Highway6L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"NewNetwork\Highway6L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
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
                    return PrefabName + " Road Tunnel";
                case NetInfoVersion.Slope:
                    return PrefabName + " Road Slope";
                default:
                    throw new NotImplementedException();
            }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var highwayInfo = ToolsCSL.FindPrefab<NetInfo>("Highway");


            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetSegmentsTexture(
                        new TexturesSet(
                            @"NewNetwork\Highway6L\Textures\Ground_Segment__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Ground_Segment__APRMap.png"));
                    info.SetNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Textures\Ground_Node__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Ground_Node__APRMap.png"),
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Textures\Ground_NodeLOD__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Ground_NodeLOD__APRMap.png",
                            @"NewNetwork\Highway6L\Textures\Ground_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Textures\Elevated_Node__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Elevated_Node__APRMap.png"));
                        // Lets leave the crossings there until we have a fix
                        //new TexturesSet
                        //   (@"NewNetwork\Highway6L\Textures\Elevated_NodeLOD__MainTex.png",
                        //    @"NewNetwork\Highway6L\Textures\Elevated_NodeLOD__APRMap.png",
                        //    @"NewNetwork\Highway6L\Textures\Elevated_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    info.SetNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Textures\Slope_Node__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Slope_Node__APRMap.png"),
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Textures\Slope_NodeLOD__MainTex.png",
                            @"NewNetwork\Highway6L\Textures\Slope_NodeLOD__APRMap.png",
                            @"NewNetwork\Highway6L\Textures\Slope_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Tunnel:
                    break;
            }


            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            if (version == NetInfoVersion.Ground)
            {
                info.m_surfaceLevel = 0;
                //info.m_class = highwayInfo.m_class;
                info.m_segments[0].m_mesh = (Mesh)Mesh.Instantiate(info.m_segments[0].m_lodMesh);
                info.m_nodes[0].m_mesh = (Mesh)Mesh.Instantiate(info.m_nodes[0].m_lodMesh);

                info.m_segments[0].m_mesh.Setup(Highway6LSegmentModel.BuildMesh(), "HW_6L_Segment0_Grnd");
                info.m_nodes[0].m_mesh.Setup(Highway6LNodeModel.BuildMesh(), "HW_6L_Node0_Grnd");
            }


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_createPavement = (version != NetInfoVersion.Ground);
            info.m_createGravel = (version == NetInfoVersion.Ground);
            info.m_averageVehicleLaneSpeed = 2f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;

            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;


            // Disabling Parkings and Peds
            foreach (var l in info.m_lanes)
            {
                switch (l.m_laneType)
                {
                    case NetInfo.LaneType.Parking:
                        l.m_laneType = NetInfo.LaneType.None;
                        break;
                    case NetInfo.LaneType.Pedestrian:
                        l.m_laneType = NetInfo.LaneType.None;
                        break;
                }
            }

            // Setting up lanes
            var vehiculeLanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToArray();
            var nbLanes = vehiculeLanes.Count(); // Supposed to be 6

            const float laneWidth = 2f; // TODO: Make it 2.5 with new texture
            const float laneWidthPad = 1f;
            const float laneWidthTotal = laneWidth + laneWidthPad;
            var positionStart = (laneWidthTotal * ((1f - nbLanes) / 2f));

            for (int i = 0; i < vehiculeLanes.Length; i++)
            {
                var l = vehiculeLanes[i];
                l.m_allowStop = false;
                l.m_speedLimit = 2f;

                if (version == NetInfoVersion.Ground)
                {
                    l.m_verticalOffset = 0f;
                }

                l.m_width = laneWidthTotal;
                l.m_position = positionStart + i * laneWidthTotal;
            }


            if (version == NetInfoVersion.Ground)
            {
                var hwPlayerNetAI = highwayInfo.GetComponent<PlayerNetAI>();
                var playerNetAI = info.GetComponent<PlayerNetAI>();

                if (hwPlayerNetAI != null && playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost * 2;
                    playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost * 2;
                }
            }
            else // Same as the original oneway
            {

            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_highwayRules = true;
                roadBaseAI.m_trafficLights = false;
            }

            var roadAI = info.GetComponent<RoadAI>();

            if (roadAI != null)
            {
                roadAI.m_enableZoning = false;
            }

            info.SetHighwayProps(highwayInfo);
            info.TrimHighwayProps();
        }

        public void ModifyExistingNetInfo()
        {
            var highwayRampInfo = ToolsCSL.FindPrefab<NetInfo>("HighwayRamp");
            highwayRampInfo.m_UIPriority = highwayRampInfo.m_UIPriority + 1;
        }
    }
}
