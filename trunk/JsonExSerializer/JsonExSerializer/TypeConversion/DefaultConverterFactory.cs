using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;
using System.Reflection;
using System.ComponentModel;

namespace JsonExSerializer.TypeConversion
{
    /// <summary>
    /// The default type converter factory, which handles registering types directly,
    /// JsonConverterAttribute, and System.ComponentModel methods of declaring type converters.
    /// </summary>
    public class DefaultConverterFactory : ITypeConverterFactory
    {
        private IDictionary<MemberInfo, IJsonTypeConverter> _registeredTypes;

        public DefaultConverterFactory()
        {
            _registeredTypes = new Dictionary<MemberInfo, IJsonTypeConverter>();
        }

        /// <summary>
        /// Register a type converter for the given type
        /// </summary>
        /// <param name="typeToConvert">the object that will be converted</param>
        /// <param name="converter">the converter instance</param>
        public void RegisterConverter(Type typeToConvert, IJsonTypeConverter converter)
        {
            if (converter != null && typeToConvert != null)
            {
                _registeredTypes[typeToConvert] = converter;
            }
        }

        /// <summary>
        /// Register a type converter for the given property of a class
        /// </summary>
        /// <param name="property">the property that will be converted</param>
        /// <param name="converter">the converter instance</param>
        public void RegisterConverter(PropertyInfo property, IJsonTypeConverter converter)
        {
            if (converter != null && property != null)
            {
                _registeredTypes[property] = converter;
            }
        }

        /// <summary>
        /// Checks to see if this converter factory can produce a converter for the given type
        /// </summary>
        /// <param name="forType">the type to check</param>
        /// <returns>true if this factory can produce a converter for the type</returns>
        public bool HasConverter(Type forType)
        {
            bool result = HasConverter((MemberInfo)forType);
            if (!result)
            {
                // don't use converters for primitives and strings
                if (forType.IsPrimitive || forType == typeof(string))
                {
                    result = false;
                }
                else
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(forType);
                    result = converter.CanConvertFrom(typeof(string)) && converter.CanConvertTo(typeof(string));
                }
            }
            return result;
        }

        /// <summary>
        /// Checks to see if this converter factory can produce a converter for the object property
        /// </summary>
        /// <param name="forProperty">the property to check</param>
        /// <returns>true if this factory can produce a converter for the property</returns>
        public bool HasConverter(PropertyInfo forProperty)
        {
            return HasConverter((MemberInfo)forProperty);
        }

        /// <summary>
        /// Internal method to check for a converter for both Type and PropertyInfo, but will
        /// not check for a System.ComponentModel TypeConverter.
        /// </summary>
        /// <param name="member">the object to check for, either Type or PropertyInfo</param>
        /// <returns>true if it has a converter</returns>
        private bool HasConverter(MemberInfo member)
        {
            return (_registeredTypes.ContainsKey(member)
            || (member.IsDefined(typeof(JsonConvertAttribute), false)));
        }

        /// <summary>
        /// Gets a Converter for the property specified
        /// </summary>
        /// <param name="forProperty">the property to convert</param>
        /// <returns>a json type converter</returns>
        public IJsonTypeConverter GetConverter(PropertyInfo forProperty)
        {
            return GetConverter((MemberInfo)forProperty);
        }

        /// <summary>
        /// Gets a Converter for the type specified.
        /// </summary>
        /// <param name="forType">the type to convert</param>
        /// <returns>a json type converter</returns>
        public IJsonTypeConverter GetConverter(Type forType)
        {
            IJsonTypeConverter converter = GetConverter((MemberInfo)forType);
            if (converter != null)
            {
                return converter;
            }
            else
            {
                // System.ComponentModel.TypeConverter
                converter = new TypeConverterAdapter(TypeDescriptor.GetConverter(forType));
                _registeredTypes[forType] = converter;
                return converter;
            }
        }

        /// <summary>
        /// Internal method to construct a type converter
        /// </summary>
        /// <param name="forMember"></param>
        /// <returns></returns>
        private IJsonTypeConverter GetConverter(MemberInfo forMember)
        {
            if (_registeredTypes.ContainsKey(forMember))
            {
                return _registeredTypes[forMember];
            }
            else if (forMember.IsDefined(typeof(JsonConvertAttribute), true))
            {
                // just one for now, but later support chaining of converters
                JsonConvertAttribute convAttr = (JsonConvertAttribute) forMember.GetCustomAttributes(typeof(JsonConvertAttribute), false)[0];
                IJsonTypeConverter converter = (IJsonTypeConverter) Activator.CreateInstance(convAttr.Converter);
                _registeredTypes[forMember] = converter;
                // should we register it?
                return converter;
            }
            else
            {
                return null;
            }
        }
    }
}
