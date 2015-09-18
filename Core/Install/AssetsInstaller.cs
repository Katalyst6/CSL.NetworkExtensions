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

            Loading.QueueAction(() =>
            {
                try
                {
                    AssetManager.instance.FindAndLoadAllTextures();
                }
                catch (Exception ex)
                {
                    Debug.Log("NExt: Crashed-AssetsInstaller");
                    Debug.Log("NExt: " + ex.Message);
                    Debug.Log("NExt: " + ex.ToString());
                }

                Done = true;
            });
        }
    }
}
