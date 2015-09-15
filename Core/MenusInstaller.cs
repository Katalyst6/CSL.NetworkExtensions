using ColossalFramework.UI;
using NetworkExtensions.Framework;
using System;
using Debug = NetworkExtensions.Framework.Debug;
using Object = UnityEngine.Object;

namespace NetworkExtensions
{
    public class MenusInstaller
    {
        private static bool _isInitialized; 

        private static bool ValidateAdditionalMenusPrerequisites()
        {
            var roadsGroupPanels = Object.FindObjectsOfType<RoadsGroupPanel>();
            if (roadsGroupPanels == null)
            {
                return false;
            }

            //if (roadsGroupPanels.Length == 0)
            //{
            //    return false;
            //}

            //var templateFound = roadsGroupPanels
            //    .Select(panel => panel.Find<UIButton>("RoadsSmall"))
            //    .Any(button => button != null);

            //if (!templateFound)
            //{
            //    return false;
            //}

            return true;
        }

        public static void Execute()
        {
            if (_isInitialized)
            {
                return;
            }

            if (ValidateAdditionalMenusPrerequisites())
            {
                Debug.Log("NExt: Ready for New Menus");

                Loading.QueueAction(() =>
                {
                    try
                    {
                        Debug.Log("NExt: Initialized New Menus");

                        foreach (var panel in Object.FindObjectsOfType<RoadsGroupPanel>())
                        {
                            panel.PopulateGroups();

                            var button = panel.Find<UIButton>("RoadOW");
                            if (button == null)
                            {
                                Debug.Log("NExt: Button not found");
                            }
                            else
                            {
                                Debug.Log("NExt: Button found!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("NExt: Crashed-Initialized New Menus");
                        Debug.Log("NExt: " + ex.Message);
                        Debug.Log("NExt: " + ex.ToString());
                    }
                });

                _isInitialized = true;
            }
            else
            {
                Debug.Log("NExt: Not Ready for New Menus");
            }
        }
    }
}
