using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetworkExtensions.Framework
{
	public static class TypeExtensions
    {
        /// <summary>
        /// Returns all fields from this type, including static, private and inherited private fields.
        /// </summary>
        public static IEnumerable<FieldInfo> GetAllFieldsFromType(this Type type)
        {
            if (type == null)
                return Enumerable.Empty<FieldInfo>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;
            if (type.BaseType != null)
                return type.GetFields(flags).Concat(type.BaseType.GetAllFieldsFromType());
            else
                return type.GetFields(flags);
        }

		/// <summary>
		/// Searches for the field identified by the given name, regardless of type or accessibility.
		/// If it exists, it's returned.
		/// </summary>
		public static FieldInfo GetFieldByName(this Type type, string name)
		{
			return type.GetAllFieldsFromType().FirstOrDefault(p => p.Name == name);
		}
	}
}
