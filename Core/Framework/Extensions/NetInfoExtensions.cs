using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ColossalFramework;
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
            //Debug.Log(String.Format("NExt: Cloning {0} -> {1}", originalNetInfo.name, newName));

            var gameObject = Object.Instantiate(originalNetInfo.gameObject);
            gameObject.transform.parent = originalNetInfo.gameObject.transform; // N.B. This line is evil and removing it is killoing the game's performances
            gameObject.name = newName;

            var info = gameObject.GetComponent<NetInfo>();
            info.m_prefabInitialized = false;

            //Debug.Log(String.Format("NExt: Cloning completed {0} -> {1}", originalNetInfo.name, newName));

            return info;
        }

        public static void DisplayLaneProps(this NetInfo info)
        {
            foreach (var lane in info.m_lanes)
            {
                if (lane.m_laneProps != null)
                {
                    Debug.Log(string.Format("NExt: Lane name {0}", lane.m_laneProps.name));

                    if (lane.m_laneProps.m_props != null)
                    {
                        foreach (var prop in lane.m_laneProps.m_props)
                        {
                            if (prop.m_prop != null)
                            {
                                Debug.Log(string.Format("NExt:     Prop name {0}", prop.m_prop.name));
                            }
                        }
                    }
                }
            }
        }

        public static NetInfo SetUICategory(this NetInfo info, string category)
        {
            typeof(NetInfo).GetField("m_UICategory", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(info, category);

            return info;
        }

        public static NetInfo SetSegmentsTexture(this NetInfo info, TexturesSet newTextures, TexturesSet newLODTextures = null)
        {
            foreach (var segment in info.m_segments)
            {
                if (segment.m_material != null)
                {
                    segment.m_material = segment.m_material.Clone(newTextures);
                }

                if (segment.m_segmentMaterial != null)
                {
                    segment.m_segmentMaterial = segment.m_segmentMaterial.Clone(newTextures);
                }

                if (segment.m_lodMaterial != null)
                {
                    if (newLODTextures != null)
                    {
                        segment.m_lodMaterial = segment.m_lodMaterial.Clone(newLODTextures);
                    }
                }
            }

            return info;
        }

        public static NetInfo SetNodesTexture(this NetInfo info, TexturesSet newTextures, TexturesSet newLODTextures = null)
        {
            foreach (var node in info.m_nodes)
            {
                if (node.m_material != null)
                {
                    node.m_material = node.m_material.Clone(newTextures);
                }

                if (node.m_nodeMaterial != null)
                {
                    node.m_nodeMaterial = node.m_nodeMaterial.Clone(newTextures);
                }

                if (node.m_lodMaterial != null)
                {
                    if (newLODTextures != null)
                    {
                        node.m_lodMaterial = node.m_lodMaterial.Clone(newLODTextures);
                    }
                }
            }

            return info;
        }
    }
}
