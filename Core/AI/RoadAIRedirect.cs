using System;
using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;

#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions.AI
{
    public partial class RoadAIRedirect : RoadBaseAI
    {
        private void CreateZoneBlocks(ushort segment, ref NetSegment data)
        {
            try
            {
                switch (this.m_info.name)
                {
                    case "Alley2L":
                        CreateZoneBlocksNew(this.m_info, segment, ref data);
                        break;

                    default:
                        CreateZoneBlocksOriginal(this.m_info, segment, ref data);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("NExt: Crashed-CreateZoneBlocks");
                Debug.Log("NExt: " + ex.Message);
                Debug.Log("NExt: " + ex.ToString());
            }
        }
    }
}
