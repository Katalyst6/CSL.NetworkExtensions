using System;
using System.Reflection;
using Externals;
using NetworkExtensions.AI;
using NetworkExtensions.Menus;
using UnityEngine;

#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions.Install
{
    public class Initializer : Installer
    {
#if DEBUG
        private int _frameNb = 0;
#endif

        protected override bool ValidatePrerequisites()
        {
#if DEBUG
            if (_frameNb++ < 20) // Giving some time for the UI to refresh **NB. Putting this constant higher than 100 causes wierd behavior**
            {
                return false;
            }
#endif

            return true;
        }

        protected override void Install()
        {
            var version = GetType().Assembly.GetName().Version;
            Debug.Log(string.Format("NExt: Version {0}", version));

            InstallRedirections();
        }

        private static bool s_redirectionsInstalled;

        private void InstallRedirections()
        {
            if (s_redirectionsInstalled)
            {
                return;
            }

            try
            {
                InstallRoadsGroupPanelRedirect();
            }
            catch (Exception ex)
            {
                Debug.Log("NExt: Crashed-RedirectionsInstall");
                Debug.Log("NExt: " + ex.Message);
                Debug.Log("NExt: " + ex.ToString());
            }
            finally
            {
                s_redirectionsInstalled = true;
            }
        }

        private static IDisposable s_rgpRedirect;

        private void InstallRoadsGroupPanelRedirect()
        {
            var originalMethod = typeof(RoadsGroupPanel).GetMethod("GetCategoryOrder", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (originalMethod == null)
            {
                Debug.Log("NExt: Cannot find the GetCategoryOrder original method, continuing");
                return;
            }

            var newMethod = typeof(RoadsGroupPanelRedirect).GetMethod("GetCategoryOrder", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (newMethod == null)
            {
                Debug.Log("NExt: Cannot find the GetCategoryOrder new method, continuing");
                return;
            }

            s_rgpRedirect = originalMethod.RedirectTo(newMethod); // TODO: Make that "uninstallable"
        }

        private static IDisposable s_raiRedirect;

        private void InstallRoadAIRedirect()
        {
            var originalMethod = typeof(RoadAI).GetMethod("CreateZoneBlocks", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (originalMethod == null)
            {
                Debug.Log("NExt: Cannot find the GetCategoryOrder original method, continuing");
                return;
            }

            var newMethod = typeof(RoadAIRedirect).GetMethod("CreateZoneBlocks", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (newMethod == null)
            {
                Debug.Log("NExt: Cannot find the GetCategoryOrder new method, continuing");
                return;
            }

            s_raiRedirect = originalMethod.RedirectTo(newMethod); // TODO: Make that "uninstallable"
        }
    }
}
