using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace JsonExSerializer.TypeConversion
{
    public class TypeConverterAdapter : IJsonTypeConverter
    {
        private Type _sourceType;
        private TypeConverter _converter;

        public TypeConverterAdapter(TypeConverter converter) {
            _converter = converter;
        }

        public object ConvertFrom(object item)
        {
            return _converter.ConvertToString(item);
        }

        public object ConvertTo(object item, Type sourceType)
        {
            return _converter.ConvertFromString((string) item);
        }

        public object Context
        {
            set { return; }
        }
    }
}
