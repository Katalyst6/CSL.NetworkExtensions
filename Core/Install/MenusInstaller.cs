using System;
using System.Linq;
using ColossalFramework.UI;
using NetworkExtensions.Framework;
using NetworkExtensions.Menus;
using Debug = NetworkExtensions.Framework.Debug;

namespace NetworkExtensions.Install
{
    public class MenusInstaller : Installer
    {
        protected override bool ValidatePrerequisites()
        {
            try
            {
                if (!ModInitializer.s_initializedLocalization)
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
                    Debug.Log("NExt: Installing Additionnal Menus");
                    var menuInstalled = false;

                    var group = FindObjectsOfType<RoadsGroupPanel>().FirstOrDefault();

                    if (InstallRoadSmallHV(group))
                    {
                        menuInstalled = true;
                    }

                    if (menuInstalled)
                    {
                        Debug.Log("NExt: Additionnal Menus have been installed");
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
                    Debug.Log("NExt: Crashed-Initialized New Menus");
                    Debug.Log("NExt: " + ex.Message);
                    Debug.Log("NExt: " + ex.ToString());
                }
            });
        }

        private static bool InstallRoadSmallHV(RoadsGroupPanel group)
        {
            const int TARGET_ID = 1;
            const string PANEL_NAME = AdditionnalMenus.ROADS_SMALL_HV + "Panel";
            const string BTN_NAME = AdditionnalMenus.ROADS_SMALL_HV;

            var panelInstalled = false;
            var buttonInstalled = false;

            var p = group.Find<UIPanel>(PANEL_NAME);
            if (p != null)
            {
                p.zOrder = TARGET_ID;
                panelInstalled = true;
            }

            var b = group.Find<UIButton>(BTN_NAME);
            if (b != null)
            {
                b.zOrder = TARGET_ID;
                b.atlas = ToolsUnity.LoadMenuThumbnails();
                buttonInstalled = true;
            }

            return panelInstalled && buttonInstalled;
        }
    }
}
