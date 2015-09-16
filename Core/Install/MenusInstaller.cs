using System;
using System.Linq;
using ColossalFramework.UI;
using NetworkExtensions.AdditionnalMenus;
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
                        var button1 = group.Find<UIButton>("RoadsMedium");
                        var n = button1.normalBgSprite;
                        var d = button1.disabledBgSprite;
                        var f = button1.focusedBgSprite;
                        var h = button1.hoveredBgSprite;
                        var p = button1.pressedBgSprite;

                        var spriteToGet = new[]
                        {
                            n, d, f, h, p
                        };

                        var texs = button1
                            .atlas
                            .sprites
                            .Where(s => spriteToGet.Contains(s.name))
                            .OrderBy(s => s.name)
                            .Select(s => s.texture)
                            .ToArray();

                        foreach (var t in texs)
                        {
                            AssetManager.instance.SpecialTextures.Add(t);
                        }

                        var button = group.Find<UIButton>(Menus.ROADS_SMALL_HV);
                        var panel = group.Find<UIPanel>(Menus.ROADS_SMALL_HV + "Panel");

                        if (button != null && panel != null)
                        {
                            // TODO: Set the thumbnail
                            button.zOrder = 1;
                            panel.zOrder = 1;

                            button.atlas = ToolsUnity.LoadMenuThumbnails(
                                "SubBar" + Menus.ROADS_SMALL_HV,
                                @"AdditionnalMenus\Textures\RoadsSmallHV.png");

                            menuInstalled = true;
                            break;
                        }
                    }

                    if (menuInstalled)
                    {
                        Debug.Log("NExt: New Menus have been installed");
                    }
#if DEBUG
                    else
                    {
                        Debug.Log("NExt: Something has happened, new menus have not been installed");
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
    }
}
