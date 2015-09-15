using System;
using ICities;
using NetworkExtensions.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NetworkExtensions
{
    public partial class Mod : LoadingExtensionBase
    {
        private bool _isReleased = true;
        private GameObject _container = null;
        private NetCollection _newRoads = null;
        private ModInitializer _initalizer = null;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (_isReleased)
            {
                if (GetPath() != PATH_NOT_FOUND)
                {
                    _container = new GameObject(NEXT_OBJECT_NAME);

                    _newRoads = _container.AddComponent<NetCollection>();
                    _newRoads.name = NEWROADS_NETCOLLECTION;

                    _initalizer = _container.AddComponent<ModInitializer>();
                    _initalizer.NewRoads = _newRoads;
                    _initalizer.InitializationCompleted += InitializationCompleted;
                }

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

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            MenusInstaller.Execute();
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
    }
}
