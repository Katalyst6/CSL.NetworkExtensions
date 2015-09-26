namespace CSL.NetworkExtensions.Framework.ModParts
{
    public interface IActivablePart : IModPart
    {
        bool IsEnabled { get; set; }

        int OptionsPriority { get; }

        string DisplayName { get; }
    }

    public static class ActivableModPartExtensions
    {
        public static string GetSerializableName(this IActivablePart nExtModPart)
        {
            return nExtModPart.Name.Replace(" ", "_");
        }
    }
}
