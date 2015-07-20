using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using NetworkExtensions.Framework.Extensions;
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
        public static IEnumerable<NetInfo> Build(this INetInfoBuilder builder)
        {
            var newNetInfos = new List<NetInfo>();

            var thumbnails = builder.LoadThumbnails();
            var infoTips = builder.LoadInfoTooltip();

            // Ground version
            var mainInfo = ToolsCSL.CloneNetInfo(builder.GetPrefabNameForVersion(NetInfoVersion.Ground), builder.Name);

            mainInfo.m_UIPriority = builder.Priority;
            mainInfo.m_Atlas = thumbnails;
            mainInfo.m_Thumbnail = thumbnails.name;
            mainInfo.m_InfoTooltipAtlas = infoTips;
            mainInfo.m_InfoTooltipThumbnail = infoTips.name;

            mainInfo.SetUICategory(builder.UICategory);
            builder.BuildUp(mainInfo, NetInfoVersion.Ground);
            
            newNetInfos.Add(mainInfo);

            var mainInfoAI = mainInfo.GetComponent<RoadAI>();


            if (builder.SupportedVersions.HasFlag(NetInfoVersion.Elevated))
            {
                var elevatedInfo = ToolsCSL.CloneNetInfo(builder.GetPrefabNameForVersion(NetInfoVersion.Elevated), builder.Name + " " + NetInfoVersion.Elevated);

                elevatedInfo.SetUICategory(builder.UICategory);
                builder.BuildUp(elevatedInfo, NetInfoVersion.Elevated);

                mainInfoAI.m_elevatedInfo = elevatedInfo;
                newNetInfos.Add(elevatedInfo);
            }

            if (builder.SupportedVersions.HasFlag(NetInfoVersion.Bridge))
            {
                var bridgedInfo = ToolsCSL.CloneNetInfo(builder.GetPrefabNameForVersion(NetInfoVersion.Bridge), builder.Name + " " + NetInfoVersion.Bridge);

                bridgedInfo.SetUICategory(builder.UICategory);
                builder.BuildUp(bridgedInfo, NetInfoVersion.Bridge);

                mainInfoAI.m_bridgeInfo = bridgedInfo;
                newNetInfos.Add(bridgedInfo);
            }

            if (builder.SupportedVersions.HasFlag(NetInfoVersion.Tunnel))
            {
                var tunnelInfo = ToolsCSL.CloneNetInfo(builder.GetPrefabNameForVersion(NetInfoVersion.Tunnel), builder.Name + " " + NetInfoVersion.Tunnel);

                tunnelInfo.SetUICategory(builder.UICategory);
                builder.BuildUp(tunnelInfo, NetInfoVersion.Tunnel);

                mainInfoAI.m_tunnelInfo = tunnelInfo;
                newNetInfos.Add(tunnelInfo);
            }

            if (builder.SupportedVersions.HasFlag(NetInfoVersion.Slope))
            {
                var slopeInfo = ToolsCSL.CloneNetInfo(builder.GetPrefabNameForVersion(NetInfoVersion.Slope), builder.Name + " " + NetInfoVersion.Slope);

                slopeInfo.SetUICategory(builder.UICategory);
                builder.BuildUp(slopeInfo, NetInfoVersion.Slope);

                mainInfoAI.m_slopeInfo = slopeInfo;
                newNetInfos.Add(slopeInfo);
            }

            Debug.Log(string.Format("NExt: Initialized {0}", builder.Name));

            return newNetInfos;
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

        private static UITextureAtlas LoadThumbnails(this INetInfoBuilder builder)
        {
            return ToolsUnity.LoadThumbnails(builder.CodeName, builder.ThumbnailsPath);
        }

        private static UITextureAtlas LoadInfoTooltip(this INetInfoBuilder builder)
        {
            return ToolsUnity.LoadInfoTooltip(builder.CodeName, builder.InfoTooltipPath);
        }

        private static string GetPrefabNameForVersion(this INetInfoBuilder builder, NetInfoVersion version)
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
    }
}
