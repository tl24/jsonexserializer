using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;
using JsonExSerializer;

namespace JsonExSerializerTests.Mocks
{
    public class StringHolder
    {
        private string _stringProp;

        public StringHolder(string prop)
        {
            _stringProp = prop;
        }

        public string StringProp
        {
            get { return this._stringProp; }
            set { this._stringProp = value; }
        }

    }

    public class BoolHolder
    {
        private bool _boolProp;

        public BoolHolder(bool prop)
        {
            _boolProp = prop;
        }

        public bool BoolProp
        {
            get { return this._boolProp; }
            set { this._boolProp = value; }
        }

    }

    public class ChainedConversionMock
    {
        private StringHolder _stringProp;

        public StringHolder StringProp
        {
            get { return this._stringProp; }
            set { this._stringProp = value; }
        }

    }

    public class StringToBoolConverter : IJsonTypeConverter
    {

        #region IJsonTypeConverter Members

        public Type GetSerializedType(Type sourceType)
        {
            return typeof(BoolHolder);
        }

        public object ConvertFrom(object item, JsonExSerializer.SerializationContext serializationContext)
        {
            return new BoolHolder(bool.Parse(((StringHolder) item).StringProp));
        }

        public object ConvertTo(object item, Type sourceType, JsonExSerializer.SerializationContext serializationContext)
        {
            return new StringHolder(((BoolHolder)item).BoolProp.ToString());
        }

        public object Context
        {
            set { return; }
        }

        #endregion
    }

    public class BoolToIntConverter : IJsonTypeConverter
    {
        #region IJsonTypeConverter Members

        public Type GetSerializedType(Type sourceType)
        {
            return typeof(int);
        }

        public object ConvertFrom(object item, JsonExSerializer.SerializationContext serializationContext)
        {
            return ((BoolHolder)item).BoolProp ? -1 : 0;
        }

        public object ConvertTo(object item, Type sourceType, JsonExSerializer.SerializationContext serializationContext)
        {
            return new BoolHolder(((int)item) == 0 ? false : true);
        }

        public object Context
        {
            set { return; }
        }

        #endregion
    }

}
