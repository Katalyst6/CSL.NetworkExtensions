using System;
using System.Collections.Generic;
using ColossalFramework;

namespace NetworkExtensions.Framework
{
    public interface INetInfoBuilder : IActivablePart
    {
        int Priority { get; }

        string PrefabName { get; }
        string CodeName { get; }
        string Description { get; }

        string UICategory { get; }
        string ThumbnailsPath { get; }
        string InfoTooltipPath { get; }

        NetInfoVersion SupportedVersions { get; }

        void BuildUp(NetInfo info, NetInfoVersion version);
    }

    [Flags]
    public enum NetInfoVersion
    {
        Ground = 0, //By default
        Elevated = 1,
        Bridge = 2,
        Tunnel = 4,
        Slope = 8,
        All = 15
    }

    public static class NetInfoBuilderExtensions
    {
        public static IEnumerable<NetInfo> Build(this INetInfoBuilder builder)
        {
            var newNetInfos = new List<NetInfo>();


            // Ground version--------------------------------------------------
            var mainInfo = builder.BuildVersion(NetInfoVersion.Ground, newNetInfos);
            mainInfo.m_UIPriority = builder.Priority;

            if (!builder.CodeName.IsNullOrWhiteSpace() && !builder.ThumbnailsPath.IsNullOrWhiteSpace())
            {
                var thumbnails = ToolsUnity.LoadToolThumbnails(builder.CodeName, builder.ThumbnailsPath);
                mainInfo.m_Atlas = thumbnails;
                mainInfo.m_Thumbnail = thumbnails.name;
            }

            if (!builder.CodeName.IsNullOrWhiteSpace() && !builder.InfoTooltipPath.IsNullOrWhiteSpace())
            {
                var infoTips = ToolsUnity.LoadInfoTooltip(builder.CodeName, builder.InfoTooltipPath);
                mainInfo.m_InfoTooltipAtlas = infoTips;
                mainInfo.m_InfoTooltipThumbnail = infoTips.name;
            }


            // Other versions -------------------------------------------------
            var mainInfoAI = mainInfo.GetComponent<RoadAI>();

            mainInfoAI.m_elevatedInfo = builder.BuildVersion(NetInfoVersion.Elevated, newNetInfos);
            mainInfoAI.m_bridgeInfo = builder.BuildVersion(NetInfoVersion.Bridge, newNetInfos);
            mainInfoAI.m_tunnelInfo = builder.BuildVersion(NetInfoVersion.Tunnel, newNetInfos);
            mainInfoAI.m_slopeInfo = builder.BuildVersion(NetInfoVersion.Slope, newNetInfos);

            return newNetInfos;
        }

        private static NetInfo BuildVersion(this INetInfoBuilder builder, NetInfoVersion version, ICollection<NetInfo> holdingCollection)
        {
            if (builder.SupportedVersions.HasFlag(version))
            {
                var completePrefabName = VanillaNetInfos.GetPrefabName(builder.PrefabName, version);
                var completeName = builder.GetNewName(version);

                var info = ToolsCSL
                    .FindPrefab<NetInfo>(completePrefabName)
                    .Clone(completeName);

                info.SetUICategory(builder.UICategory);
                builder.BuildUp(info, version);

                holdingCollection.Add(info);

                return info;
            }
            else
            {
                return null;
            }
        }

        private static string GetNewName(this INetInfoBuilder builder, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    return builder.Name;

                default:
                    return builder.Name + " " + version;
            }
        }
    }
}
