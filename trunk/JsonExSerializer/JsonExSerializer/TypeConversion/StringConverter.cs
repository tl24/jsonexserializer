using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.TypeConversion
{
    /// <summary>
    /// Utility converter for converting classes to and from a string.  The
    /// class is expected to have a constructor that takes a string or object and the
    /// ToString method should provide the conversion to string.
    /// </summary>
    public class StringConverter : JsonConverterBase
    {
        public override Type GetSerializedType(Type sourceType)
        {
            return typeof(string);
        }

        public override object ConvertFrom(object item, ISerializerSettings serializationContext)
        {
            return item.ToString();
        }

        public override object ConvertTo(object item, Type sourceType, ISerializerSettings serializationContext)
        {
            return Activator.CreateInstance(sourceType, item);
        }
    }
}
