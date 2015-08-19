using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkExtensions.Framework
{
    public interface IModPart
    {
        bool IsEnabled { get; set; }

        int OptionsPriority { get; }

        string Name { get; }
    }

    public static class ModPartExtensions
    {
        public static string GetSerializableName(this IModPart nExtModPart)
        {
            return nExtModPart.Name.Replace(" ", "_");
        }
    }
}
