//using System;
//using System.Collections;
//using System.Reflection;
//using ColossalFramework;
//using ColossalFramework.Globalization;
//using ColossalFramework.UI;
//using UnityEngine;
//using Debug = NetworkExtensions.Framework.Debug;
//#if DEBUG

//#endif

//namespace NetworkExtensions.SomeNetwork
//{
//    public class SomeContainer : MonoBehaviour
//    {
//        //public bool initialized = false;
//        //public int initialized_tab = 100;
//        //public static bool initialized_locale = false;
//        //public static bool initialized_locale_category = false;
//        //public static UITextureAtlas thumbnails = null;

//        //void Awake()
//        //{
//        //    DontDestroyOnLoad(this);
//        //}

//        //void OnLevelWasLoaded(int level)
//        //{
//        //    if (level == 6)
//        //    {
//        //        initialized = false;
//        //    }
//        //}

//        //void Update()
//        //{
//        //    if (initialized)
//        //    {
//        //        if (initialized_tab > 0)
//        //        {
//        //            if (--initialized_tab <= 0)
//        //            {
//        //                initialized_tab = 100;
//        //                Debug.Log("SOME ROADS: Trying to locate panels");
//        //                RoadsGroupPanel[] rgp = GameObject.FindObjectsOfType<RoadsGroupPanel>();
//        //                PublicTransportGroupPanel[] pgp = GameObject.FindObjectsOfType<PublicTransportGroupPanel>();
//        //                if (rgp != null && pgp != null)
//        //                {
//        //                    int done = 0;
//        //                    try
//        //                    {
//        //                        foreach (RoadsGroupPanel panel in rgp)
//        //                        {
//        //                            UIButton button = panel.Find<UIButton>("Some Roads");
//        //                            if (button != null)
//        //                            {
//        //                                if (button.name == "Some Roads")
//        //                                {
//        //                                    button.text = "SR";
//        //                                    ++done;
//        //                                    Debug.Log("SOME ROADS: Found tab button & changed text in roads panel");
//        //                                }
//        //                            }
//        //                            else
//        //                            {
//        //                                Debug.Log("SOME ROADS: Found roads panel, but not our button");
//        //                            }
//        //                        }
//        //                        foreach (PublicTransportGroupPanel panel in pgp)
//        //                        {
//        //                            UIButton button = panel.Find<UIButton>("Some Roads");
//        //                            if (button != null)
//        //                            {
//        //                                if (button.name == "Some Roads")
//        //                                {
//        //                                    button.text = "SR";
//        //                                    ++done;
//        //                                    Debug.Log("SOME ROADS: Found tab button & changed text in public transport panel");
//        //                                }
//        //                            }
//        //                            else
//        //                            {
//        //                                Debug.Log("SOME ROADS: Found public transport panel, but not our button");
//        //                            }
//        //                        }
//        //                        if (done >= 2)
//        //                        {
//        //                            initialized_tab = 0;
//        //                        }
//        //                    }
//        //                    catch (Exception)
//        //                    {
//        //                        return;
//        //                    }
//        //                }
//        //            }
//        //        }
//        //        return;
//        //    }

//        //    try
//        //    {
//        //        GameObject.Find("Road").GetComponent<NetCollection>();
//        //        GameObject.Find("Beautification").GetComponent<NetCollection>();
//        //        typeof(LocaleManager).GetField("m_Locale", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(SingletonLite<LocaleManager>.instance);
//        //    }
//        //    catch (Exception)
//        //    {
//        //        return;
//        //    }

//        //    Debug.Log("SOME ROADS: Found collections");
//        //    initialized = true;
//        //    loadThumbnailAtlas();
//        //    buildRoads();
//        //    initialized_locale = true;
//        //    Debug.Log("SOME ROADS: Finished installing components");
//        //}

//        public void buildRoads()
//        {
//            RoadAI ai = null;

