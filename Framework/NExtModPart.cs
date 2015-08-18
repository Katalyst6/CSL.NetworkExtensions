using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkExtensions.Framework
{
    public abstract class NExtModPart : INExtModPart
    {
        public bool IsActive { get; set; }
    }
}
