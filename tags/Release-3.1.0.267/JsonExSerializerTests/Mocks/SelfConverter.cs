using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializerTests.Mocks
{
    /// <summary>
    /// An object that implements its own type converter
    /// </summary>
    public class SelfConverter : JsonConverterBase, IJsonTypeConverter
    {
        private int num;
        public SelfConverter()
        {
            num = ((new Random()).Next(1, 1000000));
        }

        public override bool Equals(object obj)
        {
            if (obj is SelfConverter)
            {
                SelfConverter other = (SelfConverter)obj;
                return this.num == other.num;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return num.GetHashCode();
        }

        public override Type GetSerializedType(Type sourceType)
        {
            return typeof(int);
        }

        public override object ConvertFrom(object item, JsonExSerializer.SerializationContext serializationContext)
        {
            return num;
        }

        public override object ConvertTo(object item, Type sourceType, JsonExSerializer.SerializationContext serializationContext)
        {
            this.num = (int)item;
            return this;
        }
    }
}
