using System;
using ColossalFramework.UI;
using NetworkExtensions.Framework;
using Object = UnityEngine.Object;

namespace NetworkExtensions.Install
{
    public class MenusInstaller : IInstaller
    {
        public void Execute()
        {
            Loading.QueueAction(() =>
            {
                try
                {
                    var menuInstalled = false;

                    foreach (var group in Object.FindObjectsOfType<RoadsGroupPanel>())
                    {
                        var button = group.Find<UIButton>(Menus.ROADS_SMALL_HV);
                        var panel = group.Find<UIPanel>(Menus.ROADS_SMALL_HV + "Panel");

                        if (button != null && panel != null)
                        {
                            // TODO: Set the thumbnail
                            button.zOrder = 1;
                            panel.zOrder = 1;
                            menuInstalled = true;
                            break;
                        }
                    }

                    if (menuInstalled)
                    {
                        Debug.Log("NExt: New Menus have been installed");
                    }
                    else
                    {
                        Debug.Log("NExt: Something has happened, new menus have not been installed");
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
    }
}
