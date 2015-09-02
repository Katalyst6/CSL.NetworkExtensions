using System;
using ColossalFramework;
using ColossalFramework.Globalization;
using NetworkExtensions.Framework;
using UnityEngine;

#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions
{
    public partial class ModInitializer
    {
        //private void Initialize()
        //{
        //    if (!_initializedPanels & _initializedNetworkInfo)
        //    {
        //        if (ValidateSetPanelNamePrerequisites())
        //        {
        //            try
        //            {
        //                Debug.Log("NExt: Set Panels");
        //                SetPanelName();
        //            }
        //            catch (Exception ex)
        //            {
        //                Debug.Log("Crashed-SetPanel");
        //                Debug.Log("NExt: " + ex.Message);
        //                Debug.Log("NExt: " + ex.ToString());
        //            }
        //            finally
        //            {
        //                _initializedPanels = true;
        //            }
        //        }
        //    }

        //    _finishedInit =
        //        _initializedNetworkInfo &&
        //        _initializedLocalization;
        //     && _initializedPanels;
        //}

        //private bool ValidateSetPanelNamePrerequisites()
        //{
        //    var roadsGroupPanel = FindObjectsOfType<RoadsGroupPanel>();
        //    if (roadsGroupPanel == null)
        //    {
        //        return false;
        //    }

        //    if (roadsGroupPanel.Length == 0)
        //    {
        //        return false;
        //    }

        //    Debug.Log("NExt: Found roads panels, continuing");
        //    return true;
        //}

        //private void SetPanelName()
        //{
        //    foreach (var panel in FindObjectsOfType<RoadsGroupPanel>())
        //    {
        //        var button = panel.Find<UIButton>(NEXT_CATEGORY_NAME);
        //        if (button != null && button.name == NEXT_CATEGORY_NAME)
        //        {
        //            button.text = "EXT";
        //            Debug.Log("NExt: Found tab button & changed text in roads panel");
        //            break;
        //        }
        //    }
        //}
    }
}