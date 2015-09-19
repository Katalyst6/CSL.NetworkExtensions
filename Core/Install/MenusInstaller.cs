using System;
using System.Linq;
using ColossalFramework.UI;
using NetworkExtensions.Framework;
using NetworkExtensions.Menus;

#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions.Install
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
                        Debug.Log("NExt: Additionnal Menus have been installed successfully");
                    }
#if DEBUG
                    else
                    {
                        Debug.Log("NExt: Something has happened, Additionnal Menus have not been installed");
                    }
#endif
                }
                catch (Exception ex)
                {
                    Debug.Log("NExt: Crashed-Initialized Additionnal Menus");
                    Debug.Log("NExt: " + ex.Message);
                    Debug.Log("NExt: " + ex.ToString());
                }
            });
        }

        private static bool InstallRoadSmallHV(RoadsGroupPanel group)
        {
            var b = group.Find<UIButton>(AdditionnalMenus.ROADS_SMALL_HV);
            if (b != null)
            {
                b.atlas = ToolsUnity.LoadMenuThumbnails();
                return true;
            }

            return false;
        }
    }
}
