﻿using System;
using ColossalFramework;
using ColossalFramework.Globalization;
using CSL.NetworkExtensions.Framework;
using UnityEngine;

#if DEBUG
using Debug = CSL.NetworkExtensions.Framework.Debug;
#endif

namespace CSL.RoadExtensions.Install
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
                    //Debug.Log("REx: Localization");
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
                    Debug.Log("REx: Crashed-Localization");
                    Debug.Log("REx: " + ex.Message);
                    Debug.Log("REx: " + ex.ToString());
                }

                Done = true;
            });
        }
    }
}