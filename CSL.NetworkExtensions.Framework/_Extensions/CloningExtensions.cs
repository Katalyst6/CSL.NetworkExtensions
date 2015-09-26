﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework;

namespace CSL.NetworkExtensions.Framework
{
    public static class CloningExtensions
    {
        public static T ShallowClone<T>(this T source, params string[] omitMembers)
            where T : new()
        {
            var clone = new T();

            foreach (FieldInfo f in typeof(T).GetAllFields())
            {
                if (omitMembers.Contains(f.Name))
                {
                    continue;
                }

                f.SetValue(clone, f.GetValue(source));
            }

            return clone;
        }

        public static T CloneMembersFrom<T>(this T destination, T source, params string[] omitMembers)
            where T : new()
        {
            foreach (FieldInfo f in destination.GetType().GetAllFields(true))
            {
                if (omitMembers.Contains(f.Name))
                {
                    continue;
                }

                f.SetValue(destination, f.GetValue(source));
            }

            return destination;
        }

        private static readonly IEnumerable<Type> s_simpleTypes = new HashSet<Type>
        {
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(char),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(short),
            typeof(ushort),
            typeof(string),
            typeof(Guid),
            typeof(Enum),
        };

        public static T CloneSimpleMembersFrom<T>(this T destination, T source)
            where T : new()
        {

            foreach (FieldInfo f in destination.GetType().GetAllFields(true).OrderBy(x => x.Name))
            {
                if (s_simpleTypes.Contains(f.FieldType))
                {
                    continue;
                }

                f.SetValue(destination, f.GetValue(source));
            }

            return destination;
        }
    }
}
