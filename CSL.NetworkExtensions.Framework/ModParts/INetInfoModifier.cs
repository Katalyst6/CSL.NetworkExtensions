namespace CSL.NetworkExtensions.Framework.ModParts
{
    public interface INetInfoModifier : IActivablePart
    {
        void ModifyExistingNetInfo();
    }
}
