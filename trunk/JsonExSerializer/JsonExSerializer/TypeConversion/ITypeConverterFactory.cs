/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Reflection;
namespace JsonExSerializer.TypeConversion
{
    /// <summary>
    /// A factory for producing type converters.  One of the HasConverter
    /// methods should be called to see if this factory instance can produce
    /// a converter of the given type.  If so, then a call to GetConverter
    /// should succeed.
    /// </summary>
    public interface ITypeConverterFactory
    {
        IJsonTypeConverter GetConverter(PropertyInfo forProperty);
        IJsonTypeConverter GetConverter(Type forType);
        bool HasConverter(Type forType);
        bool HasConverter(PropertyInfo forProperty);
        SerializationContext SerializationContext { set; }
    }
}
