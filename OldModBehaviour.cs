//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using ColossalFramework;
//using ColossalFramework.Globalization;
//using ColossalFramework.UI;
//using NetworkExtensions.Extensions;
//using UnityEngine;

//namespace NetworkExtensions
//{
//    public class ModBehaviour : MonoBehaviour
//    {
//        private static UITextureAtlas _thumbnailAtlas = null;
//        private const string THUMBS_PREFIX = "SOME";

//        private bool _startInit = true;
//        private bool _finishedInit = false;
//        private bool _initializedNetworkInfo = false;
//        private bool _initializedLocalization = false;
//        private bool _initializedPanels = false;

//        private readonly ICollection<NetInfo> _networkExtensions = new List<NetInfo>();

//        void Awake()
//        {
//            DontDestroyOnLoad(this);

//            if (_thumbnailAtlas == null)
//            {
//                _thumbnailAtlas = LoadThumbnailAtlas();
//            }
//        }

//        private Locale _locale = null;

//        private bool ValidateNetworkPrerequisites()
//        {
//            var roadModel = GameObject.Find("Road");

//            if (roadModel == null)
//            {
//                Debug.Log("NExt: roadModel null");
//                return false;
//            }
//            else
//            {
//                Debug.Log("NExt: roadModel not null, continuing");
//            }

//            var roadModelNetColl = roadModel.GetComponent<NetCollection>();

//            if (roadModelNetColl == null)
//            {
//                Debug.Log("NExt: roadModelNetColl null");
//                return false;
//            }
//            else
//            {
//                Debug.Log("NExt: roadModelNetColl not null, continuing");
//            }

//            return true;
//        }

//        private bool ValidateLocalizationPrerequisites()
//        {
//            var localeManager = SingletonLite<LocaleManager>.instance;
//            Debug.Log("NExt: localeManager ok");

//            if (localeManager == null)
//            {
//                Debug.Log("NExt: localeManager null");
//                return false;
//            }
//            else
//            {
//                Debug.Log("NExt: localeManager not null, continuing");
//            }

//            var localeField = typeof(LocaleManager).GetFieldByName("m_Locale");
//            Debug.Log("NExt: locale ok");

//            if (localeField == null)
//            {
//                Debug.Log("NExt: localeField null");
//                return false;
//            }
//            else
//            {
//                Debug.Log("NExt: localeField not null, continuing");
//            }

//            _locale = (Locale)localeField.GetValue(localeManager);
//            Debug.Log("NExt: locale ok");

//            if (_locale == null)
//            {
//                Debug.Log("NExt: locale null");
//                return false;
//            }
//            else
//            {
//                Debug.Log("NExt: locale not null, continuing");
//            }

//            return true;
//        }

//        private int _frameNb = 0;

//        private void Initialize()
//        {
//            //if (_frameNb++ < 20)
//            //{
//            //    Debug.Log("NExt: " + _frameNb);
//            //    return;
//            //}

//            if (!_initializedLocalization)
//            {
//                if (ValidateLocalizationPrerequisites())
//                {
//                    try
//                    {
//                        Debug.Log("NExt: BuildNetworkExtensionsLocalization");
//                        CreateCategoryLocalization();
//                        BuildNetworkExtensionsLocalization();
//                    }
//                    catch (Exception ex)
//                    {
//                        Debug.Log("Crashed-Localization");
//                        Debug.Log("NExt: " + ex.Message);
//                        Debug.Log("NExt: " + ex.ToString());
//                    }
//                    finally
//                    {
//                        _initializedLocalization = true;
//                    }
//                }
//            }

//            if (!_initializedNetworkInfo)
//            {
//                if (ValidateNetworkPrerequisites())
//                {
//                    try
//                    {
//                        Debug.Log("NExt: BuildNetworkExtensions");
//                        BuildNetworkExtensions();
//                        //BuildRoads();
//                        Debug.Log("NExt: Finished installing components");
//                    }
//                    catch (Exception ex)
//                    {
//                        Debug.Log("Crashed-Network");
//                        Debug.Log("NExt: " + ex.Message);
//                        Debug.Log("NExt: " + ex.ToString());
//                    }
//                    finally
//                    {
//                        _initializedNetworkInfo = true;
//                    }
//                }
//            }

