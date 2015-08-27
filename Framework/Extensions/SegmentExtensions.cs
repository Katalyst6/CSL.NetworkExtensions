using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetworkExtensions.Framework
{
    public static class SegmentExtensions
    {
        public static NetInfo.Segment Clone(this NetInfo.Segment segment)
        {
            return segment.ShallowClone();
        }
    }
}
