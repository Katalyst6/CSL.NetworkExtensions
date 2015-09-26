﻿using System.Collections.Generic;
using System.Linq;

namespace CSL.ExtensionFramework
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> TrimNull<T>(this IEnumerable<T> array)
            where T: class
        {
            return array.Where(element => element != null).ToArray();
        }
    }
}
