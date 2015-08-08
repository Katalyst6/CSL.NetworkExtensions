using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ColossalFramework.IO;
using ColossalFramework.Steamworks;
using ICities;
using NetworkExtensions.Framework;
using UnityEngine;
using Object = UnityEngine.Object;
#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions
{
    public class Mod : LoadingExtensionBase, IUserMod
    {
        private const UInt64 WORKSHOP_ID = 478820060;

        public const string NEXT_OBJECT_NAME = "Network Extensions";
        public const string NEXT_CATEGORY_NAME = "Network Extensions";

        public const string ROAD_NETCOLLECTION = "Road";
        public const string NEWROADS_NETCOLLECTION = "NewRoad";

        public string Name
        {
            get { return "Network Extensions"; }
        }

        public string Description
        {
            get { return "An addition of highways and roads"; }
        }

        private bool _isReleased = true;
        private GameObject _container = null;
        private NetCollection _newRoads = null;
        private ModInitializer _initalizer = null;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (_isReleased)
            {
                _container = new GameObject(NEXT_OBJECT_NAME);

                _newRoads = _container.AddComponent<NetCollection>();
                _newRoads.name = NEWROADS_NETCOLLECTION;

                _initalizer = _container.AddComponent<ModInitializer>();
                _initalizer.NewRoads = _newRoads;
                _initalizer.InitializationCompleted += InitializationCompleted;

                _isReleased = false;
            }
        }

        private void InitializationCompleted(object sender, EventArgs e)
        {
            Loading.QueueAction(() =>
            {
                if (_initalizer != null)
                {
                    _initalizer.NewRoads = null;
                    Object.Destroy(_initalizer);
                    _initalizer = null;
                }
            });
        }

        public override void OnReleased()
        {
            base.OnReleased();

            if (_isReleased)
            {
                return;
            }

            if (_initalizer != null)
            {
                Object.Destroy(_initalizer);
                _initalizer = null;
            }

            if (_newRoads != null)
            {
                Object.Destroy(_newRoads);
                _newRoads = null;
            }

            if (_container != null)
            {
                Object.Destroy(_container);
                _container = null;
            }

            _isReleased = true;
        }

        public static string GetPath()
        {
            var localPath = DataLocation.modsPath + "/NetworkExtensions";
            //Debug.Log("NExt: " + localPath);
            if (Directory.Exists(localPath))
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

        private static IEnumerable<INetInfoBuilder> s_netInfoBuilders;
        public static IEnumerable<INetInfoBuilder> NetInfoBuilders
        {
            get
            {
                if (s_netInfoBuilders == null)
                {
                    var builderType = typeof(INetInfoBuilder);

                    s_netInfoBuilders = typeof(ModInitializer)
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

        private static IEnumerable<INetInfoModifier> s_netInfoModifiers;
        public static IEnumerable<INetInfoModifier> NetInfoModifiers
        {
            get
            {
                if (s_netInfoModifiers == null)
                {
                    var builderType = typeof(INetInfoBuilder);

                    s_netInfoModifiers = typeof(ModInitializer)
                        .Assembly
                        .GetTypes()
                        .Where(t => !t.IsAbstract && !t.IsInterface)
                        .Where(builderType.IsAssignableFrom)
                        .Select(t => (INetInfoModifier)Activator.CreateInstance(t))
                        .ToArray();
                }

                return s_netInfoModifiers;
            }
        }
    }
}
