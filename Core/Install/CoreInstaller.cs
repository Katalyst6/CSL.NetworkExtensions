#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions.Install
{
    public class CoreInstaller : Installer
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
        }
    }
}
