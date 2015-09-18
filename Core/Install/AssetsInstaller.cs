using System;
using NetworkExtensions.Framework;

namespace NetworkExtensions.Install
{
    public class AssetsInstaller : Installer
    {
        public static bool Done { get; private set; } //Only one Assets installation throughout the application

        protected override bool ValidatePrerequisites()
        {
            return true;
        }

        protected override void Install()
        {
            if (Done) //Only one Assets installation throughout the application
            {
                return;
            }

            foreach (var action in AssetManager.instance.CreateLoadingSequence())
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
                        Debug.Log("NExt: Crashed-AssetsInstaller");
                        Debug.Log("NExt: " + ex.Message);
                        Debug.Log("NExt: " + ex.ToString());
                    }
                });
            }

            Done = true;
        }
    }
}
