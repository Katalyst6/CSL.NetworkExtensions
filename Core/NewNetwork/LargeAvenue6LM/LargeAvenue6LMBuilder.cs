using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using CSL.ExtensionFramework;
using CSL.ExtensionFramework.ModParts;

namespace NetworkExtensions.NewNetwork.LargeAvenue6LM
{
    public class LargeAvenue6LMBuilder : ActivablePart, INetInfoBuilder
    {
        public int OptionsPriority { get { return 25; } }
        public int Priority { get { return 25; } }

        public string PrefabName { get { return NetInfos.Vanilla.AVENUE_4L; } }
        public string Name { get { return "Large Avenue M"; } }
        public string DisplayName { get { return "Six-Lane Road with Median"; } }
        public string CodeName { get { return "LARGEAVENUE_6LM"; } }
        public string Description { get { return "A six-lane road. Supports heavy traffic."; } }
        public string UICategory { get { return "RoadsLarge"; } }
        
        public string ThumbnailsPath    { get { return @"NewNetwork\LargeAvenue6LM\thumbnails.png"; } }
        public string InfoTooltipPath   { get { return @"NewNetwork\LargeAvenue6LM\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            // I$|H3lls7rik3r sandbox
        }
    }
}
