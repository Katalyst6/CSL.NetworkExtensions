using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Globalization;
using CSL.NetworkExtensions.Framework;
using CSL.NetworkExtensions.Framework.ModParts;
using UnityEngine;

#if DEBUG
using Debug = CSL.NetworkExtensions.Framework.Debug;
#endif

namespace CSL.RoadExtensions.NewNetwork.LargeAvenue6LM
{
    public class LargeAvenue6LMBuilder : ActivablePart, INetInfoBuilder
    {
        public int OptionsPriority { get { return 25; } }
        public int Priority { get { return 25; } }

        public string TemplatePrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "Large Avenue M"; } }
        public string DisplayName { get { return "Six-Lane Road with Median"; } }
        public string CodeName { get { return "LARGEAVENUE_6LM"; } }
        public string Description { get { return "A six-lane road. Supports heavy traffic."; } }
        public string UICategory { get { return "RoadsLarge"; } }
        
        public string ThumbnailsPath    { get { return @"NewNetwork\LargeAvenue6LM\thumbnails.png"; } }
        public string InfoTooltipPath   { get { return @"NewNetwork\LargeAvenue6LM\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var largeRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TexturesSet
                           (@"NewNetwork\LargeAvenue6LM\Textures\Ground_Segment__MainTex.png",
                            @"NewNetwork\LargeAvenue6LM\Textures\Ground_Segment__AlphaMap.png"));
                    break;
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_class = largeRoadInfo.m_class.Clone(NetInfoClasses.NEXT_LARGE_ROAD);
            info.m_UnlockMilestone = largeRoadInfo.m_UnlockMilestone;
			info.m_hasParkingSpaces = false;

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

            var nonVehicleLanes = info.m_lanes
                .Where(l => !vehicleLaneTypes.Contains(l.m_laneType))
                .ToArray();

            info.m_lanes = vehicleLanes
                .Union(nonVehicleLanes)
                .ToArray();


            Debug.Log(vehicleLanes.Length.ToString());

            for (var i = 0; i < vehicleLanes.Length; i++)
			{
				var lane = vehicleLanes[i];
				switch (i)
				{
                    default:
                        if (lane.m_position < 0)
                        {
                            lane.m_position += -2.0f;
                        }
                        else
                        {
                            lane.m_position += 2.0f;
                        }
                        break;
                }
			}

			info.Setup50LimitProps(); // traffic sign I guess? so there would be need for a new one?

            if (version == NetInfoVersion.Ground)
            {
                var lrPlayerNetAI = largeRoadInfo.GetComponent<PlayerNetAI>();
                var playerNetAI = info.GetComponent<PlayerNetAI>();

                if (lrPlayerNetAI != null && playerNetAI != null)
                {
                    playerNetAI.m_constructionCost = lrPlayerNetAI.m_constructionCost * 9 / 10; // 10% decrease
                    playerNetAI.m_maintenanceCost = lrPlayerNetAI.m_maintenanceCost * 9 / 10; // 10% decrease
                }

                var lrRoadBaseAI = largeRoadInfo.GetComponent<RoadBaseAI>();
                var roadBaseAI = info.GetComponent<RoadBaseAI>();

                if (lrRoadBaseAI != null && roadBaseAI != null)
                {
                    roadBaseAI.m_noiseAccumulation = lrRoadBaseAI.m_noiseAccumulation;
                    roadBaseAI.m_noiseRadius = lrRoadBaseAI.m_noiseRadius;
                }
            }
        }

        public void ModifyExistingNetInfo()
        {
            var localizedStringsField = typeof(Locale).GetFieldByName("m_LocalizedStrings");
            var locale = SingletonLite<LocaleManager>.instance.GetLocale();
            var localizedStrings = (Dictionary<Locale.Key, string>)localizedStringsField.GetValue(locale);

            var kvp =
                localizedStrings
                .FirstOrDefault(kvpInternal =>
                    kvpInternal.Key.m_Identifier == "NET_TITLE" &&
                    kvpInternal.Key.m_Key == NetInfos.Vanilla.ROAD_6L);

            if (!Equals(kvp, default(KeyValuePair<Locale.Key, string>)))
            {
                localizedStrings[kvp.Key] = "Six-Lane Road with Median";
            }
        }
    }
}
