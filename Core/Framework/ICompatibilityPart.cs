using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkExtensions.Framework
{
    public interface ICompatibilityPart : IModPart
    {
        bool IsPluginActive { get; }

        void Setup(IEnumerable<NetInfo> newRoads);
    }
}
