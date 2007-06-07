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
            return _registeredTypes.ContainsKey(member)
            || (member.IsDefined(typeof(JsonConvertAttribute), false))
            || member.IsDefined(typeof(TypeConverterAttribute), false);
        }

        public IJsonTypeConverter GetConverter(MemberInfo forMember)
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
                converter.SourceType = forMember is Type ? (Type) forMember : forMember.DeclaringType;
                _registeredTypes[forMember] = converter;
                // should we register it?
                return converter;
            }
            else
            {
                // System.ComponentModel.TypeConverter
                IJsonTypeConverter converter = new TypeConverterAdapter(TypeDescriptor.GetConverter(forMember.DeclaringType));
                _registeredTypes[forMember] = converter;
                return converter;
            }
        }
    }
}
