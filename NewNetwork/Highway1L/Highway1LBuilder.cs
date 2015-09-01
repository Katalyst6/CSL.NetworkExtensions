using System;
using System.Linq;
using NetworkExtensions.Framework;
using UnityEngine;

#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions.NewNetwork.Highway1L
{
    public class Highway1LBuilder : ModPart, INetInfoBuilder
    {
        public int OptionsPriority { get { return 30; } }
        public int Priority { get { return 9; } }

        public string PrefabName { get { return "Basic Road"; } }
        public string Name { get { return "Small Rural Highway"; } }
        public string CodeName { get { return "HIGHWAY_1L"; } }
        public string Description { get { return "A two-lane, two-way road suitable for low traffic between rural areas. Highway does not allow zoning next to it!"; } }
        public string UICategory { get { return "RoadsHighway"; } }

        public string ThumbnailsPath { get { return @"NewNetwork\Highway1L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"NewNetwork\Highway1L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; } // TODO: Fix the bugs with the elevated nodes texture for other supported versions
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
            // Template              //
            ///////////////////////////
            var highwayInfo = ToolsCSL.FindPrefab<NetInfo>("Highway");


            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            if (version == NetInfoVersion.Ground)
            {
                info.m_surfaceLevel = 0;
                info.m_class = highwayInfo.m_class.Clone("SmallRuralHighway");

                var segments0 = info.m_segments[0];
                var nodes0 = info.m_nodes[0];

                segments0.m_backwardForbidden = NetSegment.Flags.None;
                segments0.m_backwardRequired = NetSegment.Flags.None;

                segments0.m_forwardForbidden = NetSegment.Flags.None;
                segments0.m_forwardRequired = NetSegment.Flags.None;

                var nodes1 = nodes0.ShallowClone();

                nodes0.m_flagsForbidden = NetNode.Flags.Transition;
                nodes0.m_flagsRequired = NetNode.Flags.None;

                nodes1.m_flagsForbidden = NetNode.Flags.None;
                nodes1.m_flagsRequired = NetNode.Flags.Transition;

                segments0.SetMeshes
                    (@"NewNetwork\Highway2L\Meshes\Ground.obj",
                     @"NewNetwork\Highway2L\Meshes\Ground_LOD.obj");

                nodes0.SetMeshes
                    (@"NewNetwork\Highway2L\Meshes\Ground.obj",
                     @"NewNetwork\Highway2L\Meshes\Ground_HwTrans_LOD.obj");

                nodes1.SetMeshes
                    (@"NewNetwork\Highway2L\Meshes\Ground_Trans.obj",
                     @"NewNetwork\Highway2L\Meshes\Ground_RdTrans_LOD.obj");

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
                           (@"NewNetwork\Highway1L\Textures\Ground_Segment__MainTex.png",
                            @"NewNetwork\Highway1L\Textures\Ground_Segment__AlphaMap.png"));
                    info.SetNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway1L\Textures\Ground_Node__MainTex.png",
                            @"NewNetwork\Highway1L\Textures\Ground_Node__AlphaMap.png"),
                        new TexturesSet
                           (@"NewNetwork\Highway2L\Textures\Ground_NodeLOD__MainTex.png",
                            @"NewNetwork\Highway2L\Textures\Ground_NodeLOD__AlphaMap.png",
                            @"NewNetwork\Highway2L\Textures\Ground_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.SetNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway2L\Textures\Elevated_Node__MainTex.png",
                            @"NewNetwork\Highway2L\Textures\Elevated_Node__AlphaMap.png"));
                    // Lets leave the crossings there until we have a fix
                    //new TexturesSet
                    //   (@"NewNetwork\Highway2L\Textures\Elevated_NodeLOD__MainTex.png",
                    //    @"NewNetwork\Highway2L\Textures\Elevated_NodeLOD__AlphaMap.png",
                    //    @"NewNetwork\Highway2L\Textures\Elevated_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Slope:
                    info.SetNodesTexture(
                        new TexturesSet
                           (@"NewNetwork\Highway2L\Textures\Slope_Node__MainTex.png",
                            @"NewNetwork\Highway2L\Textures\Slope_Node__AlphaMap.png"),
                        new TexturesSet
                           (@"NewNetwork\Highway2L\Textures\Slope_NodeLOD__MainTex.png",
                            @"NewNetwork\Highway2L\Textures\Slope_NodeLOD__AlphaMap.png",
                            @"NewNetwork\Highway2L\Textures\Slope_NodeLOD__XYSMap.png"));
                    break;

                case NetInfoVersion.Tunnel:
                    break;
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_availableIn = ItemClass.Availability.All;
            info.m_createPavement = (version == NetInfoVersion.Slope);
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

            for (int i = 0; i < vehiculeLanes.Length; i++)
            {
                var l = vehiculeLanes[i];
                l.m_allowStop = false;
                l.m_speedLimit = 2f;

                if (version == NetInfoVersion.Ground)
                {
                    l.m_verticalOffset = 0f;

                    if (l.m_position < 0)
                    {
                        l.m_position -= 0.5f;
                    }
                    else
                    {
                        l.m_position += 0.5f;
                    }
                }
            }


            if (version == NetInfoVersion.Ground)
            {
                var hwPlayerNetAI = highwayInfo.GetComponent<PlayerNetAI>();
                var playerNetAI = info.GetComponent<PlayerNetAI>();

                if (hwPlayerNetAI != null && playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost / 2;
                    playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost / 2;
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

            SetHighwayProps(info, highwayInfo);
            info.TrimHighwayProps();
        }

        private static void SetHighwayProps(NetInfo info, NetInfo highwayInfoTemplate)
        {
            var leftHwLane = highwayInfoTemplate
                .m_lanes
                .Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name != null && l.m_laneProps.m_props != null)
                .FirstOrDefault(l => l.m_laneProps.name.ToLower().Contains("left"));

            var rightHwLane = highwayInfoTemplate
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
                            var newProps = ScriptableObject.CreateInstance<NetLaneProps>();
                            newProps.name = "Highway1L Left Props";

                            newProps.m_props = new NetLaneProps.Prop[0];

                            lane.m_laneProps = newProps;
                        }
                    }

                    if (rightHwLane != null)
                    {
                        if (lane.m_laneProps.name.ToLower().Contains("right"))
                        {
                            var newProps = ScriptableObject.CreateInstance<NetLaneProps>();
                            newProps.name = "Highway1L Right Props";

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
    }
}
