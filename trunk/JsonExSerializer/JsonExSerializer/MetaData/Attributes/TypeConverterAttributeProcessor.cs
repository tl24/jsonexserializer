using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer.MetaData.Attributes
{
    public class TypeConverterAttributeProcessor : AttributeProcessor
    {
        public override void Process(MetaDataBase metaData, ICustomAttributeProvider attributeProvider, IConfiguration config)
        {
            IJsonTypeConverter converter = CreateTypeConverter(attributeProvider);
            if (converter != null)
                metaData.TypeConverter = converter;
        }

        protected IJsonTypeConverter CreateTypeConverter(ICustomAttributeProvider provider)
        {
            if (provider.IsDefined(typeof(JsonConvertAttribute), false))
            {
                JsonConvertAttribute convAttr = (JsonConvertAttribute)provider.GetCustomAttributes(typeof(JsonConvertAttribute), false)[0];
                return CreateTypeConverter(convAttr);
            }
            return null;
        }

        /// <summary>
        /// Constructs a converter from the convert attribute
        /// </summary>
        /// <param name="attribute">the JsonConvertAttribute decorating a property or class</param>
        /// <returns>converter</returns>
        private static IJsonTypeConverter CreateTypeConverter(JsonConvertAttribute attribute)
        {
            IJsonTypeConverter converter = (IJsonTypeConverter)Activator.CreateInstance(attribute.Converter);
            if (attribute.Context != null)
            {
                converter.Context = attribute.Context;
            }
            return converter;
        }

    }
}
