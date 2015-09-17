using System;
using ICities;
using NetworkExtensions.Framework;
using NetworkExtensions.Install;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NetworkExtensions
{
    public partial class Mod : LoadingExtensionBase
    {
        private bool _isReleased = true;
        private GameObject _container = null;
        private NetCollection _newRoads = null;
        private LocalizationInstaller _localizationInstaller = null;
        private RoadsInstaller _roadsInstaller = null;
        private MenusInstaller _menusInstaller = null;

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

                    _localizationInstaller = _container.AddComponent<LocalizationInstaller>();
                    _localizationInstaller.InstallationCompleted += LocInstallationCompleted;

                    _roadsInstaller = _container.AddComponent<RoadsInstaller>();
                    _roadsInstaller.NewRoads = _newRoads;
                    _roadsInstaller.InitializationCompleted += RoadsInstallationCompleted;
                }

                _isReleased = false;
            }
        }

        private void LocInstallationCompleted()
        {
            Loading.QueueAction(() =>
            {
                if (_localizationInstaller != null)
                {
                    Object.Destroy(_localizationInstaller);
                    _localizationInstaller = null;
                }
            });
        }

        private void RoadsInstallationCompleted(object sender, EventArgs e)
        {
            Loading.QueueAction(() =>
            {
                if (_roadsInstaller != null)
                {
                    _roadsInstaller.NewRoads = null;
                    Object.Destroy(_roadsInstaller);
                    _roadsInstaller = null;
                }
            });
        }

        private void MenusInstallationCompleted()
        {
            Loading.QueueAction(() =>
            {
                if (_menusInstaller != null)
                {
                    Object.Destroy(_menusInstaller);
                    _menusInstaller = null;
                }
            });
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (_container != null && _menusInstaller == null)
            {
                _menusInstaller = _container.AddComponent<MenusInstaller>();
                _menusInstaller.InstallationCompleted += MenusInstallationCompleted;
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();

            if (_isReleased)
            {
                return;
            }

            if (_localizationInstaller != null)
            {
                Object.Destroy(_localizationInstaller);
                _localizationInstaller = null;
            }

            if (_roadsInstaller != null)
            {
                Object.Destroy(_roadsInstaller);
                _roadsInstaller = null;
            }

            if (_menusInstaller != null)
            {
                Object.Destroy(_menusInstaller);
                _menusInstaller = null;
            }

            if (_roadsInstaller != null)
            {
                Object.Destroy(_roadsInstaller);
                _roadsInstaller = null;
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
