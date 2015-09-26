﻿using System;
using System.Linq;
using ColossalFramework.UI;
using CSL.NetworkExtensions.Framework;
using CSL.RoadExtensions.Menus;
using UnityEngine;

#if DEBUG
using Debug = CSL.NetworkExtensions.Framework.Debug;
#endif

namespace CSL.RoadExtensions.Install
{
    public class MenusInstaller : Installer
    {
        protected override bool ValidatePrerequisites()
        {
            try
            {
                if (!LocalizationInstaller.Done)
                {
                    return false;
                }

                if (!AssetsInstaller.Done)
                {
                    return false;
                }

                var group = FindObjectsOfType<RoadsGroupPanel>().FirstOrDefault();
                if (group == null)
                {
                    return false;
                }

                var panelContainer = group.Find<UITabContainer>("GTSContainer");
                if (panelContainer == null)
                {
                    return false;
                }

                var buttonContainer = group.Find<UITabstrip>("GroupToolstrip");
                if (buttonContainer == null)
                {
                    return false;
                }

                var panels = panelContainer.components.OfType<UIPanel>();
                if (!panels.Any())
                {
                    return false;
                }

                var buttons = buttonContainer.components.OfType<UIButton>();
                if (!buttons.Any())
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        protected override void Install()
        {
            Loading.QueueAction(() =>
            {
                try
                {
                    var menuInstalled = false;

                    var group = FindObjectsOfType<RoadsGroupPanel>().FirstOrDefault();

                    if (InstallRoadSmallHV(group))
                    {
                        menuInstalled = true;
                    }

                    if (menuInstalled)
                    {
                        Debug.Log("REx: Additionnal Menus have been installed successfully");
                    }
#if DEBUG
                    else
                    {
                        Debug.Log("REx: Something has happened, Additionnal Menus have not been installed");
                    }
#endif
                }
                catch (Exception ex)
                {
                    Debug.Log("REx: Crashed-Initialized Additionnal Menus");
                    Debug.Log("REx: " + ex.Message);
                    Debug.Log("REx: " + ex.ToString());
                }
            });
        }

        private static bool InstallRoadSmallHV(RoadsGroupPanel group)
        {
            var b = group.Find<UIButton>(AdditionnalMenus.ROADS_SMALL_HV);
            if (b != null)
            {
                b.atlas = AdditionnalMenus.LoadThumbnails();
                return true;
            }

            return false;
        }
    }
}