//            if (!_initializedPanels & _initializedNetworkInfo)
//            {
//                if (ValidateSetPanelNamePrerequisites())
//                {
//                    try
//                    {
//                        Debug.Log("NExt: BuildNetworkExtensionsPanels");
//                        SetPanelName();
//                    }
//                    catch (Exception ex)
//                    {
//                        Debug.Log("Crashed-SetPanel");
//                        Debug.Log("NExt: " + ex.Message);
//                        Debug.Log("NExt: " + ex.ToString());
//                    }
//                    finally
//                    {
//                        _initializedPanels = true;
//                    }
//                }
//            }

//            _finishedInit =
//                _initializedNetworkInfo &&
//                _initializedLocalization &&
//                _initializedPanels;
//        }
        
//        void Update()
//        {
//            if (_startInit && !_finishedInit)
//            {
//                Initialize();
//            }
//        }

//        private bool ValidateSetPanelNamePrerequisites()
//        {
//            var roadsGroupPanel = FindObjectsOfType<RoadsGroupPanel>();
//            if (roadsGroupPanel == null)
//            {
//                Debug.Log("NExt: Roads panels not found");
//                return false;
//            }

//            if (roadsGroupPanel.Length == 0)
//            {
//                Debug.Log("NExt: Found roads panels, but list is empty");
//                return false;
//            }

//            Debug.Log("NExt: Found roads panels, continuing");
//            return true;
//        }

//        private void SetPanelName()
//        {
//            foreach (RoadsGroupPanel panel in FindObjectsOfType<RoadsGroupPanel>())
//            {
//                var button = panel.Find<UIButton>(Variables.CATEGORY_NAME);
//                if (button != null && button.name == Variables.CATEGORY_NAME)
//                {
//                    button.text = "EXT";
//                    Debug.Log("NExt: Found tab button & changed text in roads panel");
//                    break;
//                }
//                else
//                {
//                    Debug.Log("NExt: Found roads panel, but not our button");
//                }
//            }
//        }

//        private static UITextureAtlas LoadThumbnailAtlas()
//        {
//            var thumbnailAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
//            thumbnailAtlas.padding = 0;
//            thumbnailAtlas.name = "Network Extensions Thumbnails";

//            Shader shader = Shader.Find("UI/Default UI Shader");
//            if (shader != null) thumbnailAtlas.material = new Material(shader);

//            string path = Mod.GetPath() + "/thumbnail_atlas.png";
//            Texture2D tex = new Texture2D(1, 1);
//            tex.LoadImage(System.IO.File.ReadAllBytes(path));
//            thumbnailAtlas.material.mainTexture = tex;

//            Texture2D tx = new Texture2D(109, 100);

//            string[] ts = { "", "Disabled", "Focused", "Hovered", "Pressed" };
//            for (int i = 0; i < 10; ++i)
//            {
//                for (int j = 0; j < ts.Length; ++j)
//                {
//                    UITextureAtlas.SpriteInfo sprite = new UITextureAtlas.SpriteInfo();
//                    sprite.name = string.Format(THUMBS_PREFIX + "{0}{1}", i, ts[j]);
//                    sprite.region = new Rect((j * 109f) / 1024f, (i * 100f) / 1024f, 109f / 1024f, 100f / 1024f);
//                    sprite.texture = tx;
//                    thumbnailAtlas.AddSprite(sprite);
//                }
//            }

//            return thumbnailAtlas;
//        }

//        #region BuildingNewStyle

//        private static IEnumerable<NetInfoBuilder> _netInfoBuilders;
//        public static IEnumerable<NetInfoBuilder> NetInfoBuilders
//        {
//            get
//            {
//                if (_netInfoBuilders == null)
//                {
//                    _netInfoBuilders = new[]
//                    {
//                        DefineSmallHighwayNetInfoBuilder()
//                    };
//                }

//                return _netInfoBuilders;
//            }
//        }