//            //// Dark Basic Road
//            //NetInfo darkBasicRoad = clonePrefab("Road", "Basic Road", "Dark Basic Road", "A darker version of the basic two-lane road.");
//            //darkBasicRoad.m_Atlas = thumbnails;
//            //darkBasicRoad.m_Thumbnail = "SOME0";
//            //later(() =>
//            //{
//            //    darkBasicRoad.m_color.r = 0.3f;
//            //    darkBasicRoad.m_color.g = 0.3f;
//            //    darkBasicRoad.m_color.b = 0.3f;
//            //    Debug.Log(string.Format("SOME ROADS: Initialized {0}", darkBasicRoad.name));
//            //});
//            //NetInfo darkBasicRoadElevated = clonePrefab("Road", "Basic Road Elevated", "Dark Basic Road (Elevated)", "");
//            //later(() =>
//            //{
//            //    darkBasicRoadElevated.m_color.r = 0.3f;
//            //    darkBasicRoadElevated.m_color.g = 0.3f;
//            //    darkBasicRoadElevated.m_color.b = 0.3f;
//            //    ai = darkBasicRoad.m_netAI as RoadAI;
//            //    if (ai != null) ai.m_elevatedInfo = darkBasicRoadElevated;
//            //    Debug.Log(string.Format("SOME ROADS: Initialized {0}", darkBasicRoadElevated.name));
//            //});
//            //NetInfo darkBasicRoadBridge = clonePrefab("Road", "Basic Road Bridge", "Dark Basic Road (Bridge)", "");
//            //later(() =>
//            //{
//            //    darkBasicRoadBridge.m_color.r = 0.3f;
//            //    darkBasicRoadBridge.m_color.g = 0.3f;
//            //    darkBasicRoadBridge.m_color.b = 0.3f;
//            //    ai = darkBasicRoad.m_netAI as RoadAI;
//            //    if (ai != null) ai.m_bridgeInfo = darkBasicRoadBridge;
//            //    Debug.Log(string.Format("SOME ROADS: Initialized {0}", darkBasicRoadBridge.name));
//            //});

//            //// Planning Road (Small)
//            //// TODO: Elevated/Bridge versions
//            //NetInfo planningRoad2 = clonePrefab("Road", "Basic Road", "Planning Road (Small)", "A non-functional road for planning road layouts. Can be upgraded to actual roads.");
//            //replaceTexture(planningRoad2, "planning_road_2_lanes", true, true);
//            //planningRoad2.m_Atlas = thumbnails;
//            //planningRoad2.m_Thumbnail = "SOME5";
//            //planningRoad2.m_lanes = new NetInfo.Lane[] { };
//            //later(() =>
//            //{
//            //    planningRoad2.m_color.r = 0.4f;
//            //    planningRoad2.m_color.g = 0.6f;
//            //    planningRoad2.m_color.b = 0.8f;
//            //    planningRoad2.m_createPavement = false;
//            //    planningRoad2.m_createGravel = false;
//            //    planningRoad2.m_createRuining = false;
//            //    planningRoad2.m_hasParkingSpaces = false;
//            //    ai = planningRoad2.m_netAI as RoadAI;
//            //    ai.m_constructionCost = 0;
//            //    ai.m_maintenanceCost = 0;
//            //    ai.m_noiseRadius = 0;
//            //    ai.m_noiseAccumulation = 0;
//            //    ai.m_enableZoning = false;
//            //    ai.m_elevatedInfo = null;
//            //    ai.m_bridgeInfo = null;
//            //});
//            //// Planning Road (Large)
//            //// TODO: Elevated/Bridge versions
//            //NetInfo planningRoad6 = clonePrefab("Road", "Large Road", "Planning Road (Large)", "A non-functional road for planning road layouts. Can be upgraded to actual roads.");
//            //replaceTexture(planningRoad6, "planning_road_2_lanes", true, true);
//            //planningRoad6.m_Atlas = thumbnails;
//            //planningRoad6.m_Thumbnail = "SOME6";
//            //planningRoad6.m_lanes = new NetInfo.Lane[] { };
//            //later(() =>
//            //{
//            //    planningRoad6.m_color.r = 0.4f;
//            //    planningRoad6.m_color.g = 0.6f;
//            //    planningRoad6.m_color.b = 0.8f;
//            //    planningRoad6.m_createPavement = false;
//            //    planningRoad6.m_createGravel = false;
//            //    planningRoad6.m_createRuining = false;
//            //    planningRoad6.m_hasParkingSpaces = false;
//            //    ai = planningRoad6.m_netAI as RoadAI;
//            //    ai.m_constructionCost = 0;
//            //    ai.m_maintenanceCost = 0;
//            //    ai.m_noiseRadius = 0;
//            //    ai.m_noiseAccumulation = 0;
//            //    ai.m_enableZoning = false;
//            //    ai.m_elevatedInfo = null;
//            //    ai.m_bridgeInfo = null;
//            //});

