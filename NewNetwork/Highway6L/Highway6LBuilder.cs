using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using NetworkExtensions.Framework;
using UnityEngine;

#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions.NewNetwork.Highway6L
{
    public class Highway6LBuilder : INetInfoBuilder
    {
        public int Priority { get { return 16; } }

        public string PrefabName  { get { return "Large Oneway"; } }
        public string Name        { get { return "Large Highway"; } }
        public string CodeName    { get { return "HIGHWAY_6L"; } }
        public string Description { get { return "An highway with six lanes (100% more than the original)."; } }
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

        public void BuildUp (NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetSegmentsTexture(
                        new TexturesSet(
                            @"NewNetwork\Highway6L\Ground\Segments\_MainTex.png",
                            @"NewNetwork\Highway6L\Ground\Segments\_XYSMap.png",
                            @"NewNetwork\Highway6L\Ground\Segments\_APRMap.png"));
                    info.SetNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Ground\Nodes\_MainTex.png", 
                            null,
                            @"NewNetwork\Highway6L\Ground\Nodes\_APRMap.png"),
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Ground\LOD\Nodes\_MainTex.png",
                            @"NewNetwork\Highway6L\Ground\LOD\Nodes\_XYSMap.png",
                            @"NewNetwork\Highway6L\Ground\LOD\Nodes\_APRMap.png"));
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetSegmentsTexture(
                        new TexturesSet(
                            @"NewNetwork\Highway6L\Elevated\Segments\_MainTex.png",
                            null,
                            @"NewNetwork\Highway6L\Elevated\Segments\_APRMap.png"));
                    info.SetNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Elevated\Nodes\_MainTex.png",
                            null,
                            @"NewNetwork\Highway6L\Elevated\Nodes\_APRMap.png"),
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Elevated\LOD\Nodes\_MainTex.png",
                            @"NewNetwork\Highway6L\Elevated\LOD\Nodes\_XYSMap.png",
                            @"NewNetwork\Highway6L\Elevated\LOD\Nodes\_APRMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    info.SetNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway6L\Slope\Nodes\_MainTex.png",
                            null,
                            @"NewNetwork\Highway6L\Slope\Nodes\_APRMap.png"));
                    break;

                case NetInfoVersion.Tunnel:
                    break;

                default:
                    break;
            }



            ///////////////////////////
            // Set up                //
            ///////////////////////////
            var highwayInfo = ToolsCSL.FindPrefab<NetInfo>("Highway");
            
            info.m_createPavement = false;
            info.m_createGravel = (version == NetInfoVersion.Ground);
            info.m_averageVehicleLaneSpeed = 2f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;

            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;

            // Activate with a new mesh
            //info.m_class = highwayInfo.m_class;

            // Test 
            //info.m_surfaceLevel = 0;


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
                //l.m_verticalOffset = 0f;
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
                roadAI.m_trafficLights = false;
            }

            info.SetHighwayProps(highwayInfo);
            info.TrimHighwayProps();
        }
    }
}
