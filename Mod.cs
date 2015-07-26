using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework.IO;
using ColossalFramework.Steamworks;
using ICities;
using NetworkExtensions.Framework;
using UnityEngine;

#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions
{
    public class Mod : LoadingExtensionBase, IUserMod
    {
        private const UInt64 WORKSHOP_ID = 478820060;

        public string Name
        {
            get { return "Network Extensions"; }
        }

        public string Description
        {
            get { return "An addition of highways and roads"; }
        }

        private static GameObject s_container = null;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (s_container == null)
            {
                s_container = new GameObject("Network Extensions");
                s_container.AddComponent<NetCollection>()
                           .name = ModBehaviour.NEWROADS_NETCOLLECTION;
                s_container.AddComponent<ModBehaviour>();
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();
            if (s_container != null)
            {
                GameObject.Destroy(s_container);
                s_container = null;
            }
        }

        public static string GetPath()
        {
            var localPath = DataLocation.modsPath + "/NetworkExtensions";
            //Debug.Log("NExt: " + localPath);
            if (System.IO.Directory.Exists(localPath))
            {
                //Debug.Log("NExt: Local path exists, looking for assets here: " + localPath);
                return localPath;
            }

            foreach (var mod in Steam.workshop.GetSubscribedItems())
            {
                if (mod.AsUInt64 == WORKSHOP_ID)
                {
                    var workshopPath = Steam.workshop.GetSubscribedItemPath(mod);
                    //Debug.Log("NExt: Workshop path: " + workshopPath);
                    return workshopPath;
                }
            }

            return ".";
        }
    }
}
