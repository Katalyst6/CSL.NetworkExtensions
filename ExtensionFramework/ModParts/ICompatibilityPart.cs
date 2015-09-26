using System.Collections.Generic;

namespace CSL.ExtensionFramework.ModParts
{
    public interface ICompatibilityPart : IModPart
    {
        bool IsPluginActive { get; }

        void Setup(IEnumerable<NetInfo> newRoads);
    }
}
