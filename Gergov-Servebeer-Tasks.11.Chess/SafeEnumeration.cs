using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Gergov_Servebeer_Tasks._11.Chess
{
    public abstract class SafeEnumeration<TEnum, TSelf>
        : IEquatable<SafeEnumeration<TEnum, TSelf>>
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
                .Where(propertyInfo => typeof(SafeEnumeration<TEnum, TSelf>).IsAssignableFrom(propertyInfo.PropertyType))
                .Select(propertyInfo => propertyInfo.GetValue(null))
                .Cast<T>()
                .ToList();

            if (matches.Count == 0) throw new Exception("Parent class must create at least one public static property, which type inherits from TSelf");

            return matches;
        }

        public static bool operator ==(SafeEnumeration<TEnum, TSelf> obj1, SafeEnumeration<TEnum, TSelf> obj2)
        {
            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            if (ReferenceEquals(obj1, null) || ReferenceEquals(obj2, null))
            {
                return false;
            }

            return obj1.Equals(obj2);
        }

        public static bool operator !=(SafeEnumeration<TEnum, TSelf> obj1, SafeEnumeration<TEnum, TSelf> obj2)
        {
            return !(obj1 == obj2);
        }

        public bool Equals(SafeEnumeration<TEnum, TSelf> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id.Equals(other.Id) && Name.Equals(other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return typeof(SafeEnumeration<TEnum, TSelf>).IsAssignableFrom(obj.GetType()) && Equals((SafeEnumeration<TEnum, TSelf>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                result = (result * 397) ^ Id;
                result = (result * 397) ^ Name.GetHashCode();
                return result;
            }
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
