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

        public void BuildUp (NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetSegmentsTexture(
                        @"NewNetwork\Highway6L\Ground\Segments\_MainTex.png",
                        @"NewNetwork\Highway6L\Ground\Segments\_XYSMap.png",
                        @"NewNetwork\Highway6L\Ground\Segments\_APRMap.png");
                    info.SetNodesTexture(
                        @"NewNetwork\Highway6L\Ground\Nodes\_MainTex.png",
                        @"NewNetwork\Highway6L\Ground\Nodes\_APRMap.png");
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetSegmentsTexture(
                        @"NewNetwork\Highway6L\Elevated\Segments\_MainTex.png",
                        null,
                        @"NewNetwork\Highway6L\Elevated\Segments\_APRMap.png");
                    info.SetNodesTexture(
                        @"NewNetwork\Highway6L\Elevated\Nodes\_MainTex.png",
                        @"NewNetwork\Highway6L\Elevated\Nodes\_APRMap.png");
                    break;

                case NetInfoVersion.Slope:
                    info.SetNodesTexture(
                        @"NewNetwork\Highway6L\Slope\Nodes\_MainTex.png",
                        @"NewNetwork\Highway6L\Slope\Nodes\_APRMap.png");
                    break;

                case NetInfoVersion.Tunnel:
                    break;

                default:
                    break;
            }


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_createPavement = false;
            info.m_createGravel = (version == NetInfoVersion.Ground);
            info.m_averageVehicleLaneSpeed = 2f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;

            // Test 
            //info.m_surfaceLevel = 0;


            // Filtering out unwanted lanes
            for (int i = 0; i < info.m_lanes.Length; ++i)
            {
                var l = info.m_lanes[i];
                if (l.m_laneType == NetInfo.LaneType.Parking)
                {
                    l.m_laneType = NetInfo.LaneType.None;
                }
                else if (l.m_laneType == NetInfo.LaneType.Pedestrian)
                {
                    l.m_laneType = NetInfo.LaneType.None;
                }
            }

            // Test 
            //info.m_lanes[i] = null;
            //Tools.RemoveNull(ref info.m_lanes);

            var notNoneLanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .OrderBy(l => l.m_similarLaneIndex)
                .ToArray();
            var nbLanes = notNoneLanes.Count(); // Supposed to be 6

            const float laneWidth = 2f; // TODO: Make it 2.5 with new texture
            const float laneWidthPad = 1f;
            const float laneWidthTotal = laneWidth + laneWidthPad;
            var positionStart = (laneWidthTotal * ((1f - nbLanes) / 2f));

            for (int i = 0; i < notNoneLanes.Length; i++)
            {
                var l = notNoneLanes[i];
                l.m_allowStop = false;
                l.m_speedLimit = 2f;
                //l.m_verticalOffset = 0f;
                l.m_width = laneWidthTotal;
                l.m_position = positionStart + i * laneWidthTotal;
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

            RemoveRandomStreetProp(info);
            SetSpeedLimitSigns(info);
        }

        private void RemoveRandomStreetProp(NetInfo info)
        {
            var rds = ToolsCSL.FindPrefab<PropInfo>("Random Street Prop");

            if (rds == null)
            {
               return;
            }

            foreach (var lp in info.m_lanes.Select(l => l.m_laneProps).Where(lpi => lpi != null))
            {
                for (int i = 0; i < lp.m_props.Length; i++)
                {
                    var p = lp.m_props[i];

                    if (p == null)
                    {
                        continue;
                    }

                    if (p.m_prop == rds)
                    {
                        lp.m_props[i] = null;
                    }
                }

                Tools.RemoveNull(ref lp.m_props);
            }
        }

        private void SetSpeedLimitSigns(NetInfo info)
        {
            var speedLimit65 = ToolsCSL.FindPrefab<PropInfo>("60 Speed Limit");
            var speedLimit100 = ToolsCSL.FindPrefab<PropInfo>("100 Speed Limit");

            if (speedLimit65 == null)
            {
                return;
            }

            if (speedLimit100 == null)
            {
                return;
            }

            foreach (var lp in info.m_lanes.Select(l => l.m_laneProps).Where(lpi => lpi != null))
            {
                for (int i = 0; i < lp.m_props.Length; i++)
                {
                    var p = lp.m_props[i];

                    if (p == null)
                    {
                        continue;
                    }

                    if (p.m_prop == speedLimit65)
                    {
                        var newP = p.ShallowCopy();
                        newP.m_prop = speedLimit100;
                        newP.m_finalProp = speedLimit100;

                        lp.m_props[i] = newP;
                    }
                }
            }
        }
    }
}
