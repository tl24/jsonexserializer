using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer.MetaData;
using JsonExSerializer;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class CustomPropertyTest
    {
        [Test]
        public void WhenCustomMetaDataIsReturn_ItsSerializedCorrectly()
        {
            Serializer s = new Serializer(typeof(CustomClass));
            s.Context.TypeHandlerFactory = new CustomTypeDataRepository(typeof(CustomClassTypeHandler), s.Context);
            CustomClass cust = new CustomClass();
            cust.SetID(23);
            cust.SetName("Frank");
            cust.Value = 999;
            string result = s.Serialize(cust);
            CustomClass dest = (CustomClass)s.Deserialize(result);
            Assert.AreEqual(23, dest.GetID());
            Assert.AreEqual("Frank", dest.GetName());
            Assert.AreEqual(999, dest.Value);
        }
    }

    public class CustomClass
    {
        private string _name;
        private int _id;
        private int _value;

        public string GetName()
        {
            return _name;
        }

        public void SetName(string newName)
        {
            _name = newName;
        }

        public int GetID()
        {
            return _id;
        }

        public void SetID(int id)
        {
            _id = id;
        }

        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }

    public class CustomClassTypeHandler : TypeData
    {
        public CustomClassTypeHandler(Type t, SerializationContext context)
            : base(t, context)
        {
        }

        protected override IList<IPropertyData> ReadProperties()
        {
            IList<IPropertyData> properties = base.ReadProperties();
            properties.Add(new MethodPairPropertyHandler(this.ForType, "Name"));
            properties.Add(new MethodPairPropertyHandler(this.ForType, "ID"));
            return properties;
        }

    }

    public class MethodPairPropertyHandler : AbstractPropertyData
    {
        private string _getMethod;
        private string _setMethod;
        private string _propertyName;

        public MethodPairPropertyHandler(Type DeclaringType, string Name)
            : this(DeclaringType, "Get" + Name, "Set" + Name, Name)
        {
        }

        public MethodPairPropertyHandler(Type DeclaringType, string GetMethod, string SetMethod, string PropertyName)
            : base(DeclaringType)
        {
            _getMethod = GetMethod;
            _setMethod = SetMethod;
            _propertyName = PropertyName;
        }

        public override string Name
        {
            get { return _propertyName; }
        }

        public override Type PropertyType
        {
            get {
                return this.ForType.GetMethod(_getMethod).ReturnType;
            }
        }

        public override object GetValue(object instance)
        {
            return this.ForType.GetMethod(_getMethod).Invoke(instance, null);
        }

        public override void SetValue(object instance, object value)
        {
            this.ForType.GetMethod(_setMethod).Invoke(instance, new object[] { value });
        }

        protected override JsonExSerializer.TypeConversion.IJsonTypeConverter CreateTypeConverter()
        {
            return CreateTypeConverter(PropertyType);
        }

        public override bool Ignored
        {
            get
            {
                return false;
            }
            set
            {
                
            }
        }

    }
}
