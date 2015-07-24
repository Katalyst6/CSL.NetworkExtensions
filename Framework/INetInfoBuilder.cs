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
        public static IEnumerable<NetInfo> Build(this INetInfoBuilder builder, GameObject parentObject)
        {
            var newNetInfos = new List<NetInfo>();


            // Ground version--------------------------------------------------
            var mainInfo = builder.BuildVersion(parentObject, NetInfoVersion.Ground, null, newNetInfos);
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


            //// Other versions -------------------------------------------------

            var mainInfoAI = mainInfo.GetComponent<RoadAI>();

            builder.BuildVersion(parentObject, NetInfoVersion.Elevated, info => mainInfoAI.m_elevatedInfo = info, newNetInfos);
            builder.BuildVersion(parentObject, NetInfoVersion.Bridge, info => mainInfoAI.m_bridgeInfo = info, newNetInfos);
            builder.BuildVersion(parentObject, NetInfoVersion.Tunnel, info => mainInfoAI.m_tunnelInfo = info, newNetInfos);
            builder.BuildVersion(parentObject, NetInfoVersion.Slope, info => mainInfoAI.m_slopeInfo = info, newNetInfos);

            Debug.Log(string.Format("NExt: Initialized {0}", builder.Name));

            return newNetInfos;
        }

        private static NetInfo BuildVersion(this INetInfoBuilder builder, GameObject parentObject, NetInfoVersion version, Action<NetInfo> assign, ICollection<NetInfo> holdingCollection)
        {
            if (builder.SupportedVersions.HasFlag(version))
            {
                var completePrefabName = builder.GetPrefabInfoVersionCompleteName(version);
                var completeName = builder.GetNewInfoVersionCompleteName(version);

                var info = ToolsCSL
                    .FindPrefab<NetInfo>(completePrefabName)
                    .Clone(completeName);

                info.gameObject.transform.SetParent(parentObject.transform);
                info.SetUICategory(builder.UICategory);
                //builder.BuildUp(info, version);

                if (assign != null)
                {
                    assign(info);
                }

                holdingCollection.Add(info);

                //var mainInfoAI = info.GetComponent<RoadAI>();
                //holdingCollection.Add(mainInfoAI.m_elevatedInfo);
                //holdingCollection.Add(mainInfoAI.m_bridgeInfo);
                //holdingCollection.Add(mainInfoAI.m_tunnelInfo);
                //holdingCollection.Add(mainInfoAI.m_slopeInfo);

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

        private static string GetPrefabInfoVersionCompleteName(this INetInfoBuilder builder, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    return builder.PrefabName;

                case NetInfoVersion.Elevated:
                    return builder.PrefabName + " " + NetInfoVersion.Elevated;

                case NetInfoVersion.Bridge:
                    return builder.PrefabName + " " + NetInfoVersion.Bridge;

                case NetInfoVersion.Tunnel:
                    return builder.PrefabName + " Road Tunnel";

                case NetInfoVersion.Slope:
                    return builder.PrefabName + " Road Slope";

                default:
                    throw new ArgumentOutOfRangeException("version");
            }
        }

        private static string GetNewInfoVersionCompleteName(this INetInfoBuilder builder, NetInfoVersion version)
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
