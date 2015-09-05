﻿using System;
using System.Linq;
using ColossalFramework;
using NetworkExtensions.Framework;

namespace NetworkExtensions.NewNetwork.MediumAvenue4LTL
{
    public class MediumAvenue4LTLBuilder : ModPart, INetInfoBuilder
    {
        public int OptionsPriority { get { return 21; } }
        public int Priority { get { return 5; } }

        public string PrefabName { get { return "Large Road"; } }
        public string Name { get { return "Medium Avenue TL"; } }
        public string DisplayName { get { return "Four-Lane Road with Turning Lane"; } }
        public string CodeName { get { return "MEDIUMAVENUE_4LTL"; } }
        public string Description { get { return "A four-lane road with turning lanes and parking spaces. Supports medium traffic. Note: The turning lane goes in both direction, collisions might happen!"; } }
        public string UICategory { get { return "RoadsMedium"; } }
        
        public string ThumbnailsPath    { get { return @"NewNetwork\MediumAvenue4LTL\thumbnails.png"; } }
        public string InfoTooltipPath   { get { return @"NewNetwork\MediumAvenue4LTL\infotooltip.png"; } }

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
            // Template              //
            ///////////////////////////
            var mediumRoadInfo = ToolsCSL.FindPrefab<NetInfo>("Medium Road");


            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetSegmentsTexture(
                        new TexturesSet
                           (@"NewNetwork\MediumAvenue4LTL\Textures\Ground_Segment__MainTex.png",
                            @"NewNetwork\MediumAvenue4LTL\Textures\Ground_Segment__AlphaMap.png"),
                        new TexturesSet
                           (@"NewNetwork\MediumAvenue4LTL\Textures\Ground_SegmentLOD__MainTex.png",
                            @"NewNetwork\MediumAvenue4LTL\Textures\Ground_SegmentLOD__AlphaMap.png",
                            @"NewNetwork\MediumAvenue4LTL\Textures\Ground_SegmentLOD__XYSMap.png"));
                    info.SetNodesTexture(
                        new TexturesSet
                           (null,
                            @"NewNetwork\MediumAvenue4LTL\Textures\Ground_Node__AlphaMap.png"));
                    break;
            }


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_class = mediumRoadInfo.m_class.Clone(MediumAvenueHelper.CLASS_NAME);
            info.m_UnlockMilestone = mediumRoadInfo.m_UnlockMilestone;

            // Setting up lanes
            var vehicleLaneTypes = new[]
            {
                NetInfo.LaneType.Vehicle,
                NetInfo.LaneType.PublicTransport,
                NetInfo.LaneType.CargoVehicle,
                NetInfo.LaneType.TransportVehicle
            };

            var vehicleLanes = info.m_lanes
                .Where(l => vehicleLaneTypes.Contains(l.m_laneType))
                .OrderBy(l => l.m_position)
                .ToArray();

            for (var i = 0; i < vehicleLanes.Length; i++)
            {
                var lane = vehicleLanes[i];

                switch (i)
                {
                        // Turning lanes
                    case 3:
                    case 2:
                        lane.m_allowConnect = false;
                        lane.m_speedLimit /= 2f;
                        lane.m_position = 0f;
                        SetupTurningLaneProps(lane);
                        break;

                        // Regular lane
                    case 4:
                    case 1:
                        if (lane.m_position < 0)
                        {
                            lane.m_position += 0.5f;
                        }
                        else
                        {
                            lane.m_position += -0.5f;
                        }
                        break;
                }
            }

            info.Setup50LimitProps();


            if (version == NetInfoVersion.Ground)
            {
                var mrPlayerNetAI = mediumRoadInfo.GetComponent<PlayerNetAI>();
                var playerNetAI = info.GetComponent<PlayerNetAI>();

                if (mrPlayerNetAI != null && playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = mrPlayerNetAI.m_constructionCost * 9 / 10; // 10% decrease
                    playerNetAI.m_maintenanceCost = mrPlayerNetAI.m_maintenanceCost * 9 / 10; // 10% decrease
                } 

                var mrRoadBaseAI = mediumRoadInfo.GetComponent<RoadBaseAI>();
                var roadBaseAI = info.GetComponent<RoadBaseAI>();

                if (roadBaseAI != null)
                {
                    roadBaseAI.m_trafficLights = false;
                }

                if (mrRoadBaseAI != null && roadBaseAI != null)
                {
                    roadBaseAI.m_noiseAccumulation = mrRoadBaseAI.m_noiseAccumulation;
                    roadBaseAI.m_noiseRadius = mrRoadBaseAI.m_noiseRadius;
                }
            }
        }

        private static void SetupTurningLaneProps(NetInfo.Lane lane)
        {
            var isLeftDriving = Singleton<SimulationManager>.instance.m_metaData.m_invertTraffic == SimulationMetaData.MetaBool.True;

            if (lane.m_laneProps == null)
            {
                return;
            }

            if (lane.m_laneProps.m_props == null)
            {
                return;
            }

            var fwd = lane.m_laneProps.m_props.FirstOrDefault(p => p.m_flagsRequired == NetLane.Flags.Forward);
            var left = lane.m_laneProps.m_props.FirstOrDefault(p => p.m_flagsRequired == NetLane.Flags.Left);
            var right = lane.m_laneProps.m_props.FirstOrDefault(p => p.m_flagsRequired == NetLane.Flags.Right);

            if (fwd == null)
            {
                return;
            }

            if (left == null)
            {
                return;
            }

            if (right == null)
            {
                return;
            }


            // Existing props
            //var r0 = NetLane.Flags.Forward; 
            //var r1 = NetLane.Flags.ForwardRight;
            //var r2 = NetLane.Flags.Left;
            //var r3 = NetLane.Flags.LeftForward;
            //var r4 = NetLane.Flags.LeftForwardRight;
            //var r5 = NetLane.Flags.LeftRight;
            //var r6 = NetLane.Flags.Right;

            //var f0 = NetLane.Flags.LeftRight;
            //var f1 = NetLane.Flags.Left;
            //var f2 = NetLane.Flags.ForwardRight;
            //var f3 = NetLane.Flags.Right;
            //var f4 = NetLane.Flags.None;
            //var f5 = NetLane.Flags.Forward;
            //var f6 = NetLane.Flags.LeftForward;


            var newProps = new FastList<NetLaneProps.Prop>();

            //newProps.Add(fwd); // Do we want "Forward" on a turning lane?
            newProps.Add(left);
            newProps.Add(right);

            var fl = left.ShallowClone();
            fl.m_flagsRequired = NetLane.Flags.LeftForward;
            fl.m_flagsForbidden = NetLane.Flags.Right;
            newProps.Add(fl);

            var fr = right.ShallowClone();
            fr.m_flagsRequired = NetLane.Flags.ForwardRight;
            fr.m_flagsForbidden = NetLane.Flags.Left;
            newProps.Add(fr);

            var flr = isLeftDriving ? right.ShallowClone() : left.ShallowClone();
            flr.m_flagsRequired = NetLane.Flags.LeftForwardRight;
            flr.m_flagsForbidden = NetLane.Flags.None;
            newProps.Add(flr);

            var lr = isLeftDriving ? right.ShallowClone() : left.ShallowClone();
            lr.m_flagsRequired = NetLane.Flags.LeftRight;
            lr.m_flagsForbidden = NetLane.Flags.Forward;
            newProps.Add(lr);

            lane.m_laneProps = new NetLaneProps
            {
                name = "TurningLane", 
                m_props = newProps.ToArray()
            };
        }
    }
}
