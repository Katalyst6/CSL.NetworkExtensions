using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using CSL.NetworkExtensions.Framework;
using CSL.NetworkExtensions.Framework.ModParts;

namespace CSL.RoadExtensions.NewNetwork.LargeAvenue6LM
{
    public class LargeAvenue6LMBuilder : ActivablePart, INetInfoBuilder
    {
        public int OptionsPriority { get { return 25; } }
        public int Priority { get { return 25; } }

        public string TemplatePrefabName { get { return NetInfos.Vanilla.AVENUE_4L; } }
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
            #region Template

            var largeRoadInfo = ToolsCSL.FindPrefab<NetInfo>("Large Road");

            #endregion

            // no need for 3DModeling I guess

            #region Texturing

            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetSegmentsTexture(
                        new TexturesSet
                           (@"NewNetwork\LargeAvenue6LM\Textures\Ground_Segment__MainTex.png",
                            @"NewNetwork\LargeAvenue6LM\Textures\Ground_Segment__AlphaMap.png"));
                    break;

/*				case NetInfoVersion.Elevated:
				case NetInfoVersion.Bridge:
					info.SetNodesTexture(
						new TexturesSet
							(@"NewNetwork\LargeAvenue6LM\Textures\Elevated_Node__MainTex.png",
							@"NewNetwork\LargeAvenue6LM\Textures\Elevated_Node__APRMap.png"));
						new TexturesSet
							(@"NewNetwork\LargeAvenue6LM\Textures\Elevated_NodeLOD__MainTex.png",
							@"NewNetwork\LargeAvenue6LM\Textures\Elevated_NodeLOD__APRMap.png",
							@"NewNetwork\LargeAvenue6LM\Textures\Elevated_NodeLOD__XYSMap.png"));
                                    break;

				case NetInfoVersion.Slope:
					info.SetNodesTexture(
						new TexturesSet
							(@"NewNetwork\LargeAvenue6LM\Textures\Slope_Node__MainTex.png",
							@"NewNetwork\LargeAvenue6LM\Textures\Slope_Node__APRMap.png"),
						new TexturesSet
							(@"NewNetwork\LargeAvenue6LM\Textures\Slope_NodeLOD__MainTex.png",
							@"NewNetwork\LargeAvenue6LM\Textures\Slope_NodeLOD__APRMap.png",
							@"NewNetwork\LargeAvenue6LM\Textures\Slope_NodeLOD__XYSMap.png"));
				break;

				case NetInfoVersion.Tunnel:
				break;
*/
            }

            #endregion

			#region Set up

			info.m_availableIn = ItemClass.Availability.All;
//			info.m_createPavement = (version == NetInfoVersion.Slope);
//			info.m_createGravel = (version == NetInfoVersion.Ground);
//			info.m_averageVehicleLaneSpeed = 2f;
			info.m_hasParkingSpaces = false;
			info.m_hasPedestrianLanes = true;

			info.m_UnlockMilestone = largeRoadInfo.m_UnlockMilestone;

			#region Disabling Parkings

			foreach (var l in info.m_lanes)
			{
				switch (l.m_laneType)
				{
					case NetInfo.LaneType.Parking:
						l.m_laneType = NetInfo.LaneType.None;
						break;
				}
			}

			#endregion

			#region Setting up lanes

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

				// I think this part is totally wrong
				switch (i)
				{
					case 1:
					case 2:
					case 3:
						if (lane.m_position < 0)
						{
							lane.m_position += 0.5f;
						}
						else
						{
							lane.m_position += -0.5f;
						}
						break;

					case 4:
					case 5:
					case 6:
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

			info.Setup50LimitProps(); // traffic sign I guess? so there would be need for a new one?

			#endregion

			#region Modify Cost

			if (version == NetInfoVersion.Ground)
			{
				var lrPlayerNetAI = largeRoadInfo.GetComponent<PlayerNetAI>();
				var playerNetAI = info.GetComponent<PlayerNetAI>();

				if (lrPlayerNetAI != null && playerNetAI != null)
				{
					playerNetAI.m_constructionCost = lrPlayerNetAI.m_constructionCost * 11 / 10; // 10% increase
					playerNetAI.m_maintenanceCost = lrPlayerNetAI.m_maintenanceCost * 11 / 10; // 10% increase
				}
			}

			#endregion

			#endregion

		}
    }
}
