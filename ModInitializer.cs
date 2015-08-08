using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using NetworkExtensions.Framework;
using UnityEngine;

#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions
{
    public partial class ModInitializer : MonoBehaviour
    {
        private bool _doneWithInit = false;
        private bool _initializedNetworkInfo = false;
        private static bool s_initializedLocalization = false; //Only one localization throughout the application

        public NetCollection NewRoads { get; set; }

        public delegate void InitializationCompletedEventHandler(object sender, EventArgs e);
        public event InitializationCompletedEventHandler InitializationCompleted;

        void Start()
        {
            Loading.QueueAction(TextureManager.instance.FindAndLoadAllTextures);
        }

        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        void Update()
        {
            if (!_doneWithInit)
            {
                Initialize();
            }
        }


#if DEBUG
        private int _frameNb = 0;
        private bool _versionShown = false;
#endif
        
        private void Initialize()
        {
#if DEBUG
            if (_frameNb++ < 20) // Giving some time for the UI to refresh **NB. Putting this constant higher than 100 causes wierd behavior**
            {
                return;
            }

            if (!_versionShown)
            {
                var version = typeof(ModInitializer).Assembly.GetName().Version;
                Debug.Log(string.Format("NExt: Version {0}", version));
                _versionShown = true;
            }
#endif


            if (!s_initializedLocalization)
            {
                if (ValidateLocalizationPrerequisites())
                {
                    InitializeLocalization();
                    s_initializedLocalization = true;
                }
            }

            if (!_initializedNetworkInfo)
            {
                if (ValidateNetworkPrerequisites(NewRoads))
                {
                    InitializeNewRoads(NewRoads);
                    _initializedNetworkInfo = true;
                }
            }

            _doneWithInit =
                _initializedNetworkInfo &&
                s_initializedLocalization;

            if (_doneWithInit)
            {
                if (InitializationCompleted != null)
                {
                    InitializationCompleted(this, EventArgs.Empty);
                }
            }
        }

        private static bool ValidateLocalizationPrerequisites()
        {
            var localeManager = SingletonLite<LocaleManager>.instance;
            if (localeManager == null)
            {
                return false;
            }


            var localeField = typeof(LocaleManager).GetFieldByName("m_Locale");
            if (localeField == null)
            {
                return false;
            }


            var locale = (Locale)localeField.GetValue(localeManager);
            if (locale == null)
            {
                return false;
            }

            return true;
        }

        private static bool ValidateNetworkPrerequisites(NetCollection newRoads)
        {
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

            if (newRoads == null)
            {
                return false;
            }

            return true;
        }

        private static void InitializeLocalization()
        {
            Loading.QueueAction(() =>
            {
                try
                {
                    Debug.Log("NExt: Localization");
                    var localeManager = SingletonLite<LocaleManager>.instance;
                    var localeField = typeof(LocaleManager).GetFieldByName("m_Locale");
                    var locale = (Locale)localeField.GetValue(localeManager);

                    // For future extensions
                    //locale.AddCategoryLocalizedString();

                    foreach (var builder in Mod.NetInfoBuilders)
                    {
                        builder.DefineLocalization(locale);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("NExt: Crashed-Localization");
                    Debug.Log("NExt: " + ex.Message);
                    Debug.Log("NExt: " + ex.ToString());
                }
            });
        }

        private static void InitializeNewRoads(NetCollection newRoads)
        {
            Loading.QueueAction(() =>
            {
                Debug.Log("NExt: Build NetworkExtensions");


                // Builders -----------------------------------------------------------------------
                var newInfos = new List<NetInfo>();

                foreach (var builder in Mod.NetInfoBuilders)
                {
                    try
                    {
                        newInfos.AddRange(builder.Build());
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
                    newRoads.m_prefabs = newInfos.ToArray();

                    PrefabCollection<NetInfo>.InitializePrefabs(newRoads.name, newRoads.m_prefabs, new string[] { });
                    PrefabCollection<NetInfo>.BindPrefabs();
                }


                // Modifiers ----------------------------------------------------------------------
                foreach (var modifier in Mod.NetInfoModifiers)
                {
                    try
                    {
                        modifier.ModifyExistingNetInfo();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(string.Format("NExt: Crashed-Network modifiers {0}", modifier));
                        Debug.Log("NExt: " + ex.Message);
                        Debug.Log("NExt: " + ex.ToString());
                    }
                }

                Debug.Log("NExt: Finished installing components");
            });
        }
    }
}