//            // Rural Road (no sidewalks)
//            // TODO: lane props include wrong speed limit sign
//            // TODO: Transitions to normal roads are not perfect
//            NetInfo ruralRoad = clonePrefab("Road", "Gravel Road", "Rural Road", "A rural version of the basic two-lane road without sidewalks.");
//            ruralRoad.m_Atlas = thumbnails;
//            ruralRoad.m_Thumbnail = "SOME3";
//            for (int i = 0; i < ruralRoad.m_segments.Length; ++i)
//            {
//                ruralRoad.m_segments[i].m_material = darkBasicRoad.m_segments[0].m_material;
//                ruralRoad.m_segments[i].m_lodMaterial = darkBasicRoad.m_segments[0].m_lodMaterial;
//            }
//            for (int i = 0; i < ruralRoad.m_nodes.Length; ++i)
//            {
//                ruralRoad.m_nodes[i].m_material = darkBasicRoad.m_nodes[0].m_material;
//                ruralRoad.m_nodes[i].m_lodMaterial = darkBasicRoad.m_nodes[0].m_lodMaterial;
//            }
//            later(() =>
//            {
//                ruralRoad.m_createPavement = false;
//                ruralRoad.m_createGravel = true;
//                ruralRoad.m_createRuining = false;
//                ruralRoad.m_color.r = 0.6f;
//                ruralRoad.m_color.g = 0.6f;
//                ruralRoad.m_color.b = 0.6f;

//                ruralRoad.m_averageVehicleLaneSpeed = 0.8f;
//                for (int i = 0; i < ruralRoad.m_lanes.Length; ++i)
//                {
//                    NetInfo.Lane l = ruralRoad.m_lanes[i];
//                    if (l.m_laneType == NetInfo.LaneType.Vehicle)
//                    {
//                        l.m_speedLimit = 0.8f;
//                        foreach (NetInfo.Lane l2 in darkBasicRoad.m_lanes)
//                        {
//                            if (l2.m_laneType == NetInfo.LaneType.Vehicle && l2.m_direction == l.m_direction)
//                            {
//                                NetLaneProps p = ScriptableObject.CreateInstance<NetLaneProps>();
//                                p.name = string.Format("Rural Road marks {0}", l.m_direction.Name<NetInfo.Direction>());
//                                FastList<NetLaneProps.Prop> ps = new FastList<NetLaneProps.Prop>();
//                                foreach (NetLaneProps.Prop prop in l2.m_laneProps.m_props)
//                                {
//                                    if (prop.m_prop.name == "Manhole") continue;
//                                    ps.Add(prop);
//                                }
//                                p.m_props = ps.ToArray();
//                                ps.Clear();
//                                ps.Release();
//                                l.m_laneProps = p;
//                                break;
//                            }
//                        }
//                    }
//                    else if (l.m_laneType == NetInfo.LaneType.Pedestrian || l.m_laneType == NetInfo.LaneType.PublicTransport)
//                    {
//                        ruralRoad.m_lanes[i] = null;
//                    }
//                }
//                removeNull(ref ruralRoad.m_lanes);
//                Debug.Log(string.Format("SOME ROADS: Initialized {0}", ruralRoad.name));
//            });

