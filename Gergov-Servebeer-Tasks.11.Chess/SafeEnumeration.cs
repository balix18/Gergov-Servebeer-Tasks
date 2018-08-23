using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Gergov_Servebeer_Tasks._11.Chess
{
    public abstract class SafeEnumeration<TEnum, TSelf>
        where TEnum : Enum
        where TSelf : SafeEnumeration<TEnum, TSelf>
    {
        public string Name { get; }
        public int Id { get; }

        protected SafeEnumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => $"Id: {Id}, Name: {Name}";

        public static IEnumerable<T> GetAll<T>()
            where T : SafeEnumeration<TEnum, TSelf>
        {
            var matches = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(propertyInfo => typeof(TSelf).IsAssignableFrom(propertyInfo.PropertyType))
                .Select(propertyInfo => propertyInfo.GetValue(null))
                .Cast<T>()
                .ToList();

            if (matches.Count == 0) throw new Exception("Parent class must create at least one public static property, which type inherits from TSelf");

            return matches;
        }
    }

    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the int value of current enum value
        /// </summary>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        public static int IntValue(this Enum enumeration) => Convert.ToInt32(enumeration);
    }
}
