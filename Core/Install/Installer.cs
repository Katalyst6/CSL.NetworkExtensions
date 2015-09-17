using UnityEngine;

namespace NetworkExtensions.Install
{
    public delegate void InstallationCompletedEventHandler();

    public abstract class Installer : MonoBehaviour
    {
        public event InstallationCompletedEventHandler InstallationCompleted;

        private bool _doneWithInstall = false;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (!_doneWithInstall)
            {
                UpdateInternal();
            }
        }

        private void UpdateInternal()
        {
            if (!_doneWithInstall)
            {
                if (ValidatePrerequisites())
                {
                    Install();
                    _doneWithInstall = true;
                }
            }

            if (_doneWithInstall)
            {
                if (InstallationCompleted != null)
                {
                    InstallationCompleted();
                }
            }
        }

        protected abstract bool ValidatePrerequisites();
        protected abstract void Install();
    }
}
