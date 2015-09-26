using System;
using System.Collections.Generic;
using System.Linq;
using CSL.ExtensionFramework;
using UnityEngine;

#if DEBUG
using Debug = CSL.ExtensionFramework.Debug;
#endif

namespace NetworkExtensions.Install
{
    public class RoadsInstaller : Installer
    {
        public NetCollection NewRoads { get; set; }

        protected override bool ValidatePrerequisites()
        {
            if (!LocalizationInstaller.Done)
            {
                return false;
            }

            if (!AssetsInstaller.Done)
            {
                return false;
            }

            var roadObject = GameObject.Find(Mod.ROAD_NETCOLLECTION);
            if (roadObject == null)
            {
                return false;
            }

            var netColl = FindObjectsOfType<NetCollection>();
            if (netColl == null || !netColl.Any())
            {
                return false;
            }

            var roadCollFound = false;
            foreach (var col in netColl)
            {
                if (col.name == Mod.ROAD_NETCOLLECTION)
                {
                    roadCollFound = true;
                }
            }

            if (!roadCollFound)
            {
                return false;
            }

            if (NewRoads == null)
            {
                return false;
            }

            return true;
        }

        protected override void Install()
        {
            var localNewRoads = NewRoads;

            Loading.QueueAction(() =>
            {
                //Debug.Log("NExt: Setting up new Roads and Logic");


                // Builders -----------------------------------------------------------------------
                var newInfos = new List<NetInfo>();

                foreach (var builder in Mod.NetInfoBuilders)
                {
                    try
                    {
                        newInfos.AddRange(builder.Build());

                        Debug.Log(string.Format("NExt: {0} installed", builder.DisplayName));
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(string.Format("NExt: Crashed-Network builders {0}", builder));
                        Debug.Log("NExt: " + ex.Message);
                        Debug.Log("NExt: " + ex.ToString());
                    }
                }

                if (newInfos.Count > 0)
                {
                    localNewRoads.m_prefabs = newInfos.ToArray();
                    PrefabCollection<NetInfo>.InitializePrefabs(localNewRoads.name, localNewRoads.m_prefabs, new string[] { });
                    PrefabCollection<NetInfo>.BindPrefabs();
                }


                // Modifiers ----------------------------------------------------------------------
                foreach (var modifier in Mod.NetInfoModifiers)
                {
                    try
                    {
                        modifier.ModifyExistingNetInfo();

                        Debug.Log(string.Format("NExt: {0} modifications applied", modifier.DisplayName));
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(string.Format("NExt: Crashed-Network modifiers {0}", modifier));
                        Debug.Log("NExt: " + ex.Message);
                        Debug.Log("NExt: " + ex.ToString());
                    }
                }


                // Cross mods support -------------------------------------------------------------
                foreach (var compatibilityPart in Mod.CompatibilityParts)
                {
                    try
                    {
                        if (compatibilityPart.IsPluginActive)
                        {
                            compatibilityPart.Setup(newInfos);

                            Debug.Log(string.Format("NExt: {0} compatibility activated", compatibilityPart.Name));
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(string.Format("NExt: Crashed-CompatibilitySupport {0}", compatibilityPart.Name));
                        Debug.Log("NExt: " + ex.Message);
                        Debug.Log("NExt: " + ex.ToString());
                    }
                }
            });
        }
    }
}