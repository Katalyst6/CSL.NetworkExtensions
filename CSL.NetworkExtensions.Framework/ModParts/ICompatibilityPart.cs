using System.Collections.Generic;

namespace CSL.NetworkExtensions.Framework.ModParts
{
    public interface ICompatibilityPart : IModPart
    {
        bool IsPluginActive { get; }

        void Setup(IEnumerable<NetInfo> newRoads);
    }
}
