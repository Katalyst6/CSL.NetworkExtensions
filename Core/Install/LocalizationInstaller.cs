using System;
using ColossalFramework;
using ColossalFramework.Globalization;
using NetworkExtensions.Framework;

#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions.Install
{
    public class LocalizationInstaller : Installer
    {
        public static bool Done { get; private set; } //Only one localization throughout the application

        protected override bool ValidatePrerequisites()
        {
            var localeManager = SingletonLite<LocaleManager>.instance;
            if (localeManager == null)
            {
                return false;
            }


            var localeField = typeof(LocaleManager).GetFieldByName("m_Locale");
            if (localeField == null)
            {
                return false;
            }


            var locale = (Locale)localeField.GetValue(localeManager);
            if (locale == null)
            {
                return false;
            }

            return true;
        }

        protected override void Install()
        {
            if (Done) //Only one localization throughout the application
            {
                return;
            }

            Loading.QueueAction(() =>
            {
                try
                {
                    //Debug.Log("NExt: Localization");
                    var locale = SingletonLite<LocaleManager>.instance.GetLocale();

                    locale.CreateMenuTitleLocalizedString(Menus.AdditionnalMenus.ROADS_SMALL_HV, "Small Heavy Roads");

                    foreach (var builder in Mod.NetInfoBuilders)
                    {
                        locale.CreateNetTitleLocalizedString(builder.Name, builder.DisplayName);
                        locale.CreateNetDescriptionLocalizedString(builder.Name, builder.Description);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("NExt: Crashed-Localization");
                    Debug.Log("NExt: " + ex.Message);
                    Debug.Log("NExt: " + ex.ToString());
                }

                Done = true;
            });
        }
    }
}