//        private static NetInfoBuilder DefineSmallHighwayNetInfoBuilder()
//        {
//            return new NetInfoBuilder
//            {
//                PrefabName = "Oneway Road",
//                Name = "Small Highway",
//                Description = "A highway with two lanes. 100% as high as the original, but only 66% as way.",
//                UICategory = Variables.CATEGORY_NAME,
//                UIThumbnailAtlas = _thumbnailAtlas,
//                UIThumbnail = THUMBS_PREFIX + "4",
//                WithElevatedVersion = true,
//                WithBridgeVersion = true,
//                PostBuild = info =>
//                {
//                    info.m_createPavement = false;
//                    info.m_createGravel = true;
//                    info.m_averageVehicleLaneSpeed = 2f;
//                    for (int i = 0; i < info.m_lanes.Length; ++i)
//                    {
//                        var l = info.m_lanes[i];
//                        l.m_allowStop = false;
//                        if (l.m_laneType == NetInfo.LaneType.Vehicle)
//                        {
//                            l.m_speedLimit = 2f;
//                        }
//                        else if (l.m_laneType == NetInfo.LaneType.Pedestrian)
//                        {
//                            l.m_laneType = NetInfo.LaneType.None;
//                        }
//                        else
//                        {
//                            info.m_lanes[i] = null;
//                        }
//                    }
//                    Tools.RemoveNull(ref info.m_lanes);

//                    var roadBaseAI = info.m_netAI as RoadBaseAI;
//                    if (roadBaseAI != null)
//                    {
//                        roadBaseAI.m_highwayRules = true;
//                    }

//                    var roadAI = info.m_netAI as RoadAI;
//                    if (roadAI != null)
//                    {
//                        roadAI.m_enableZoning = false;
//                    }
//                }
//            };
//        }

//        private void BuildNetworkExtensions()
//        {
//            foreach (var builder in NetInfoBuilders)
//            {
//                var netInfo = builder.Build(transform);

//                _networkExtensions.Add(netInfo);
//            }
//        }

//        private void BuildNetworkExtensionsLocalization()
//        {
//            foreach (var builder in NetInfoBuilders)
//            {
//                builder.AddLocalizedStrings(_locale);
//            }
//        }

//        #endregion

//        #region BuildingOldStyle

//        private void BuildRoads()
//        {
//            //BuildDarkBasicRoad();
//            //BuildPlanningRoadSmall();
//            //BuildPlanningRoadLarge();
//            //BuildRuralRoad();
//            BuildSmallHighway();
//            BuildLargeHighway();
//            //BuildPlayStreet();

//            //// Train Track (Pavement)
//            //NetInfo pavementTrainTrack = clonePrefab("Public Transport", "Train Track", "Train Track (Pavement)", "Train tracks on pavement. Not much else to say about them.");
//            //Loading.QueueAction(() =>
//            //{
//            //    pavementTrainTrack.m_createGravel = false;
//            //    pavementTrainTrack.m_createPavement = true;
//            //    Debug.Log(string.Format("NExt: Initialized {0}", pavementTrainTrack.name));
//            //});
//        }

//        private void BuildDarkBasicRoad()
//        {
//            CreatePrefabLocalization("Dark Basic Road", "A darker version of the basic two-lane road.");

//            var darkBasicRoad = 
//                Tools
//                    .ClonePrefab<NetInfo>("Basic Road", "Dark Basic Road", transform)
//                    .SetUICategory(Variables.CATEGORY_NAME)
//                    .SetUIThumbnail(_thumbnailAtlas, THUMBS_PREFIX + "0");
            
//            Loading.QueueAction(() =>
//            {
//                darkBasicRoad.m_color.r = 0.3f;
//                darkBasicRoad.m_color.g = 0.3f;
//                darkBasicRoad.m_color.b = 0.3f;
//                Debug.Log(string.Format("NExt: Initialized {0}", darkBasicRoad.name));
//            });


//            var darkBasicRoadElevated = 
//                Tools
//                    .ClonePrefab<NetInfo>("Basic Road Elevated", "Dark Basic Road (Elevated)", transform)
//                    .SetUICategory(Variables.CATEGORY_NAME);
            
//            Loading.QueueAction(() =>
//            {
//                darkBasicRoadElevated.m_color.r = 0.3f;
//                darkBasicRoadElevated.m_color.g = 0.3f;
//                darkBasicRoadElevated.m_color.b = 0.3f;
//                var ai = darkBasicRoad.m_netAI as RoadAI;
//                if (ai != null) ai.m_elevatedInfo = darkBasicRoadElevated;
//                Debug.Log(string.Format("NExt: Initialized {0}", darkBasicRoadElevated.name));
//            });


