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
                        @"NewNetwork\Highway6L\Elevated\Segments\_MainTex.png",
                        null,
                        @"NewNetwork\Highway6L\Elevated\Segments\_APRMap.png");
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

            SetHighwayProps(info, highwayInfo);
            TrimProps(info);
        }

        private static void SetHighwayProps(NetInfo info, NetInfo highwayInfo)
        {
            var leftHwLane = highwayInfo
                .m_lanes
                .Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name != null && l.m_laneProps.m_props != null)
                .FirstOrDefault(l => l.m_laneProps.name.ToLower().Contains("left"));

            var rightHwLane = highwayInfo
                .m_lanes
                .Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name != null && l.m_laneProps.m_props != null)
                .FirstOrDefault(l => l.m_laneProps.name.ToLower().Contains("right"));

            foreach (var lane in info.m_lanes)
            {
                if (lane.m_laneProps != null && lane.m_laneProps.name != null)
                {
                    if (leftHwLane != null)
                    {
                        if (lane.m_laneProps.name.ToLower().Contains("left"))
                        {
                            Debug.Log(string.Format("NExt: Setting {0} props", lane.m_laneProps.name));

                            var newProps = ScriptableObject.CreateInstance<NetLaneProps>();
                            newProps.name = "Highway6L Left Props";

                            newProps.m_props = leftHwLane
                                .m_laneProps
                                .m_props
                                .Select(p => p.ShallowClone())
                                .ToArray();

                            lane.m_laneProps = newProps;
                        }
                    }

                    if (rightHwLane != null)
                    {
                        if (lane.m_laneProps.name.ToLower().Contains("right"))
                        {
                            Debug.Log(string.Format("NExt: Setting {0} props", lane.m_laneProps.name));

                            var newProps = ScriptableObject.CreateInstance<NetLaneProps>();
                            newProps.name = "Highway6L Right Props";

                            newProps.m_props = rightHwLane
                                .m_laneProps
                                .m_props
                                .Select(p => p.ShallowClone())
                                .ToArray();

                            lane.m_laneProps = newProps;
                        }
                    }
                }
            }
        }

        private static void TrimProps(NetInfo info)
        {
            var randomProp = ToolsCSL.FindPrefab<PropInfo>("Random Street Prop", false);
            var streetLight = ToolsCSL.FindPrefab<PropInfo>("New Street Light", false);
            var manhole = ToolsCSL.FindPrefab<PropInfo>("Manhole", false);

            if (randomProp == null)
            {
               return;
            }

            foreach (var laneProps in info.m_lanes.Select(l => l.m_laneProps).Where(lpi => lpi != null))
            {
                var remainingProp = new List<NetLaneProps.Prop>();

                foreach (var prop in laneProps.m_props)
                {
                    if (prop.m_prop != null)
                    {
                        if (prop.m_prop == randomProp)
                        {
                            continue;
                        }

                        if (prop.m_prop == manhole)
                        {
                            continue;
                        }

                        if (prop.m_prop == streetLight &&
                            laneProps.name.Contains("Left"))
                        {
                            continue;
                        }

                        remainingProp.Add(prop);
                    }
                }

                laneProps.m_props = remainingProp.ToArray();
            }
        }
    }
}
