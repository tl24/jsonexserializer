using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonExSerializer.MetaData;
using JsonExSerializer.TypeConversion;
using System.Linq.Expressions;
using JsonExSerializer.Framework;
using System.Reflection;

namespace JsonExSerializer
{
    public static class MetaDataExtensions
    {
        /// <summary>
        /// Registers a type converter for a type
        /// </summary>
        /// <typeparam name="T">the type to register for</typeparam>
        /// <param name="typeSettings">the type settings object of the serializer</param>
        /// <param name="converter">the converter to register</param>
        public static void RegisterTypeConverter<T>(this ITypeSettings typeSettings, IJsonTypeConverter converter)
        {
            typeSettings.RegisterTypeConverter(typeof(T), converter);
        }

        /// <summary>
        /// Registers a type converter for a property
        /// </summary>
        /// <typeparam name="T">the type that contains the property</typeparam>
        /// <typeparam name="P">the type of the property</typeparam>
        /// <param name="typeSettings">the type settings object of the serializer</param>
        /// <param name="propertyExpression">An expression that references the property</param>
        /// <param name="converter">the converter to register</param>
        public static void RegisterTypeConverter<T, P>(this ITypeSettings typeSettings, Expression<Func<T, P>> propertyExpression, IJsonTypeConverter converter)
        {
            typeSettings.RegisterTypeConverter(typeof(T), ReflectionUtils.GetPropertyName(propertyExpression), converter);
        }

        /// <summary>
        /// Returns the type data for a type
        /// </summary>
        /// <typeparam name="T">the type to retrieve</typeparam>
        /// <param name="typeSettings">the type settings object</param>
        /// <returns>type meta data</returns>
        public static ITypeData<T> Type<T>(this ITypeSettings typeSettings)
        {
            return typeSettings.Type<T>();
        }

        /// <summary>
        /// Returns the property data for a type's property
        /// </summary>
        /// <typeparam name="T">the type that contains the property</typeparam>
        /// <typeparam name="P">the property type</typeparam>
        /// <param name="typeSettings">the type settings object</param>
        /// <param name="propertyExpression">linq expression that references the property</param>
        /// <returns>type meta data</returns>
        public static IPropertyData Property<T, P>(this ITypeSettings typeSettings, Expression<Func<T, P>> propertyExpression)
        {
            MemberInfo memberInfo = ReflectionUtils.GetMemberInfo(propertyExpression);
            IPropertyData property = typeSettings[typeof(T)].FindPropertyByName(memberInfo.Name);
            return property;
        }

        /// <summary>
        /// ignores a property for a type
        /// </summary>
        /// <typeparam name="T">the type that contains the property</typeparam>
        /// <typeparam name="P">the property type</typeparam>
        /// <param name="typeSettings">the type settings object</param>
        /// <param name="propertyExpression">linq expression that references the property</param>
        /// <returns>type meta data</returns>
        public static IPropertyData IgnoreProperty<T, P>(this ITypeSettings typeSettings, Expression<Func<T, P>> propertyExpression)
        {
            return typeSettings.Property(propertyExpression).Configure(p => p.Ignored = true);
        }

        /// <summary>
        /// Configures several attributes on a property
        /// </summary>
        /// <param name="property">the property metadata</param>
        /// <param name="configureAction">action to configure the property</param>
        /// <returns>property meta data</returns>
        public static IPropertyData Configure(this IPropertyData property, Action<IPropertyData> configureAction)
        {
            configureAction(property);
            return property;
        }

        /// <summary>
        /// Returns the type data for a type
        /// </summary>
        /// <typeparam name="T">the type to retrieve</typeparam>
        /// <param name="serializerSettings">the settings for the serializer</param>
        /// <returns>type meta data</returns>
        public static ITypeData<T> Type<T>(this ISerializerSettings serializerSettings)
        {
            return serializerSettings.Types.Type<T>();
        }
    }
}