//            var darkBasicRoadBridge =
//                Tools
//                    .ClonePrefab<NetInfo>("Basic Road Bridge", "Dark Basic Road (Bridge)", transform)
//                    .SetUICategory(Variables.CATEGORY_NAME);
            
//            Loading.QueueAction(() =>
//            {
//                darkBasicRoadBridge.m_color.r = 0.3f;
//                darkBasicRoadBridge.m_color.g = 0.3f;
//                darkBasicRoadBridge.m_color.b = 0.3f;
//                var ai = darkBasicRoad.m_netAI as RoadAI;
//                if (ai != null) ai.m_bridgeInfo = darkBasicRoadBridge;
//                Debug.Log(string.Format("NExt: Initialized {0}", darkBasicRoadBridge.name));
//            });
//        }

//        private void BuildPlanningRoadSmall()
//        {
//            // TODO: Elevated/Bridge versions
//            CreatePrefabLocalization("Planning Road (Small)", "A non-functional road for planning road layouts. Can be upgraded to actual roads.");

//            var planningRoad2 =
//                Tools
//                    .ClonePrefab<NetInfo>("Basic Road", "Planning Road (Small)", transform)
//                    .SetUICategory(Variables.CATEGORY_NAME)
//                    .SetUIThumbnail(_thumbnailAtlas, THUMBS_PREFIX + "5")
//                    .ReplaceTexture("planning_road_2_lanes", true, true);

//            planningRoad2.m_lanes = new NetInfo.Lane[] { };
//            Loading.QueueAction(() =>
//            {
//                planningRoad2.m_color.r = 0.4f;
//                planningRoad2.m_color.g = 0.6f;
//                planningRoad2.m_color.b = 0.8f;
//                planningRoad2.m_createPavement = false;
//                planningRoad2.m_createGravel = false;
//                planningRoad2.m_createRuining = false;
//                planningRoad2.m_hasParkingSpaces = false;
//                var ai = planningRoad2.m_netAI as RoadAI;
//                ai.m_constructionCost = 0;
//                ai.m_maintenanceCost = 0;
//                ai.m_noiseRadius = 0;
//                ai.m_noiseAccumulation = 0;
//                ai.m_enableZoning = false;
//                ai.m_elevatedInfo = null;
//                ai.m_bridgeInfo = null;
//            });
//        }

//        private void BuildPlanningRoadLarge()
//        {
//            // TODO: Elevated/Bridge versions
//            CreatePrefabLocalization("Planning Road (Large)", "A non-functional road for planning road layouts. Can be upgraded to actual roads.");
            
//            var planningRoad6 = 
//                Tools
//                    .ClonePrefab<NetInfo>("Large Road", "Planning Road (Large)", transform)
//                    .SetUICategory(Variables.CATEGORY_NAME)
//                    .SetUIThumbnail(_thumbnailAtlas, THUMBS_PREFIX + "6")
//                    .ReplaceTexture("planning_road_2_lanes", true, true);

//            planningRoad6.m_lanes = new NetInfo.Lane[] { };
//            Loading.QueueAction(() =>
//            {
//                planningRoad6.m_color.r = 0.4f;
//                planningRoad6.m_color.g = 0.6f;
//                planningRoad6.m_color.b = 0.8f;
//                planningRoad6.m_createPavement = false;
//                planningRoad6.m_createGravel = false;
//                planningRoad6.m_createRuining = false;
//                planningRoad6.m_hasParkingSpaces = false;
//                var ai = planningRoad6.m_netAI as RoadAI;
//                ai.m_constructionCost = 0;
//                ai.m_maintenanceCost = 0;
//                ai.m_noiseRadius = 0;
//                ai.m_noiseAccumulation = 0;
//                ai.m_enableZoning = false;
//                ai.m_elevatedInfo = null;
//                ai.m_bridgeInfo = null;
//            });
//        }

