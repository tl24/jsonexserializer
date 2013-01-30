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
            Serializer serializer = new Serializer();
            ISerializerSettings config = serializer.Settings;
            IPropertyData property = config.Types[typeof(SimpleObject)].FindProperty(propertyName);
            SimpleObject testObject = new SimpleObject();
            property.DefaultValueSetting = DefaultValueOption.SuppressDefaultValues;
            Assert.IsFalse(property.ShouldWriteValue(config, property.GetValue(testObject)));
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
            Serializer serializer = new Serializer();
            ISerializerSettings config = serializer.Settings;
            IPropertyData property = config.Types[typeof(SimpleObject)].FindProperty(propertyName);
            SimpleObject testObject = new SimpleObject();
            property.DefaultValueSetting = DefaultValueOption.WriteAllValues;
            Assert.IsTrue(property.ShouldWriteValue(config, property.GetValue(testObject)));
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
            Serializer serializer = new Serializer();
            ISerializerSettings config = serializer.Settings;
            IPropertyData property = config.Types[typeof(SimpleObject)].FindProperty(propertyName);
            property.DefaultValueSetting = DefaultValueOption.SuppressDefaultValues;
            Assert.IsTrue(property.ShouldWriteValue(config, value));
        }

        [RowTest]
        [Row("IntValue", -1)]
        [Row("LongValue", -20L)]
        [Row("FloatValue", -1f)]
        [Row("DoubleValue", -100.0)]
        [Row("BoolValue", true)]
        [Row("StringValue", "none")]
        [Row("CharValue", ' ')]
        [Row("EnumValue", SimpleEnum.EnumValue3)]
        public void WhenSuppressDefaultValuesOnPropertyWithCustomDefault_ValueIsCustomDefault_PropertyIsNotWritten(string propertyName, object defaultValue)
        {
            Serializer serializer = new Serializer();
            ISerializerSettings config = serializer.Settings;
            IPropertyData property = config.Types[typeof(SimpleObject)].FindProperty(propertyName);
            property.DefaultValueSetting = DefaultValueOption.SuppressDefaultValues;
            property.DefaultValue = defaultValue;
            Assert.IsFalse(property.ShouldWriteValue(config, defaultValue));
        }

        [Test]
        public void TestDefaultPropertyAttributes()
        {
            ISerializerSettings config = new SerializerSettings();
            ITypeData typeData = config.Types.Type<MockDefaultValues>();
            IPropertyData intDefault = typeData.FindProperty("IntDefault");
            Assert.AreEqual(DefaultValueOption.SuppressDefaultValues, intDefault.DefaultValueSetting, "IntDefault DefaultValueSetting");
            Assert.IsFalse(intDefault.ShouldWriteValue(config, 0));

            IPropertyData intCustomDefault = typeData.FindProperty("IntCustomDefault");
            Assert.AreEqual(DefaultValueOption.SuppressDefaultValues, intCustomDefault.DefaultValueSetting, "IntCustomDefault DefaultValueSetting");
            Assert.IsFalse(intCustomDefault.ShouldWriteValue(config, 10));

            IPropertyData stringDefaultDisabled = typeData.FindProperty("StringDefaultDisabled");
            Assert.AreEqual(DefaultValueOption.WriteAllValues, stringDefaultDisabled.DefaultValueSetting, "StringDefaultDisabled DefaultValueSetting");
            Assert.IsTrue(stringDefaultDisabled.ShouldWriteValue(config, null));
        }

        [Test]
        public void TestDefaultSerialization()
        {
            Serializer serializer = new Serializer();
            MockDefaultValues mock = new MockDefaultValues();
            mock.StringDefaultDisabled = "test";
            serializer.Settings.IsCompact = true;
            string result = serializer.Serialize(mock);
            Assert.AreEqual(@"{""StringDefaultDisabled"":""test""}", result.Trim());
        }

        [Test]
        public void WhenSuppressDefaultValuesOnType_CascadesToProperty()
        {
            ISerializerSettings config = new SerializerSettings();
            ITypeData typeData = config.Types[typeof(SimpleObject)];
            typeData.DefaultValueSetting = DefaultValueOption.SuppressDefaultValues;
            IPropertyData property = config.Types[typeof(SimpleObject)].FindProperty("IntValue");
            Assert.IsFalse(property.ShouldWriteValue(config, 0));
        }

        [Test]
        public void WhenDefaultValuesSetOnType_PropertyInheritsIt()
        {
            ISerializerSettings config = new SerializerSettings();
            ITypeData typeData = config.Types[typeof(SimpleObject)];
            typeData.DefaultValueSetting = DefaultValueOption.SuppressDefaultValues;
            typeData.DefaultValues[typeof(string)] = "FromType";
            IPropertyData property = config.Types[typeof(SimpleObject)].FindProperty("StringValue");
            Assert.IsFalse(property.ShouldWriteValue(config, "FromType"));
            Assert.IsTrue(property.ShouldWriteValue(config, ""));
        }

        [Test]
        public void WhenDefaultValuesSetOnContext_PropertyInheritsFromContextIfNotSetOnType()
        {
            ISerializerSettings config = new SerializerSettings();
            ITypeData typeData = config.Types[typeof(SimpleObject)];
            typeData.DefaultValueSetting = DefaultValueOption.SuppressDefaultValues;
            config.DefaultValues[typeof(string)] = "FromType";
            typeData.DefaultValues[typeof(int)] = 22;

            IPropertyData stringProperty = config.Types[typeof(SimpleObject)].FindProperty("StringValue");
            Assert.IsFalse(stringProperty.ShouldWriteValue(config, "FromType"));
            Assert.IsTrue(stringProperty.ShouldWriteValue(config, ""));

            IPropertyData intProperty = config.Types[typeof(SimpleObject)].FindProperty("IntValue");
            Assert.IsFalse(intProperty.ShouldWriteValue(config, 22));
            Assert.IsTrue(intProperty.ShouldWriteValue(config, 0));

            IPropertyData shortProperty = config.Types[typeof(SimpleObject)].FindProperty("ShortValue");
            shortProperty.DefaultValue = (short)9;
            Assert.IsFalse(shortProperty.ShouldWriteValue(config, (short)9));
            Assert.IsTrue(shortProperty.ShouldWriteValue(config, 0));
        }

        [Test]
        public void WhenDefaultValuesSetByAttributeOnType_PropertyInheritsIt()
        {
            ISerializerSettings config = new SerializerSettings();
            ITypeData typeData = config.Types[typeof(MockDefaultValuesCascade)];
            IPropertyData property = typeData.FindProperty("EmptyString");
            Assert.IsFalse(property.ShouldWriteValue(config, ""));
            Assert.IsTrue(property.ShouldWriteValue(config, null));
        }

        [Test]
        public void DefaultValuesOnTypeAreConvertedIfNotSameType()
        {
            ISerializerSettings config = new SerializerSettings();
            ITypeData typeData = config.Types[typeof(MockDefaultValuesCascade)];
            IPropertyData property = typeData.FindProperty("ConvertedDefault");
            Assert.IsFalse(property.ShouldWriteValue(config, (short)32));
            Assert.IsTrue(property.ShouldWriteValue(config, 0));
        }

        [Test]
        public void DefaultValuesOnPropertyAreConvertedIfNotSameType()
        {
            ISerializerSettings config = new SerializerSettings();
            ITypeData typeData = config.Types[typeof(MockDefaultValues)];
            IPropertyData property = typeData.FindProperty("ConvertedValue");
            Assert.IsFalse(property.ShouldWriteValue(config, (short)32));
            Assert.IsTrue(property.ShouldWriteValue(config, 0));
        }
    }
}
