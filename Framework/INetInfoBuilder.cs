using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NetworkExtensions.Framework
{
    public interface INetInfoBuilder
    {
        int Priority { get; }

        string PrefabName { get; }
        string Name { get; }
        string CodeName { get; }

        string Description { get; }

        string UICategory { get; }
        string ThumbnailsPath { get; }
        string InfoTooltipPath { get; }

        NetInfoVersion SupportedVersions { get; }
        string GetPrefabName(NetInfoVersion version);

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
            var mainInfo = builder.BuildVersion(NetInfoVersion.Ground, null, newNetInfos);
            mainInfo.m_UIPriority = builder.Priority;

            if (!builder.CodeName.IsNullOrWhiteSpace() && !builder.ThumbnailsPath.IsNullOrWhiteSpace())
            {
                var thumbnails = ToolsUnity.LoadThumbnails(builder.CodeName, builder.ThumbnailsPath);
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

            builder.BuildVersion(NetInfoVersion.Elevated, info => mainInfoAI.m_elevatedInfo = info, newNetInfos);
            builder.BuildVersion(NetInfoVersion.Bridge, info => mainInfoAI.m_bridgeInfo = info, newNetInfos);
            builder.BuildVersion(NetInfoVersion.Tunnel, info => mainInfoAI.m_tunnelInfo = info, newNetInfos);
            builder.BuildVersion(NetInfoVersion.Slope, info => mainInfoAI.m_slopeInfo = info, newNetInfos);

            Debug.Log(string.Format("NExt: Initialized {0}", builder.Name));

            return newNetInfos;
        }

        private static NetInfo BuildVersion(this INetInfoBuilder builder, NetInfoVersion version, Action<NetInfo> assign, ICollection<NetInfo> holdingCollection)
        {
            if (builder.SupportedVersions.HasFlag(version))
            {
                var completePrefabName = builder.GetPrefabName(version);
                var completeName = builder.GetNewName(version);

                var info = ToolsCSL
                    .FindPrefab<NetInfo>(completePrefabName)
                    .Clone(completeName);

                info.SetUICategory(builder.UICategory);
                builder.BuildUp(info, version);

                if (assign != null)
                {
                    assign(info);
                }

                holdingCollection.Add(info);

                return info;
            }

            return null;
        }

        public static void DefineLocalization(this INetInfoBuilder builder, Locale locale)
        {
            locale.AddLocalizedString(new Locale.Key
            {
                m_Identifier = "NET_TITLE",
                m_Key = builder.Name
            }, builder.Name);

            locale.AddLocalizedString(new Locale.Key()
            {
                m_Identifier = "NET_DESC",
                m_Key = builder.Name
            }, builder.Description);
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
