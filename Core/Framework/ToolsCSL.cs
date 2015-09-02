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
        public static T FindPrefab<T>(string prefabName, bool crashOnNotFound = true) 
            where T : PrefabInfo
        {
            var prefab = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(p => p.name == prefabName);

            if (prefab == null)
            {
                throw new Exception(string.Format("NExt: Prefab {0} not found", prefabName));
            }

            return prefab;
        }
    }
}
