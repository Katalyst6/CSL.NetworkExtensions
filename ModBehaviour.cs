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
    public partial class ModBehaviour : MonoBehaviour
    {
        public const string NEXT_CATEGORY_NAME = "Network Extensions";

        public const string ROAD_NETCOLLECTION = "Road";
        public const string NEWROADS_NETCOLLECTION = "NewRoad";

        private bool _doneWithInit = false;
        private bool _initializedNetworkInfo = false;
        private bool _initializedLocalization = false;

        void Start()
        {
            ToolsCSL.Loading.QueueAction(TextureManager.Instance.FindAndLoadAllTextures);
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

        private static IEnumerable<INetInfoBuilder> s_netInfoBuilders;
        public static IEnumerable<INetInfoBuilder> NetInfoBuilders
        {
            get
            {
                if (s_netInfoBuilders == null)
                {
                    var builderType = typeof (INetInfoBuilder);

                    s_netInfoBuilders = typeof (ModBehaviour)
                        .Assembly
                        .GetTypes()
                        .Where(t => !t.IsAbstract && !t.IsInterface)
                        .Where(builderType.IsAssignableFrom)
                        .Select(t => (INetInfoBuilder)Activator.CreateInstance(t))
                        .ToArray();
                }

                return s_netInfoBuilders;
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
                var version = typeof(ModBehaviour).Assembly.GetName().Version;
                Debug.Log(string.Format("NExt: Version {0}", version));
                _versionShown = true;
            }
#endif


            if (!_initializedLocalization)
            {
                if (ValidateLocalizationPrerequisites())
                {
                    try
                    {
                        Debug.Log("NExt: Localization");
                        var localeManager = SingletonLite<LocaleManager>.instance;
                        var localeField = typeof(LocaleManager).GetFieldByName("m_Locale");
                        var locale = (Locale)localeField.GetValue(localeManager);

                        locale.AddCategoryLocalizedString();

                        foreach (var builder in NetInfoBuilders)
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
                    finally
                    {
                        _initializedLocalization = true;
                    }
                }
            }

            if (!_initializedNetworkInfo)
            {
                if (ValidateNetworkPrerequisites())
                {
                    ToolsCSL.Loading.QueueAction(() =>
                    {
                        try
                        {
                            Debug.Log("NExt: Build NetworkExtensions");

                            var newInfos = new List<NetInfo>();

                            foreach (var builder in NetInfoBuilders)
                            {
                                newInfos.AddRange(builder.Build());
                            }

                            if (newInfos.Count > 0)
                            {
                                var newRoadCollection = NewRoadsNetCollection;
                                newRoadCollection.m_prefabs = newInfos.ToArray();

                                PrefabCollection<NetInfo>.InitializePrefabs(newRoadCollection.name, newRoadCollection.m_prefabs, new string[] { });
                            }

                            Debug.Log("NExt: Finished installing components");
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("NExt: Crashed-Network");
                            Debug.Log("NExt: " + ex.Message);
                            Debug.Log("NExt: " + ex.ToString());
                        }
                    });


                    _initializedNetworkInfo = true;
                }
            }

            _doneWithInit =
                _initializedNetworkInfo &&
                _initializedLocalization;

        }

        private bool ValidateNetworkPrerequisites()
        {
            var roadObject = GameObject.Find(ROAD_NETCOLLECTION);
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
                if (col.name == ROAD_NETCOLLECTION)
                {
                    roadCollFound = true;
                }
            }

            if (!roadCollFound)
            {
                return false;
            }

            var thisNetColls = gameObject.GetComponents<NetCollection>();
            if (thisNetColls == null)
            {
                return false;
            }

            var newRoadCollFound = false;
            foreach (var col in thisNetColls)
            {
                if (col.name == NEWROADS_NETCOLLECTION)
                {
                    newRoadCollFound = true;
                }
            }

            if (!newRoadCollFound)
            {
                return false;
            }

            return true;
        }

        private NetCollection NewRoadsNetCollection
        {
            get
            {
                var thisNetColls = gameObject.GetComponents<NetCollection>();
                if (thisNetColls == null)
                {
                    return null;
                }

                foreach (var col in thisNetColls)
                {
                    if (col.name == NEWROADS_NETCOLLECTION)
                    {
                        return col;
                    }
                }

                return null;
            }
        }

        private bool ValidateLocalizationPrerequisites()
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
    }
}