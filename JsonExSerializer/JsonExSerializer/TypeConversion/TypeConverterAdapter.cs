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
        private SerializationContext _serializationContext;

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

        #region IJsonTypeConverter Members

        public Type GetSerializedType(Type sourceType)
        {
            return typeof(string);
        }

        #endregion

        public SerializationContext SerializationContext
        {
            get { return this._serializationContext; }
            set { this._serializationContext = value; }
        }
    }
}