//            // Small Highway
//            NetInfo smallHighway = clonePrefab("Road", "Oneway Road", "Small Highway", "A highway with two lanes. 100% as high as the original, but only 66% as way.");
//            replaceTexture(smallHighway, "road_small", true, false);
//            smallHighway.m_Atlas = thumbnails;
//            smallHighway.m_Thumbnail = "SOME4";
//            later(() =>
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
//                removeNull(ref smallHighway.m_lanes);
//                ai = smallHighway.m_netAI as RoadAI;
//                ai.m_highwayRules = true;
//                ai.m_enableZoning = false;
//                Debug.Log(string.Format("SOME ROADS: Initialized {0}", smallHighway.name));
//            });
//            NetInfo smallHighwayElevated = clonePrefab("Road", "Oneway Road Elevated", "Small Highway (Elevated)", "");
//            replaceTexture(smallHighwayElevated, "road_small", true, false);
//            later(() =>
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
//                removeNull(ref smallHighwayElevated.m_lanes);
//                ai = smallHighway.m_netAI as RoadAI;
//                ai.m_elevatedInfo = smallHighwayElevated;
//                RoadBridgeAI bai = smallHighwayElevated.m_netAI as RoadBridgeAI;
//                bai.m_highwayRules = true;
//                Debug.Log(string.Format("SOME ROADS: Initialized {0}", smallHighwayElevated.name));
//            });
//            NetInfo smallHighwayBridge = clonePrefab("Road", "Oneway Road Bridge", "Small Highway (Bridge)", "");
//            replaceTexture(smallHighwayBridge, "road_small", true, false);
//            later(() =>
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
//                removeNull(ref smallHighwayBridge.m_lanes);
//                ai = smallHighway.m_netAI as RoadAI;
//                ai.m_bridgeInfo = smallHighwayBridge;
//                RoadBridgeAI bai = smallHighwayBridge.m_netAI as RoadBridgeAI;
//                bai.m_highwayRules = true;
//                Debug.Log(string.Format("SOME ROADS: Initialized {0}", smallHighwayBridge.name));
//            });

//            // Play Street
//            // TODO: LOD switches this to a pink basic road when zoomed out, in particular intersections
//            // TODO: better road markings, speed limit signs, etc.?
//            NetInfo playStreet = clonePrefab("Road", "Basic Road", "Play Street", "A play street for residential areas. Very low speed limit.");
//            replaceTexture(playStreet, "play_street", true, true);
//            playStreet.m_Atlas = thumbnails;
//            playStreet.m_Thumbnail = "SOME1";
//            later(() =>
//            {
//                playStreet.m_color.r = 0.7f;
//                playStreet.m_color.g = 0.6f;
//                playStreet.m_color.b = 0.65f;
//                playStreet.m_hasParkingSpaces = false;

//                NetLaneProps.Prop pot = new NetLaneProps.Prop();
//                pot.m_prop = findProp("Flower pot 01");
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
//                bench.m_prop = findProp("Bench 01");
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
//                    NetLaneProps.Prop rot = new NetLaneProps.Prop();
//                    shallowCopy<NetLaneProps.Prop>(propsLeft.m_props[i], ref rot);
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
//                Debug.Log(string.Format("SOME ROADS: Initialized {0}", playStreet.name));
//            });
//            NetInfo playStreetElevated = clonePrefab("Road", "Basic Road Elevated", "Play Street (Elevated)", "");
//            replaceTexture(playStreetElevated, "play_street", true, true);
//            later(() =>
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
//                ai = playStreet.m_netAI as RoadAI;
//                if (ai != null)
//                {
//                    ai.m_elevatedInfo = playStreetElevated;
//                    ai.m_bridgeInfo = playStreetElevated;
//                }
//                Debug.Log(string.Format("SOME ROADS: Initialized {0}", playStreetElevated.name));
//            });

//            // Dike
//            NetInfo dike = clonePrefab("Road", "Gravel Road", "Dike", "Fight back against the floods and reclaim land from the sea with this affordable and fashionable dike.");
//            dike.m_Atlas = thumbnails;
//            dike.m_Thumbnail = "SOME2";
//            later(() =>
//            {
//                dike.m_lowerTerrain = false;
//                dike.m_flattenTerrain = true;
//                ai = dike.m_netAI as RoadAI;
//                if (ai != null)
//                {
//                    ai.m_elevatedInfo = dike;
//                    ai.m_bridgeInfo = dike;
//                    ai.m_enableZoning = false;
//                    ai.m_constructionCost *= 15;
//                    ai.m_maintenanceCost *= 3;
//                }
//                Debug.Log(string.Format("SOME ROADS: Initialized {0}", dike.name));
//            });

//            // Train Track (Pavement)
//            NetInfo pavementTrainTrack = clonePrefab("Public Transport", "Train Track", "Train Track (Pavement)", "Train tracks on pavement. Not much else to say about them.");
//            later(() =>
//            {
//                pavementTrainTrack.m_createGravel = false;
//                pavementTrainTrack.m_createPavement = true;
//                Debug.Log(string.Format("SOME ROADS: Initialized {0}", pavementTrainTrack.name));
//            });

