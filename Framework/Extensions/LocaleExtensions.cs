using ColossalFramework.Globalization;

namespace NetworkExtensions.Framework.Extensions
{
    public static class LocaleExtensions
    {
        public static void AddPrefabLocalizedStrings(this Locale locale, string name, string desc)
        {
            locale.AddLocalizedString(new Locale.Key
            {
                m_Identifier = "NET_TITLE",
                m_Key = name
            }, name);

            locale.AddLocalizedString(new Locale.Key()
            {
                m_Identifier = "NET_DESC",
                m_Key = name
            }, desc);
        }

        public static void AddCategoryLocalizedString(this Locale locale)
        {
            locale.AddLocalizedString(new Locale.Key()
            {
                m_Identifier = "MAIN_CATEGORY",
                m_Key = ModBehaviour.NEXT_CATEGORY_NAME
            }, ModBehaviour.NEXT_CATEGORY_NAME);
        }
    }
}
