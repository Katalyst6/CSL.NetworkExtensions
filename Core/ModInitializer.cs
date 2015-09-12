using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
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
        private static bool s_initializedLocalization = false; //Only one localization throughout the application
        private bool _initializedCoreLogic = false;
        private bool _initializedAdditionalMenus = false;

        public NetCollection NewRoads { get; set; }

        public delegate void InitializationCompletedEventHandler(object sender, EventArgs e);
        public event InitializationCompletedEventHandler InitializationCompleted;

        void Start()
        {
            Loading.QueueAction(AssetManager.instance.FindAndLoadAllTextures);
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

            if (!_initializedCoreLogic & s_initializedLocalization)
            {
                if (ValidateCoreLogicPrerequisites(NewRoads))
                {
                    InitializeCoreLogic(NewRoads);
                    _initializedCoreLogic = true;
                }
            }

            if (!_initializedAdditionalMenus & _initializedCoreLogic & s_initializedLocalization)
            {
                if (ValidateAdditionalMenusPrerequisites())
                {
                    InitializeAdditionalMenus();
                    _initializedAdditionalMenus = true;
                }
            }

            _doneWithInit =
                _initializedAdditionalMenus &&
                _initializedCoreLogic &&
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

        private static bool ValidateAdditionalMenusPrerequisites()
        {
            var roadsGroupPanels = FindObjectsOfType<RoadsGroupPanel>();
            if (roadsGroupPanels == null)
            {
                return false;
            }

            //if (roadsGroupPanels.Length == 0)
            //{
            //    return false;
            //}

            //var templateFound = roadsGroupPanels
            //    .Select(panel => panel.Find<UIButton>("RoadsSmall"))
            //    .Any(button => button != null);

            //if (!templateFound)
            //{
            //    return false;
            //}

            return true;
        }

        private static bool ValidateCoreLogicPrerequisites(NetCollection newRoads)
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
                    //Debug.Log("NExt: Localization");
                    var localeManager = SingletonLite<LocaleManager>.instance;
                    var localeField = typeof(LocaleManager).GetFieldByName("m_Locale");
                    var locale = (Locale)localeField.GetValue(localeManager);

                    // For future extensions
                    //locale.AddCategoryLocalizedString();

                    foreach (var builder in Mod.NetInfoBuilders)
                    {
                        locale.CreateNetTitleLocalizedString(builder.Name, builder.DisplayName);
                        locale.CreateNetDescriptionLocalizedString(builder.Name, builder.Description);
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

        private static void InitializeAdditionalMenus()
        {
            Loading.QueueAction(() =>
            {
                try
                {
                    Debug.Log("NExt: Initialized New Menus");

                    foreach (var panel in FindObjectsOfType<RoadsGroupPanel>())
                    {
                        panel.PopulateGroups();

                        var button = panel.Find<UIButton>("RoadOW");
                        if (button == null)
                        {
                            Debug.Log("NExt: Button not found");
                        }
                        else
                        {
                            Debug.Log("NExt: Button found!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("NExt: Crashed-Initialized New Menus");
                    Debug.Log("NExt: " + ex.Message);
                    Debug.Log("NExt: " + ex.ToString());
                }
            });
        }

        private static void InitializeCoreLogic(NetCollection newRoads)
        {
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