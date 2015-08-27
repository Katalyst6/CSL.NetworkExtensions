using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetworkExtensions.Framework
{
    public static class NodeExtensions
    {
        public static NetInfo.Node Clone(this NetInfo.Node node)
        {
            return node.ShallowClone();
        }
    }
}
