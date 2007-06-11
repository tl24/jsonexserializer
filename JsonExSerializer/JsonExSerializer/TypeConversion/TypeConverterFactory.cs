using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;
using System.Reflection;
using System.ComponentModel;

namespace JsonExSerializer.TypeConversion
{
    public class TypeConverterFactory
    {
        private IDictionary<MemberInfo, IJsonTypeConverter> _registeredTypes;

        public TypeConverterFactory()
        {
            _registeredTypes = new Dictionary<MemberInfo, IJsonTypeConverter>();
        }

        public void RegisterConverter(MemberInfo typeToConvert, IJsonTypeConverter converter)
        {
            if (converter != null && typeToConvert != null)
            {
                _registeredTypes[typeToConvert] = converter;
            }
        }

        public bool HasConverter(MemberInfo member)
        {
            if (_registeredTypes.ContainsKey(member)
            || (member.IsDefined(typeof(JsonConvertAttribute), false))) {
                return true;
            } else {
                Type t = null;
                if (member is Type)
                    t = member as Type;
                else
                    return false;

                // don't use converters for primitives and strings
                if (t.IsPrimitive || t == typeof(string))
                    return false;

                TypeConverter converter = TypeDescriptor.GetConverter(t);
                return converter.CanConvertFrom(typeof(string)) && converter.CanConvertTo(typeof(string));
            }
        }

        public IJsonTypeConverter GetConverter(MemberInfo forMember)
        {
            if (!(forMember is Type || forMember is PropertyInfo))
            {
                throw new ArgumentException("forMember parameter to GetConverter must be either a Type, or PropertyInfo");
            }

            if (_registeredTypes.ContainsKey(forMember))
            {
                return _registeredTypes[forMember];
            }
            else if (forMember.IsDefined(typeof(JsonConvertAttribute), true))
            {
                // just one for now, but later support chaining of converters
                JsonConvertAttribute convAttr = (JsonConvertAttribute) forMember.GetCustomAttributes(typeof(JsonConvertAttribute), false)[0];
                IJsonTypeConverter converter = (IJsonTypeConverter) Activator.CreateInstance(convAttr.Converter);
                converter.SourceType = forMember is Type ? (Type) forMember : ((PropertyInfo) forMember).PropertyType;
                _registeredTypes[forMember] = converter;
                // should we register it?
                return converter;
            }
            else
            {
                // System.ComponentModel.TypeConverter
                IJsonTypeConverter converter = new TypeConverterAdapter(TypeDescriptor.GetConverter((Type) forMember));
                _registeredTypes[forMember] = converter;
                return converter;
            }
        }
    }
}