//        private void BuildRuralRoad()
//        {
//            //// Rural Road (no sidewalks)
//            //// TODO: lane props include wrong speed limit sign
//            //// TODO: Transitions to normal roads are not perfect
//            //NetInfo ruralRoad = clonePrefab("Road", "Gravel Road", "Rural Road", "A rural version of the basic two-lane road without sidewalks.");
//            //ruralRoad.m_Atlas = thumbnails;
//            //ruralRoad.m_Thumbnail = THUMBS_PREFIX + "3";
//            //for (int i = 0; i < ruralRoad.m_segments.Length; ++i)
//            //{
//            //    ruralRoad.m_segments[i].m_material = darkBasicRoad.m_segments[0].m_material;
//            //    ruralRoad.m_segments[i].m_lodMaterial = darkBasicRoad.m_segments[0].m_lodMaterial;
//            //}
//            //for (int i = 0; i < ruralRoad.m_nodes.Length; ++i)
//            //{
//            //    ruralRoad.m_nodes[i].m_material = darkBasicRoad.m_nodes[0].m_material;
//            //    ruralRoad.m_nodes[i].m_lodMaterial = darkBasicRoad.m_nodes[0].m_lodMaterial;
//            //}
//            //Loading.QueueAction(() =>
//            //{
//            //    ruralRoad.m_createPavement = false;
//            //    ruralRoad.m_createGravel = true;
//            //    ruralRoad.m_createRuining = false;
//            //    ruralRoad.m_color.r = 0.6f;
//            //    ruralRoad.m_color.g = 0.6f;
//            //    ruralRoad.m_color.b = 0.6f;

//            //    ruralRoad.m_averageVehicleLaneSpeed = 0.8f;
//            //    for (int i = 0; i < ruralRoad.m_lanes.Length; ++i)
//            //    {
//            //        NetInfo.Lane l = ruralRoad.m_lanes[i];
//            //        if (l.m_laneType == NetInfo.LaneType.Vehicle)
//            //        {
//            //            l.m_speedLimit = 0.8f;
//            //            foreach (NetInfo.Lane l2 in darkBasicRoad.m_lanes)
//            //            {
//            //                if (l2.m_laneType == NetInfo.LaneType.Vehicle && l2.m_direction == l.m_direction)
//            //                {
//            //                    NetLaneProps p = ScriptableObject.CreateInstance<NetLaneProps>();
//            //                    p.name = string.Format("Rural Road marks {0}", l.m_direction.Name<NetInfo.Direction>());
//            //                    FastList<NetLaneProps.Prop> ps = new FastList<NetLaneProps.Prop>();
//            //                    foreach (NetLaneProps.Prop prop in l2.m_laneProps.m_props)
//            //                    {
//            //                        if (prop.m_prop.name == "Manhole") continue;
//            //                        ps.Add(prop);
//            //                    }
//            //                    p.m_props = ps.ToArray();
//            //                    ps.Clear();
//            //                    ps.Release();
//            //                    l.m_laneProps = p;
//            //                    break;
//            //                }
//            //            }
//            //        }
//            //        else if (l.m_laneType == NetInfo.LaneType.Pedestrian || l.m_laneType == NetInfo.LaneType.PublicTransport)
//            //        {
//            //            ruralRoad.m_lanes[i] = null;
//            //        }
//            //    }
//            //    removeNull(ref ruralRoad.m_lanes);
//            //    Debug.Log(string.Format("NExt: Initialized {0}", ruralRoad.name));
//            //});
//        }

//        private void BuildSmallHighway()
//        {
//            CreatePrefabLocalization("Small Highway", "A highway with two lanes. 100% as high as the original, but only 66% as way.");
            
//            var smallHighway = 
//                Tools
//                    .ClonePrefab<NetInfo>("Oneway Road", "Small Highway", transform)
//                    .SetUICategory(Variables.CATEGORY_NAME)
//                    .SetUIThumbnail(_thumbnailAtlas, THUMBS_PREFIX + "4")
//                    .ReplaceTexture("road_small", true, false);

//            Loading.QueueAction(() =>
//            {
//                smallHighway.m_createPavement = false;
//                smallHighway.m_createGravel = true;
//                smallHighway.m_averageVehicleLaneSpeed = 2f;
//                for (int i = 0; i < smallHighway.m_lanes.Length; ++i)
//                {
//                    NetInfo.Lane l = smallHighway.m_lanes[i];
//                    l.m_allowStop = false;
//                    if (l.m_laneType == NetInfo.LaneType.Vehicle)
//                    {
//                        l.m_speedLimit = 2f;
//                    }
//                    else if (l.m_laneType == NetInfo.LaneType.Pedestrian)
//                    {
//                        l.m_laneType = NetInfo.LaneType.None;
//                    }
//                    else
//                    {
//                        smallHighway.m_lanes[i] = null;
//                    }
//                }
//                Tools.RemoveNull(ref smallHighway.m_lanes);
//                var ai = smallHighway.m_netAI as RoadAI;
//                ai.m_highwayRules = true;
//                ai.m_enableZoning = false;
//                Debug.Log(string.Format("NExt: Initialized {0}", smallHighway.name));
//            });


