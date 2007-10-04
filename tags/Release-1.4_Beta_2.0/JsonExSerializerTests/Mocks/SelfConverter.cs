using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializerTests.Mocks
{
    /// <summary>
    /// An object that implements its own type converter
    /// </summary>
    public class SelfConverter : IJsonTypeConverter
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

        #region IJsonTypeConverter Members

        public Type GetSerializedType(Type sourceType)
        {
            return typeof(int);
        }

        public object ConvertFrom(object item, JsonExSerializer.SerializationContext serializationContext)
        {
            return num;
        }

        public object ConvertTo(object item, Type sourceType, JsonExSerializer.SerializationContext serializationContext)
        {
            this.num = (int)item;
            return this;
        }

        public object Context
        {
            set { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }
}
