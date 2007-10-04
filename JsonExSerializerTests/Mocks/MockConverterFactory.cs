using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;
using System.Reflection;
using JsonExSerializer;

namespace JsonExSerializerTests.Mocks
{
    public class MockConverterFactory : ITypeConverterFactory
    {
        public Dictionary<string, IKeyOnlyObject> Stored;
        private SerializationContext context;
        private IJsonTypeConverter converter;

        public MockConverterFactory()
        {
            Stored = new Dictionary<string, IKeyOnlyObject>();
            converter = new KeyOnlyObjectConverter(Stored);
        }

        public IJsonTypeConverter GetConverter(PropertyInfo forProperty)
        {
            throw new Exception("No property converters registered.");
        }

        public IJsonTypeConverter GetConverter(Type forType)
        {
            return converter;
        }

        public bool HasConverter(Type forType)
        {
            // return converters for types implementing IKeyOnlyObject
            return (forType.GetInterface(typeof(IKeyOnlyObject).FullName) != null);
        }

        public bool HasConverter(PropertyInfo forProperty)
        {
            // convert the ID property
            return false;
        }

        public SerializationContext SerializationContext
        {
            get { return context; }
            set { 
                context = value;
                converter.Context = value;
            }
        }
    }

    public interface IKeyOnlyObject
    {
        string ID
        {
            get;
            set;
        }
    }

    public class KeyOnlyObjectImpl : IKeyOnlyObject
    {
        private string _id;
        private string _name;

        #region IKeyOnlyObject Members

        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        #endregion

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }
    }

    public class KeyOnlyObjectConverter : IJsonTypeConverter
    {

        private Dictionary<string, IKeyOnlyObject> Stored = null;

        public KeyOnlyObjectConverter(Dictionary<string, IKeyOnlyObject> Stored)
        {
            this.Stored = Stored;
        }

        #region IJsonTypeConverter Members

        public Type GetSerializedType(Type sourceType)
        {
            return typeof(string);
        }

        public object ConvertFrom(object item, SerializationContext serializationContext)
        {
            IKeyOnlyObject ko = (IKeyOnlyObject) item;
            Stored[ko.ID] = ko;
            return ko.ID;
        }

        public object ConvertTo(object item, Type sourceType, SerializationContext serializationContext)
        {
            string key = (string)item;
            return Stored[key];
        }

        public object Context
        {
            set { ; }
        }

        #endregion
    }
}