//            var smallHighwayElevated = 
//                Tools
//                    .ClonePrefab<NetInfo>("Oneway Road Elevated", "Small Highway (Elevated)", transform)
//                    .SetUICategory(Variables.CATEGORY_NAME)
//                    .ReplaceTexture("road_small", true, false);

//            Loading.QueueAction(() =>
//            {
//                smallHighwayElevated.m_averageVehicleLaneSpeed = 2f;
//                for (int i = 0; i < smallHighwayElevated.m_lanes.Length; ++i)
//                {
//                    NetInfo.Lane l = smallHighwayElevated.m_lanes[i];
//                    l.m_allowStop = false;
//                    if (l.m_laneType == NetInfo.LaneType.Vehicle)
//                    {
//                        l.m_speedLimit = 2f;
//                    }
//                    else if (l.m_laneType == NetInfo.LaneType.Pedestrian || l.m_laneType == NetInfo.LaneType.PublicTransport)
//                    {
//                        smallHighwayElevated.m_lanes[i] = null;
//                    }
//                }
//                Tools.RemoveNull(ref smallHighwayElevated.m_lanes);
//                var ai = smallHighway.m_netAI as RoadAI;
//                ai.m_elevatedInfo = smallHighwayElevated;
//                RoadBridgeAI bai = smallHighwayElevated.m_netAI as RoadBridgeAI;
//                bai.m_highwayRules = true;
//                Debug.Log(string.Format("NExt: Initialized {0}", smallHighwayElevated.name));
//            });


//            var smallHighwayBridge = 
//                Tools
//                    .ClonePrefab<NetInfo>("Oneway Road Bridge", "Small Highway (Bridge)", transform)
//                    .SetUICategory(Variables.CATEGORY_NAME)
//                    .ReplaceTexture("road_small", true, false);

//            Loading.QueueAction(() =>
//            {
//                smallHighwayBridge.m_averageVehicleLaneSpeed = 2f;
//                for (int i = 0; i < smallHighwayBridge.m_lanes.Length; ++i)
//                {
//                    NetInfo.Lane l = smallHighwayBridge.m_lanes[i];
//                    if (l.m_laneType == NetInfo.LaneType.Vehicle)
//                    {
//                        l.m_allowStop = false;
//                        l.m_speedLimit = 2f;
//                    }
//                    else if (l.m_laneType == NetInfo.LaneType.Pedestrian || l.m_laneType == NetInfo.LaneType.PublicTransport)
//                    {
//                        smallHighwayBridge.m_lanes[i] = null;
//                    }
//                }
//                Tools.RemoveNull(ref smallHighwayBridge.m_lanes);
//                var ai = smallHighway.m_netAI as RoadAI;
//                ai.m_bridgeInfo = smallHighwayBridge;
//                RoadBridgeAI bai = smallHighwayBridge.m_netAI as RoadBridgeAI;
//                bai.m_highwayRules = true;
//                Debug.Log(string.Format("NExt: Initialized {0}", smallHighwayBridge.name));
//            });
//        }

//        private void BuildLargeHighway()
//        {
//            CreatePrefabLocalization("Large Highway", "An highway with six lanes. 100% as high as the original, with 200% more lanes.");

//            var largeHighway =
//                Tools
//                    .ClonePrefab<NetInfo>("Large Oneway", "Large Highway", transform)
//                    .SetUICategory(Variables.CATEGORY_NAME) // "RoadsHighway"
//                    .SetUIThumbnail(_thumbnailAtlas, THUMBS_PREFIX + "4")
//                    .ReplaceSegmentTexture(
//                        @"LargeHighway\Segments\_MainTex.png",
//                        @"LargeHighway\Segments\_XYSMap.png",
//                        @"LargeHighway\Segments\_APRMap.png")
//                    .ReplaceNodeTexture(
//                        @"LargeHighway\Nodes\_MainTex.png",
//                        @"LargeHighway\Nodes\_APRMap.png");