//            /*
//             * Underground train tracks commented out until I can make tunnel entrance and underground station assets
//             * 
//            // Train Track (Underground) 
//            NetInfo undergroundTrainTrack = clonePrefab("Public Transport", "Train Track", "Train Track (Underground)", "Train tracks below the ground. What will they think of next?");
//            later(() => {
//                undergroundTrainTrack.m_createGravel = false;
//                undergroundTrainTrack.m_createPavement = false;
//                undergroundTrainTrack.m_buildHeight = -10;
//                undergroundTrainTrack.m_flattenTerrain = false;
//                undergroundTrainTrack.m_clipTerrain = false;
//                // enable connection to metro tunnels, which is useless since they won't use each other's tracks anyway
//                undergroundTrainTrack.m_connectionClass = ScriptableObject.CreateInstance<ItemClass>();
//                undergroundTrainTrack.m_connectionClass.m_service = ItemClass.Service.PublicTransport;
//                undergroundTrainTrack.m_connectionClass.m_subService = ItemClass.SubService.None;
//                undergroundTrainTrack.m_connectionClass.m_level = ItemClass.Level.None;
//                undergroundTrainTrack.m_connectionClass.m_layer = ItemClass.Layer.None;
//                Debug.Log(string.Format("SOME ROADS: Initialized {0}", undergroundTrainTrack.name));
//            });
//            */
//        }

//        public void loadThumbnailAtlas()
//        {
//            if (thumbnails != null) return;

//            thumbnails = ScriptableObject.CreateInstance<UITextureAtlas>();
//            thumbnails.padding = 0;
//            thumbnails.name = "Some Roads Thumbnails";

//            Shader shader = Shader.Find("UI/Default UI Shader");
//            if (shader != null) thumbnails.material = new Material(shader);

//            string path = getModPath() + "/thumbnail_atlas.png";
//            Texture2D tex = new Texture2D(1, 1);
//            tex.LoadImage(System.IO.File.ReadAllBytes(path));
//            thumbnails.material.mainTexture = tex;

//            Texture2D tx = new Texture2D(109, 100);

//            string[] ts = new string[] { "", "Disabled", "Focused", "Hovered", "Pressed" };
//            for (int i = 0; i < 10; ++i)
//            {
//                for (int j = 0; j < ts.Length; ++j)
//                {
//                    UITextureAtlas.SpriteInfo sprite = new UITextureAtlas.SpriteInfo();
//                    sprite.name = string.Format("SOME{0}{1}", i, ts[j]);
//                    sprite.region = new Rect((j * 109f) / 1024f, (i * 100f) / 1024f, 109f / 1024f, 100f / 1024f);
//                    sprite.texture = tx;
//                    thumbnails.AddSprite(sprite);
//                }
//            }
//        }

//        public void replaceTexture(NetInfo ni, string texName, bool segments, bool nodes)
//        {
//            string path = getModPath() + "/{0}.png";
//            Texture2D tex = new Texture2D(1, 1);
//            tex.LoadImage(System.IO.File.ReadAllBytes(string.Format(path, texName)));
//            replaceTexture(ni, tex, segments, nodes);
//        }

//        public void replaceTexture(NetInfo ni, Texture tex, bool segments, bool nodes)
//        {
//            if (segments)
//            {
//                Material mat = new Material(ni.m_segments[0].m_material);
//                mat.shader = ni.m_segments[0].m_material.shader;
//                mat.SetTexture("_MainTex", tex);
//                for (int i = 0; i < ni.m_segments.Length; ++i)
//                {
//                    ni.m_segments[i].m_material = mat;
//                    ni.m_segments[i].m_lodRenderDistance = 2500;
//                }
//            }
//            if (nodes)
//            {
//                Material mat = new Material(ni.m_nodes[0].m_material);
//                mat.shader = ni.m_nodes[0].m_material.shader;
//                mat.SetTexture("_MainTex", tex);
//                for (int i = 0; i < ni.m_nodes.Length; ++i)
//                {
//                    ni.m_nodes[i].m_material = mat;
//                    ni.m_nodes[i].m_lodRenderDistance = 2500;
//                }
//            }
//        }

//        public void removeNull<T>(ref T[] array)
//        {
//            int count = 0;
//            for (int i = 0; i < array.Length; ++i)
//            {
//                if (array[i] != null) ++count;
//            }
//            T[] nu = new T[count];
//            count = 0;
//            for (int i = 0; i < array.Length; ++i)
//            {
//                if (array[i] != null)
//                {
//                    nu[count] = array[i];
//                    ++count;
//                }
//            }
//            array = nu;
//        }

