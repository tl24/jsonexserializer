using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;

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

        /// <summary>
        /// Tests whether two types are equivalent.  Types are equivalent if the
        /// types are equal, or if one type is the nullable type of the other type
        /// </summary>
        /// <param name="a">first type to test</param>
        /// <param name="b">second type to test</param>
        /// <returns>true if equivalent</returns>
        public static bool AreEquivalentTypes(Type a, Type b)
        {
            if (a.Equals(b))
                return true;

            if (IsNullableType(a))
                return AreEquivalentTypes(Nullable.GetUnderlyingType(a), b);

            if (IsNullableType(b))
                return AreEquivalentTypes(Nullable.GetUnderlyingType(b), a);

            // GetType() returns System.RuntimeType whereas typeof(T) returns System.Type
            if (a.GetType() == b || a == b.GetType())
                return true;

            return false;
        }

        /// <summary>
        /// Checks to see if the type is a nullable type
        /// </summary>
        /// <param name="checkType">the type to check</param>
        /// <returns>true if the type is a nullable type</returns>
        public static bool IsNullableType(Type checkType)
        {
            return (checkType.IsGenericType && typeof(Nullable<>).Equals(checkType.GetGenericTypeDefinition()));
        }

        /// <summary>
        /// Wraps an exception with the same type as the inner exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <returns></returns>
        public static Exception WrapException(string message, Exception innerException)
        {
            return (Exception) Activator.CreateInstance(innerException.GetType(), message, innerException);
        }

        public static MemberInfo GetMemberInfo<TSourceType, TProperty>(Expression<Func<TSourceType, TProperty>> bindExpression)
        {
            var lambda = (LambdaExpression)bindExpression;

            MemberExpression memberExpression = (MemberExpression)lambda.Body;
            return memberExpression.Member;
        }

        public static string GetPropertyName<TSourceType, TProperty>(Expression<Func<TSourceType, TProperty>> bindExpression)
        {
            return GetMemberInfo(bindExpression).Name;
        }
    }
}
