using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace JsonExSerializer.Framework
{
    public class ReflectionUtils
    {
        /// <summary>
        /// Returns the first attribute matching <typeparamref name="T"/> or null if none exist.
        /// </summary>
        /// <typeparam name="T">the custom attribute type</typeparam>
        /// <param name="provider">member with custom attributes</param>
        /// <param name="inherit">When true look up the hierarchy chain for inherited attributes</param>
        /// <returns>the first attribute</returns>
        public static T GetAttribute<T>(ICustomAttributeProvider provider, bool inherit) where T : Attribute
        {
            if (provider.IsDefined(typeof(T), inherit))
                return (T) provider.GetCustomAttributes(typeof(T), inherit)[0];
            else
                return null;
        }
    }
}
