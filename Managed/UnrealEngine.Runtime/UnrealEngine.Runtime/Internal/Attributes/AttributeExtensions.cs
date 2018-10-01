using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnrealEngine.Runtime
{
    public static class AttributeExtensions
    {
        // Adding HasCustomAttribute as GetCustomAttribute<> will throw an exception if there is more
        // than one attribute found which isn't desirable

        public static bool HasCustomAttribute<T>(this MemberInfo element, bool inherit) where T : Attribute
        {
            object[] attributes = element.GetCustomAttributes(typeof(T), inherit);
            return attributes != null && attributes.Length > 0;
        }

        public static bool HasCustomAttribute<T>(this ParameterInfo element, bool inherit) where T : Attribute
        {
            object[] attributes = element.GetCustomAttributes(typeof(T), inherit);
            return attributes != null && attributes.Length > 0;
        }

        public static bool HasCustomAttribute<T>(this Assembly element, bool inherit) where T : Attribute
        {
            object[] attributes = element.GetCustomAttributes(typeof(T), inherit);
            return attributes != null && attributes.Length > 0;
        }

        public static bool HasCustomAttribute<T>(this Module element, bool inherit) where T : Attribute
        {
            object[] attributes = element.GetCustomAttributes(typeof(T), inherit);
            return attributes != null && attributes.Length > 0;
        }
    }
}
