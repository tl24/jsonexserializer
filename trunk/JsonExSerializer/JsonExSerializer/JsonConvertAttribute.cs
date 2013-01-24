/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Struct,AllowMultiple=false,Inherited=false)]
    public class JsonConvertAttribute : System.Attribute
    {
        private Type _converter;

        public JsonConvertAttribute(Type converter)
        {
            _converter = converter;
        }

        public Type Converter
        {
            get { return this._converter; }
        }

        /// <summary>
        /// Creates the type converter represented by this attribute.
        /// </summary>
        /// <returns></returns>
        public virtual IJsonTypeConverter CreateTypeConverter()
        {
            IJsonTypeConverter converter = (IJsonTypeConverter)Activator.CreateInstance(Converter);
            return converter;
        }
    }
}
