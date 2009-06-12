using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer;
using JsonExSerializerTests.Mocks;
using JsonExSerializer.MetaData;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class DefaultValuesTests
    {
        [RowTest]
        [Row("IntValue")]
        [Row("ByteValue")]
        [Row("ShortValue")]
        [Row("LongValue")]
        [Row("FloatValue")]
        [Row("DoubleValue")]
        [Row("BoolValue")]
        [Row("StringValue")]
        [Row("CharValue")]
        [Row("EnumValue")]
        public void WhenSuppressDefaultValuesOnProperty_ValueIsDefault_PropertyNotWritten(string propertyName)
        {
            Serializer serializer = new Serializer(typeof(SimpleObject));
            SerializationContext context = serializer.Context;
            IPropertyData property = context.TypeHandlerFactory[typeof(SimpleObject)].FindProperty(propertyName);
            SimpleObject testObject = new SimpleObject();
            property.DefaultValueSetting = DefaultValueOption.SuppressDefaultValues;
            Assert.IsFalse(property.ShouldWriteValue(context, property.GetValue(testObject)));
        }

        [RowTest]
        [Row("IntValue")]
        [Row("ByteValue")]
        [Row("ShortValue")]
        [Row("LongValue")]
        [Row("FloatValue")]
        [Row("DoubleValue")]
        [Row("BoolValue")]
        [Row("StringValue")]
        [Row("CharValue")]
        [Row("EnumValue")]
        public void WhenWriteAllValuesOnProperty_ValueIsDefault_PropertyWritten(string propertyName)
        {
            Serializer serializer = new Serializer(typeof(SimpleObject));
            SerializationContext context = serializer.Context;
            IPropertyData property = context.TypeHandlerFactory[typeof(SimpleObject)].FindProperty(propertyName);
            SimpleObject testObject = new SimpleObject();
            property.DefaultValueSetting = DefaultValueOption.WriteAllValues;
            Assert.IsTrue(property.ShouldWriteValue(context, property.GetValue(testObject)));
        }

        [RowTest]
        [Row("IntValue", 10)]
        [Row("LongValue", 1400000L)]
        [Row("FloatValue", 23.42f)]
        [Row("DoubleValue", 99.99)]
        [Row("BoolValue", true)]
        [Row("StringValue","")]
        [Row("CharValue",'a')]
        [Row("EnumValue", SimpleEnum.EnumValue2)]
        public void WhenSuppressDefaultValuesOnProperty_ValueIsNotDefault_PropertyIsWritten(string propertyName, object value)
        {
            Serializer serializer = new Serializer(typeof(SimpleObject));
            SerializationContext context = serializer.Context;
            IPropertyData property = context.TypeHandlerFactory[typeof(SimpleObject)].FindProperty(propertyName);
            property.DefaultValueSetting = DefaultValueOption.SuppressDefaultValues;
            Assert.IsTrue(property.ShouldWriteValue(context, value));
        }

        [RowTest]
        [Row("IntValue", -1)]
        [Row("LongValue", -20L)]
        [Row("FloatValue", -1f)]
        [Row("DoubleValue", -100)]
        [Row("BoolValue", true)]
        [Row("StringValue", "none")]
        [Row("CharValue", ' ')]
        [Row("EnumValue", SimpleEnum.EnumValue3)]
        public void WhenSuppressDefaultValuesOnPropertyWithCustomDefault_ValueIsCustomDefault_PropertyIsNotWritten(string propertyName, object defaultValue)
        {
            Serializer serializer = new Serializer(typeof(SimpleObject));
            SerializationContext context = serializer.Context;
            IPropertyData property = context.TypeHandlerFactory[typeof(SimpleObject)].FindProperty(propertyName);
            property.DefaultValueSetting = DefaultValueOption.SuppressDefaultValues;
            property.DefaultValue = defaultValue;
            Assert.IsFalse(property.ShouldWriteValue(context, defaultValue));
        }

        [Test]
        public void TestDefaultPropertyAttributes()
        {
            SerializationContext context = new SerializationContext();
            TypeData typeData = context.TypeHandlerFactory[typeof(MockDefaultValues)];
            IPropertyData intDefault = typeData.FindProperty("IntDefault");
            Assert.AreEqual(DefaultValueOption.SuppressDefaultValues, intDefault.DefaultValueSetting, "IntDefault DefaultValueSetting");
            Assert.IsFalse(intDefault.ShouldWriteValue(context, 0));

            IPropertyData intCustomDefault = typeData.FindProperty("IntCustomDefault");
            Assert.AreEqual(DefaultValueOption.SuppressDefaultValues, intCustomDefault.DefaultValueSetting, "IntCustomDefault DefaultValueSetting");
            Assert.IsFalse(intCustomDefault.ShouldWriteValue(context, 10));

            IPropertyData stringDefaultDisabled = typeData.FindProperty("StringDefaultDisabled");
            Assert.AreEqual(DefaultValueOption.WriteAllValues, stringDefaultDisabled.DefaultValueSetting, "StringDefaultDisabled DefaultValueSetting");
            Assert.IsTrue(stringDefaultDisabled.ShouldWriteValue(context, null));
        }

        [Test]
        public void TestDefaultSerialization()
        {
            Serializer serializer = new Serializer(typeof(MockDefaultValues));
            MockDefaultValues mock = new MockDefaultValues();
            mock.StringDefaultDisabled = "test";
            serializer.Context.IsCompact = true;
            serializer.Context.OutputTypeComment = false;            
            string result = serializer.Serialize(mock);
            Assert.AreEqual(@"{""StringDefaultDisabled"":""test""}", result.Trim());
        }
    }
}
