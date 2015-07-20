using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NetworkExtensions.Framework
{
    public static partial class ToolsCSL
    {
        public static NetInfo CloneNetInfo(string prefabName, string newName)
        {
            // TODO: Perf issue with Object.Instantiate
            //return CloneNetInfoII(prefabName, newName);

            Debug.Log(string.Format("NExt: Cloning {0} -> {1}", prefabName, newName));

            var prefab = FindPrefab<NetInfo>(prefabName);
            if (prefab == null)
            {
                throw new Exception(string.Format("NExt: Prefab {0} not found", prefabName));
            }

            var gameObject = Object.Instantiate(prefab.gameObject);
            gameObject.name = newName;

            var info = gameObject.GetComponent<NetInfo>();
            info.m_prefabInitialized = false;

            Debug.Log(string.Format("NExt: Cloning completed {0} -> {1}", prefabName, newName));

            return info;
        }

        public static NetInfo CloneNetInfoII(string prefabName, string newName)
        {
            Debug.Log(string.Format("NExt: Cloning {0} -> {1}", prefabName, newName));

            var prefab = FindPrefab<NetInfo>(prefabName);
            if (prefab == null)
            {
                throw new Exception(string.Format("NExt: Prefab {0} not found", prefabName));
            }

            var gameObject = new GameObject(newName);
            var info = gameObject.AddComponent<NetInfo>();

            info.CopyMembersFrom(prefab, "gameObject", "transform", "tag", "m_instanceID");

            info.m_class = ScriptableObject.CreateInstance<ItemClass>();
            info.m_class.CopyMembersFrom(prefab.m_class);
            info.m_class.name = newName;

            info.m_instanceID = InstanceID.Empty;
            info.m_instanceNeeded = true;

            info.m_netAI = (NetAI)gameObject.AddComponent(prefab.m_netAI.GetType());
            info.m_netAI.CopyMembersFrom(prefab.m_netAI, "m_info", "gameObject", "transform", "tag");
            info.m_netAI.name = newName;
            info.m_netAI.m_info = info;

            info.name = newName;
            info.m_prefabInitialized = false;

            Debug.Log(string.Format("NExt: Cloning completed {0} -> {1}", prefabName, newName));
            return info;
        }

        public static T FindPrefab<T>(string prefabName) where T : PrefabInfo
        {
            return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(p => p.name == prefabName);
        }
    }
}