//            Loading.QueueAction(() =>
//            {
//                largeHighway.m_createPavement = false;
//                largeHighway.m_createGravel = true;
//                largeHighway.m_averageVehicleLaneSpeed = 2f;

//                // Filtering unwanted lines
//                for (int i = 0; i < largeHighway.m_lanes.Length; ++i)
//                {
//                    var l = largeHighway.m_lanes[i];
//                    if (l.m_laneType == NetInfo.LaneType.Pedestrian || 
//                        l.m_laneType == NetInfo.LaneType.Parking)
//                    {
//                        largeHighway.m_lanes[i] = null;
//                    }
//                }
//                Tools.RemoveNull(ref largeHighway.m_lanes);

//                var lanes = largeHighway.m_lanes.OrderBy(l => l.m_similarLaneIndex).ToArray();
//                var nbLanes = lanes.Count(); // Supposed to be 6

//                const float laneWidth = 2f; // TODO: Make it 2.5 with new texture
//                const float laneWidthPad = 1f;
//                const float laneWidthTotal = laneWidth + laneWidthPad;
//                var positionStart = (laneWidthTotal * ((1f - nbLanes) / 2f));

//                for (int i = 0; i < lanes.Length; i++)
//                {
//                    var l = lanes[i];
//                    l.m_allowStop = false;
//                    l.m_speedLimit = 2f;
//                    l.m_verticalOffset = 0f;
//                    l.m_width = laneWidthTotal;
//                    l.m_position = positionStart + i * laneWidthTotal;
//                }


//                var ai = (RoadAI)largeHighway.m_netAI;
//                ai.m_highwayRules = true;
//                ai.m_enableZoning = false;
//                ai.m_trafficLights = false;
//                Debug.Log(string.Format("NExt: Initialized {0}", largeHighway.name));
//            });
//        }

//        private void BuildPlayStreet()
//        {
//            // TODO: LOD switches this to a pink basic road when zoomed out, in particular intersections
//            // TODO: better road markings, speed limit signs, etc.?
//            CreatePrefabLocalization("Play Street", "A play street for residential areas. Very low speed limit.");

//            var playStreet = 
//                Tools
//                    .ClonePrefab<NetInfo>("Basic Road", "Play Street", transform)
//                    .SetUICategory(Variables.CATEGORY_NAME)
//                    .SetUIThumbnail(_thumbnailAtlas, THUMBS_PREFIX + "1")
//                    .ReplaceTexture("play_street", true, true);

//            Loading.QueueAction(() =>
//            {
//                playStreet.m_color.r = 0.7f;
//                playStreet.m_color.g = 0.6f;
//                playStreet.m_color.b = 0.65f;
//                playStreet.m_hasParkingSpaces = false;

//                NetLaneProps.Prop pot = new NetLaneProps.Prop();
//                pot.m_prop = FindProp("Flower pot 01");
//                pot.m_finalProp = pot.m_prop;
//                pot.m_tree = pot.m_finalTree = null;
//                pot.m_segmentOffset = 0;
//                pot.m_minLength = 20;
//                pot.m_repeatDistance = 10;
//                pot.m_probability = 100;
//                pot.m_colorMode = NetLaneProps.ColorMode.Default;
//                pot.m_flagsRequired = NetLane.Flags.None;
//                pot.m_flagsForbidden = NetLane.Flags.None;
//                pot.m_startFlagsRequired = NetNode.Flags.None;
//                pot.m_startFlagsForbidden = NetNode.Flags.None;
//                pot.m_endFlagsRequired = NetNode.Flags.None;
//                pot.m_endFlagsForbidden = NetNode.Flags.None;

//                NetLaneProps.Prop bench = new NetLaneProps.Prop();
//                bench.m_prop = FindProp("Bench 01");
//                bench.m_angle = 90;
//                bench.m_finalProp = pot.m_prop;
//                bench.m_tree = pot.m_finalTree = null;
//                bench.m_segmentOffset = 0.12f;
//                bench.m_minLength = 20;
//                bench.m_repeatDistance = 10;
//                bench.m_probability = 100;
//                bench.m_colorMode = NetLaneProps.ColorMode.Default;
//                bench.m_flagsRequired = NetLane.Flags.None;
//                bench.m_flagsForbidden = NetLane.Flags.None;
//                bench.m_startFlagsRequired = NetNode.Flags.None;
//                bench.m_startFlagsForbidden = NetNode.Flags.None;
//                bench.m_endFlagsRequired = NetNode.Flags.None;
//                bench.m_endFlagsForbidden = NetNode.Flags.None;

