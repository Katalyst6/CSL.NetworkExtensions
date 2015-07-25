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
    public static class NetInfoExtensions
    {
        public static NetInfo Clone(this NetInfo originalNetInfo, string newName)
        {
            Debug.Log(String.Format("NExt: Cloning {0} -> {1}", originalNetInfo.name, newName));

            var gameObject = Object.Instantiate(originalNetInfo.gameObject);
            gameObject.transform.parent = originalNetInfo.gameObject.transform;
            gameObject.name = newName;

            var info = gameObject.GetComponent<NetInfo>();
            info.m_prefabInitialized = false;

            Debug.Log(String.Format("NExt: Cloning completed {0} -> {1}", originalNetInfo.name, newName));

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
