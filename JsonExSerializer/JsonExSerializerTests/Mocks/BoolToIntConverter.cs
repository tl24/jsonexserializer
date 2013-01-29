using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;
using JsonExSerializer;

namespace JsonExSerializerTests.Mocks
{
    public class BoolToIntConverter : JsonConverterBase, IJsonTypeConverter
    {

        public override Type GetSerializedType(Type sourceType)
        {
            return typeof(int);
        }

        public override object ConvertFrom(object item, ISerializerSettings serializationContext)
        {
            return ((bool)item) ? -1 : 0;
        }

        public override object ConvertTo(object item, Type sourceType, ISerializerSettings serializationContext)
        {
            return ((int)item) == 0 ? false : true;
        }
    }
}
