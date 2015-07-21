using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions.Framework
{
    public static class CloningExtensions
    {
        public static NetInfo Clone(this NetInfo originalPrefab, string newName)
        {
            // TODO: Perf issue with Object.Instantiate
            return CloneII(originalPrefab, newName);

            Debug.Log(String.Format("NExt: Cloning {0} -> {1}", originalPrefab.name, newName));

            var gameObject = Object.Instantiate(originalPrefab.gameObject);
            gameObject.name = newName;

            var info = gameObject.GetComponent<NetInfo>();
            info.m_prefabInitialized = false;

            Debug.Log(String.Format("NExt: Cloning completed {0} -> {1}", originalPrefab.name, newName));

            return info;
        }

        private static readonly string[] s_netInfoUnclonedMembers = new[]
            {
                Selector.NetInfo(ni => ni.gameObject),
                Selector.NetInfo(ni => ni.transform),
                Selector.NetInfo(ni => ni.tag),
                Selector.NetInfo(ni => ni.gameObject),
                Selector.NetInfo(ni => ni.m_class),
                Selector.NetInfo(ni => ni.m_instanceID),
                Selector.NetInfo(ni => ni.m_netAI),
                Selector.NetInfo(ni => ni.m_lanes),
                Selector.NetInfo(ni => ni.m_sortedLanes)
            }
            .Select(s => s.MemberName)
            .ToArray();

        public static NetInfo CloneII(this NetInfo originalPrefab, string newName)
        {
            Debug.Log(String.Format("NExt: Cloning {0} -> {1}", originalPrefab.name, newName));

            var gameObject = new GameObject(newName);
            var info = gameObject.AddComponent<NetInfo>();

            info.CloneMembersFrom(originalPrefab, s_netInfoUnclonedMembers);
            info.name = newName;
            info.m_prefabInitialized = false;

            //info.m_availableIn = originalPrefab.m_availableIn;
            //info.m_color = originalPrefab.m_color;
            //info.m_connectionClass = originalPrefab.m_connectionClass;
            //info.m_intersectClass = originalPrefab.m_intersectClass;
            //info.m_notUsedGuide = originalPrefab.m_notUsedGuide;
            //info.m_placementCursor = originalPrefab.m_placementCursor;
            //info.m_placementStyle = originalPrefab.m_placementStyle;
            //info.m_setCitizenFlags = originalPrefab.m_setCitizenFlags;
            //info.m_setVehicleFlags = originalPrefab.m_setVehicleFlags;
            //info.m_UnlockMilestone = originalPrefab.m_UnlockMilestone;
            //info.m_upgradeCursor = originalPrefab.m_upgradeCursor;

            // TODO: Implement thoses
            //info.m_segments = originalPrefab.m_segments;
            //info.m_nodes = originalPrefab.m_nodes;
            //info.m_lanes = originalPrefab.m_lanes; // BOOM!! Huge perf impact
            //info.m_sortedLanes = originalPrefab.m_sortedLanes;


            // ItemClass ------------------------
            if (originalPrefab.m_class != null)
            {
                var originItemClass = originalPrefab.m_class;
                var itemClass = ScriptableObject.CreateInstance<ItemClass>();
                itemClass.CloneMembersFrom(originItemClass);
                itemClass.name = newName;

                info.m_class = itemClass;
            }


            // InstanceId -----------------------
            {
                info.m_instanceID = InstanceID.Empty;
                info.m_instanceNeeded = true;
            }


            // Lanes -----------------------
            if (originalPrefab.m_lanes != null)
            {
                info.m_lanes = originalPrefab
                    .m_lanes
                    .Select(originalNetInfoLane => originalNetInfoLane.Clone())
                    .ToArray();
            }


            // NetAI ----------------------------
            if (originalPrefab.m_netAI != null)
            {
                info.m_netAI = originalPrefab.m_netAI.Clone(info, newName);
            }

            Debug.Log(String.Format("NExt: Cloning completed {0} -> {1}", originalPrefab.name, newName));
            return info;
        }

        private static readonly string[] s_netAIUnclonedMembers = new[]
            {
                Selector.NetAI(nai => nai.gameObject),
                Selector.NetAI(nai => nai.transform),
                Selector.NetAI(nai => nai.m_info),
                Selector.NetAI(nai => nai.tag),
                Selector.RoadAI(rai => rai.m_elevatedInfo),
                Selector.RoadAI(rai => rai.m_bridgeInfo),
                Selector.RoadAI(rai => rai.m_slopeInfo),
                Selector.RoadAI(rai => rai.m_tunnelInfo),
            }
            .Select(s => s.MemberName)
            .ToArray();

        public static NetAI Clone(this NetAI originNetAI, NetInfo newInfo, string newName)
        {
            Debug.Log(String.Format("NExt: Cloning {0} AI", originNetAI.name));

            var netAI = (NetAI)newInfo.gameObject.AddComponent(originNetAI.GetType());
            netAI.CloneMembersFrom(originNetAI, s_netAIUnclonedMembers);
            netAI.name = newInfo.name;
            netAI.m_info = newInfo;

            netAI.transform.SetParent(newInfo.transform);

            if (originNetAI is RoadAI && netAI is RoadAI)
            {
                (netAI as RoadAI).CloneMembersFrom(originNetAI as RoadAI, newName);
            }

            return netAI;
        }

        private static void CloneMembersFrom(this RoadAI roadAI, RoadAI originalRoadAI, string newName)
        {
            if (originalRoadAI.m_elevatedInfo != null)
            {
                roadAI.m_elevatedInfo = originalRoadAI.m_elevatedInfo.CloneII(newName + " Elevated");
                roadAI.m_elevatedInfo.transform.SetParent(roadAI.transform);
            }

            if (originalRoadAI.m_bridgeInfo != null)
            {
                roadAI.m_bridgeInfo = originalRoadAI.m_bridgeInfo.CloneII(newName + " Bridge");
                roadAI.m_bridgeInfo.transform.SetParent(roadAI.transform);
            }

            if (originalRoadAI.m_slopeInfo != null)
            {
                roadAI.m_slopeInfo = originalRoadAI.m_slopeInfo.CloneII(newName + " Slope");
                roadAI.m_slopeInfo.transform.SetParent(roadAI.transform);
            }

            if (originalRoadAI.m_tunnelInfo != null)
            {
                roadAI.m_tunnelInfo = originalRoadAI.m_tunnelInfo.CloneII(newName + " Tunnel");
                roadAI.m_tunnelInfo.transform.SetParent(roadAI.transform);
            }
        }

        public static NetInfo.Lane Clone(this NetInfo.Lane originalNetInfoLane)
        {
            var netInfoLane = originalNetInfoLane.ShallowClone();

            if (originalNetInfoLane.m_laneProps != null)
            {
                netInfoLane.m_laneProps = originalNetInfoLane.m_laneProps.Clone();
            }
            return netInfoLane;
        }

        public static NetLaneProps Clone(this NetLaneProps originalNetLaneProps)
        {
            var props = ScriptableObject.CreateInstance<NetLaneProps>();
            props.CloneMembersFrom(originalNetLaneProps);

            if (originalNetLaneProps.m_props != null)
            {
                props.m_props = originalNetLaneProps
                    .m_props
                    .Select(propsProp => propsProp.ShallowClone())
                    .ToArray();
            }

            return props;
        }
    }
}
