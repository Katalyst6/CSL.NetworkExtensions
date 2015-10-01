﻿using System;
using CSL.NetworkExtensions.Framework;
using UnityEngine;

#if DEBUG
using Debug = CSL.NetworkExtensions.Framework.Debug;
#endif


namespace CSL.RoadExtensions.Install
{
    public class AssetsInstaller : Installer
    {
        public static bool Done { get; private set; } // Only one Assets installation throughout the application

        protected override bool ValidatePrerequisites()
        {
            return true;
        }

        protected override void Install()
        {
            if (Done) // Only one Assets installation throughout the application
            {
                return;
            }

            foreach (var action in AssetManager.instance.CreateLoadingSequence(Mod.GetPath()))
            {
                var localAction = action;

                Loading.QueueAction(() =>
                {
                    try
                    {
                        localAction();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("REx: Crashed-AssetsInstaller");
                        Debug.Log("REx: " + ex.Message);
                        Debug.Log("REx: " + ex.ToString());
                    }
                });
            }

            Done = true;
        }
    }
}
