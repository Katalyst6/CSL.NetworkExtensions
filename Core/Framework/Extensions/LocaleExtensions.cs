using ColossalFramework.Globalization;

namespace NetworkExtensions.Framework
{
    public static class LocaleExtensions
    {
        public static void AddCategoryLocalizedString(this Locale locale)
        {
            locale.AddLocalizedString(new Locale.Key()
            {
                m_Identifier = "MAIN_CATEGORY",
                m_Key = Mod.NEXT_CATEGORY_NAME
            }, Mod.NEXT_CATEGORY_NAME);
        }
    }
}
