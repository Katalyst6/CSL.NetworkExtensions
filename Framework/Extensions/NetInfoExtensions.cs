using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions.Framework
{
    internal static class NetInfoExtensions
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

        public static NetInfo CloneII(this NetInfo originalPrefab, string newName)
        {
            Debug.Log(String.Format("NExt: Cloning {0} -> {1}", originalPrefab.name, newName));

            var gameObject = new GameObject(newName);
            var info = gameObject.AddComponent<NetInfo>();

            //info.CopySimpleMembersFrom(originalPrefab);
            //info.CopyMembersFrom(originalPrefab, "gameObject", "transform", "tag", "m_class", "m_instanceID", "m_netAI");
            info.m_availableIn = originalPrefab.m_availableIn;
            //info.m_class = originalPrefab.m_class;
            info.m_color = originalPrefab.m_color;
            info.m_connectionClass = originalPrefab.m_connectionClass;
            info.m_intersectClass = originalPrefab.m_intersectClass;
            //info.m_lanes = originalPrefab.m_lanes; // BOOM!! Huge perf impact
            //info.m_netAI = originalPrefab.m_netAI;
            //info.m_nodes = originalPrefab.m_nodes;
            info.m_notUsedGuide = originalPrefab.m_notUsedGuide;
            info.m_placementCursor = originalPrefab.m_placementCursor;
            info.m_placementStyle = originalPrefab.m_placementStyle;
            //info.m_segments = originalPrefab.m_segments;
            info.m_setCitizenFlags = originalPrefab.m_setCitizenFlags;
            info.m_setVehicleFlags = originalPrefab.m_setVehicleFlags;
            //info.m_sortedLanes = originalPrefab.m_sortedLanes;
            info.m_UnlockMilestone = originalPrefab.m_UnlockMilestone;
            info.m_upgradeCursor = originalPrefab.m_upgradeCursor;


            info.m_class = ScriptableObject.CreateInstance<ItemClass>();
            info.m_class.CopyMembersFrom(originalPrefab.m_class);
            info.m_class.name = newName;

            info.m_instanceID = InstanceID.Empty;
            info.m_instanceNeeded = true;

            if (originalPrefab.m_netAI != null)
            {
                Debug.Log(String.Format("NExt: Cloning {0} AI", originalPrefab.name));

                var originNetAI = originalPrefab.m_netAI;
                var netAI = (NetAI)gameObject.AddComponent(originNetAI.GetType());
                //netAI.CopyMembersFrom(originalPrefab.m_netAI, 
                //    "m_info", 
                //    "gameObject", 
                //    "transform",
                //    "tag"
                //    ,
                //    "m_elevatedInfo",
                //    "m_bridgeInfo",
                //    "m_slopeInfo",
                //    "m_tunnelInfo"
                //    );
                netAI.name = newName;
                netAI.m_info = info;
                netAI.transform.SetParent(info.transform);

                if (originNetAI is RoadAI && netAI is RoadAI)
                {
                    var originalRoadAI = originNetAI as RoadAI;
                    var roadAI = netAI as RoadAI;

                    if (originalRoadAI.m_elevatedInfo != null)
                    {
                        roadAI.m_elevatedInfo = originalRoadAI.m_elevatedInfo.CloneII(newName + " Elevated");
                        roadAI.m_elevatedInfo.transform.SetParent(netAI.transform);
                    }

                    if (originalRoadAI.m_bridgeInfo != null)
                    {
                        roadAI.m_bridgeInfo = originalRoadAI.m_bridgeInfo.CloneII(newName + " Bridge");
                        roadAI.m_bridgeInfo.transform.SetParent(netAI.transform);
                    }

                    if (originalRoadAI.m_slopeInfo != null)
                    {
                        roadAI.m_slopeInfo = originalRoadAI.m_slopeInfo.CloneII(newName + " Slope");
                        roadAI.m_slopeInfo.transform.SetParent(netAI.transform);
                    }

                    if (originalRoadAI.m_tunnelInfo != null)
                    {
                        roadAI.m_tunnelInfo = originalRoadAI.m_tunnelInfo.CloneII(newName + " Tunnel");
                        roadAI.m_tunnelInfo.transform.SetParent(netAI.transform);
                    }
                }
            }

            info.name = newName;
            info.m_prefabInitialized = false;

            Debug.Log(String.Format("NExt: Cloning completed {0} -> {1}", originalPrefab.name, newName));
            return info;
        }

        public static void DisplayLaneProps(this NetInfo info)
        {
            foreach (var propInfo in info.m_lanes
                .Select(l => l.m_laneProps)
                .Where(lpi => lpi != null)
                .SelectMany(lp => lp.m_props)
                .Where(p => p != null)
                .Select(p => p.m_prop)
                .Where(pi => pi != null)
                .Distinct())
            {
                Debug.Log(String.Format("NExt: Prop info name {0}", propInfo.name));
            }
        }

        public static NetInfo SetUICategory(this NetInfo info, string category)
        {
            typeof(NetInfo).GetField("m_UICategory", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(info, category);

            return info;
        }

        public static NetInfo SetSegmentsTexture(this NetInfo info, string mainTexPath, string xysMapPath = null, string aprMapPath = null)
        {
            var mainTex = TextureManager.Instance.GetTexture(mainTexPath);
            var xysMap = TextureManager.Instance.GetTexture(xysMapPath);
            var aprMap = TextureManager.Instance.GetTexture(aprMapPath);

            SetSegmentsTexture(info, mainTex, xysMap, aprMap);

            return info;
        }

        public static NetInfo SetSegmentsTexture(this NetInfo info, Texture mainTex, Texture xysMap, Texture aprMap)
        {
            for (int i = 0; i < info.m_segments.Length; i++)
            {
                var material = new Material(info.m_segments[i].m_material);

                material.SetTexture("_MainTex", mainTex);

                if (xysMap != null)
                {
                    material.SetTexture("_XYSMap", xysMap);
                }

                if (aprMap != null)
                {
                    material.SetTexture("_APRMap", aprMap);
                }

                info.m_segments[i].m_material = material;
            }

            return info;
        }

        public static NetInfo SetNodesTexture(this NetInfo info, string mainTexPath, string aprMapPath = null)
        {
            var mainTex = TextureManager.Instance.GetTexture(mainTexPath);
            var aprMap = TextureManager.Instance.GetTexture(aprMapPath);

            SetNodesTexture(info, mainTex, aprMap);

            return info;
        }

        public static NetInfo SetNodesTexture(this NetInfo info, Texture mainTex, Texture aprMap)
        {
            for (int i = 0; i < info.m_nodes.Length; i++)
            {
                var material = new Material(info.m_nodes[i].m_material);

                material.SetTexture("_MainTex", mainTex);

                if (aprMap != null)
                {
                    material.SetTexture("_APRMap", aprMap);
                }

                info.m_nodes[i].m_material = material;
            }

            return info;
        }
    }
}