//                NetLaneProps propsLeft = ScriptableObject.CreateInstance<NetLaneProps>();
//                propsLeft.name = "Some Roads - play street left";
//                propsLeft.m_props = new NetLaneProps.Prop[] { pot, bench };

//                NetLaneProps propsRight = ScriptableObject.CreateInstance<NetLaneProps>();
//                propsRight.name = "Some Roads - play street right";
//                propsRight.m_props = new NetLaneProps.Prop[propsLeft.m_props.Length];

//                for (int i = 0; i < propsRight.m_props.Length; ++i)
//                {
//                    var rot = propsLeft.m_props[i].ShallowCopy();
//                    rot.m_angle = (rot.m_angle + 180 + 360) % 360;
//                    propsRight.m_props[i] = rot;
//                }

//                playStreet.m_averageVehicleLaneSpeed = 0.2f;
//                foreach (NetInfo.Lane l in playStreet.m_lanes)
//                {
//                    if (l.m_laneType == NetInfo.LaneType.Vehicle)
//                    {
//                        l.m_speedLimit = 0.2f;
//                    }
//                    if (l.m_laneType == NetInfo.LaneType.Pedestrian)
//                    {
//                        l.m_laneProps = propsLeft;
//                        NetLaneProps tmp = propsLeft;
//                        propsLeft = propsRight;
//                        propsRight = tmp;
//                    }
//                    else
//                    {
//                        l.m_laneProps = null;
//                    }
//                }
//                Debug.Log(string.Format("NExt: Initialized {0}", playStreet.name));
//            });


//            var playStreetElevated =
//                Tools
//                    .ClonePrefab<NetInfo>("Basic Road Elevated", "Play Street (Elevated)", transform) 
//                    .SetUICategory(Variables.CATEGORY_NAME)
//                    .ReplaceTexture("play_street", true, true);

//            Loading.QueueAction(() =>
//            {
//                playStreetElevated.m_color.r = 0.7f;
//                playStreetElevated.m_color.g = 0.6f;
//                playStreetElevated.m_color.b = 0.65f;
//                playStreetElevated.m_averageVehicleLaneSpeed = 0.2f;
//                foreach (NetInfo.Lane l in playStreetElevated.m_lanes)
//                {
//                    if (l.m_laneType == NetInfo.LaneType.Vehicle)
//                    {
//                        l.m_speedLimit = 0.2f;
//                    }
//                    l.m_laneProps = null;
//                }
//                var ai = playStreet.m_netAI as RoadAI;
//                if (ai != null)
//                {
//                    ai.m_elevatedInfo = playStreetElevated;
//                    ai.m_bridgeInfo = playStreetElevated;
//                }
//                Debug.Log(string.Format("NExt: Initialized {0}", playStreetElevated.name));
//            });
//        }

//        #endregion

//        private PropInfo FindProp(string name)
//        {
//            foreach (PropCollection collection in PropCollection.FindObjectsOfType<PropCollection>())
//            {
//                //Debug.Log(string.Format("NExt: PropCollection {0}", collection.name));
//                foreach (PropInfo prop in collection.m_prefabs)
//                {
//                    //Debug.Log(string.Format("NExt: - PropInfo {0}", prop.name));
//                    if (prop.name == name)
//                    {
//                        return prop;
//                    }
//                }
//            }
//            return null;
//        }

//        private void CreatePrefabLocalization(string name, string desc)
//        {
//            _locale.AddLocalizedString(new Locale.Key
//            {
//                m_Identifier = "NET_TITLE",
//                m_Key = name
//            }, name);

//            _locale.AddLocalizedString(new Locale.Key()
//            {
//                m_Identifier = "NET_DESC",
//                m_Key = name
//            }, desc);
//        }

//        private void CreateCategoryLocalization()
//        {
//            _locale.AddLocalizedString(new Locale.Key()
//            {
//                m_Identifier = "MAIN_CATEGORY",
//                m_Key = Variables.CATEGORY_NAME
//            }, Variables.CATEGORY_NAME);
//        }
//    }
//}