//        public void cloneArray<T>(ref T[] source) where T : new()
//        {
//            T[] new_array = new T[source.Length];
//            for (int i = 0; i < new_array.Length; ++i)
//            {
//                T original = source[i];
//                T copy = new T();
//                foreach (FieldInfo fi in typeof(T).GetAllFields())
//                {
//                    fi.SetValue(copy, fi.GetValue(original));
//                }
//                new_array[i] = copy;
//            }
//            source = new_array;
//        }

//        public void shallowCopy<T>(T source, ref T clone) where T : new()
//        {
//            foreach (FieldInfo f in typeof(T).GetAllFields())
//            {
//                f.SetValue(clone, f.GetValue(source));
//            }
//        }

//        public void later(Action a)
//        {
//            Singleton<LoadingManager>.instance.QueueLoadingAction(inCoroutine(a));
//        }

//        public IEnumerator inCoroutine(Action a)
//        {
//            a.Invoke();
//            yield break;
//        }

//        public PropInfo findProp(string name)
//        {
//            foreach (PropCollection collection in PropCollection.FindObjectsOfType<PropCollection>())
//            {
//                //Debug.Log(string.Format("SOME ROADS: PropCollection {0}", collection.name));
//                foreach (PropInfo prop in collection.m_prefabs)
//                {
//                    //Debug.Log(string.Format("SOME ROADS: - PropInfo {0}", prop.name));
//                    if (prop.name == name)
//                    {
//                        return prop;
//                    }
//                }
//            }
//            return null;
//        }

//        public NetInfo clonePrefab(string collectionName, string sourceName, string name, string desc)
//        {
//            Debug.Log(string.Format("SOME ROADS: Cloning {1} -> {2}, adding to collection: {0}", collectionName, sourceName, name));

//            foreach (NetCollection collection in NetCollection.FindObjectsOfType<NetCollection>())
//            {
//                foreach (NetInfo prefab in collection.m_prefabs)
//                {
//                    if (prefab.name == sourceName)
//                    {
//                        return clonePrefab(prefab, collectionName, name, desc);
//                    }
//                }
//            }
//            return null;
//        }

//        public NetInfo clonePrefab(NetInfo prefab, string collectionName, string name, string desc)
//        {
//            Locale locale = (Locale)typeof(LocaleManager).GetField("m_Locale", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(SingletonLite<LocaleManager>.instance);
//            NetInfo originalInfo = prefab;
//            GameObject someObject = GameObject.Instantiate<GameObject>(prefab.gameObject);
//            someObject.transform.SetParent(transform);
//            someObject.name = name;
//            NetInfo someInfo = someObject.GetComponent<NetInfo>();
//            someInfo.m_prefabInitialized = false;
//            someInfo.m_netAI = null;

//            typeof(NetInfo).GetField("m_UICategory", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(someInfo, "Some Roads");

//            if (!initialized_locale)
//            {
//                Locale.Key k = new Locale.Key() { m_Identifier = "NET_TITLE", m_Key = name };
//                locale.AddLocalizedString(k, name);
//                k = new Locale.Key() { m_Identifier = "NET_DESC", m_Key = name };
//                locale.AddLocalizedString(k, desc);
//                if (!initialized_locale_category)
//                {
//                    k = new Locale.Key() { m_Identifier = "MAIN_CATEGORY", m_Key = "Some Roads" };
//                    locale.AddLocalizedString(k, "Some Roads");
//                    initialized_locale_category = true;
//                }
//            }

//            if (collectionName != null)
//            {
//                MethodInfo initMethod = typeof(NetCollection).GetMethod("InitializePrefabs", BindingFlags.Static | BindingFlags.NonPublic);
//                Singleton<LoadingManager>.instance.QueueLoadingAction((IEnumerator)initMethod.Invoke(null, new object[] {
//                    collectionName,
//                    new[] { someInfo },
//                    new string[] { }
//                }));

//                // temporarily remove props reference and reset after InitializePrefabs() is done
//                foreach (NetInfo.Lane l in someInfo.m_lanes)
//                {
//                    NetLaneProps p = l.m_laneProps;
//                    later(() =>
//                    {
//                        l.m_laneProps = p;
//                    });
//                    l.m_laneProps = null;
//                }
//            }

//            return someInfo;
//        }
//    }

//}