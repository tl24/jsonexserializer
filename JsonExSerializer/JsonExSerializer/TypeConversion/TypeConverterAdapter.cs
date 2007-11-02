/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace JsonExSerializer.TypeConversion
{

    /// <summary>
    /// TypeConverter that utilizes the System.ComponentModel.TypeConverter for
    /// a given type.
    /// </summary>
    public class TypeConverterAdapter : IJsonTypeConverter
    {
        private TypeConverter _converter;

        public TypeConverterAdapter(TypeConverter converter) {
            _converter = converter;
        }

        public object ConvertFrom(object item, SerializationContext serializationContext)
        {
            return _converter.ConvertToString(item);
        }

        public object ConvertTo(object item, Type sourceType, SerializationContext serializationContext)
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
    }
}
