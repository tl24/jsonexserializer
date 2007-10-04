using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer;
using JsonExSerializerTests.Mocks;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class AdvancedConfigSettingTests
    {
        [Test]
        public void TestAddBinding()
        {
            Serializer s = Serializer.GetSerializer(typeof(object), "TypeBindingAdd");
            Assert.AreEqual("SimpleObject", s.Context.GetTypeAlias(typeof(SimpleObject)));
            // check that defaults are still mapped
            Assert.AreEqual("int", s.Context.GetTypeAlias(typeof(int)));
        }

        [Test]
        public void TestRemoveBinding()
        {
            Serializer s = Serializer.GetSerializer(typeof(object), "TypeBindingRemove");
            // verify int is not mapped
            // <remove type="System.Int32, mscorlib" />
            Assert.IsNull(s.Context.GetTypeAlias(typeof(int)));
            // verify float is not mapped
            // <remove alias="float" />
            Assert.IsNull(s.Context.GetTypeAlias(typeof(float)));
        }

        [Test]
        public void TestClearAddBinding()
        {
            Serializer s = Serializer.GetSerializer(typeof(object), "TypeBindingClearAdd");
            Assert.AreEqual("SimpleObject", s.Context.GetTypeAlias(typeof(SimpleObject)));
            // check that defaults are not mapped
            Assert.IsNull(s.Context.GetTypeAlias(typeof(int)));
        }

        [Test]
        public void TestRegisterTypeConverter()
        {
            Serializer s = Serializer.GetSerializer(typeof(object), "TestRegisterTypeConverter");
            IJsonTypeConverter typeConverter = s.Context.GetConverter(typeof(SimpleObject));
            IJsonTypeConverter propConverter = s.Context.GetConverter(typeof(SimpleObject).GetProperty("BoolValue"));
            Assert.IsNotNull(typeConverter, "No converter for simple object registered");
            Assert.IsNotNull(propConverter, "No converter for simple object, BoolValue property registered");
        }

        [Test]
        public void TestRegisterTypeConverterFactory()
        {
            Serializer s = Serializer.GetSerializer(typeof(KeyOnlyObjectImpl), "TestTypeConverterFactories");
            KeyOnlyObjectImpl ko = new KeyOnlyObjectImpl();
            ko.ID = "TestID";
            ko.Name = "TestName";

            string result = s.Serialize(ko);

            // assert string is ID only
            Assert.AreEqual("\"" + ko.ID + "\"", result, "Factory converter should serialize to ID only");

            KeyOnlyObjectImpl des = (KeyOnlyObjectImpl)s.Deserialize(result);
            Assert.AreSame(ko, des);             
        }

        [Test]
        public void TestCollectionHandler()
        {
            Serializer s = Serializer.GetSerializer(typeof(MockCollection), "TestCollectionHandlers");
            MockCollection coll = new MockCollection("test");
            string result = s.Serialize(coll);
            MockCollection actual = (MockCollection) s.Deserialize(result);
            Assert.AreEqual(coll.Value(), actual.Value(), "MockCollectionHandler not configured correctly");
        }

        [Test]
        public void IgnorePropertyTest()
        {
            MyLine line = new MyLine();
            line.Start = new MyImmutablePoint(1, 5);
            line.End = new MyImmutablePoint(2, 12);

            Serializer s = Serializer.GetSerializer(typeof(MyLine), "TestIgnoreProperties");
            string result = s.Serialize(line);
            MyLine actual = (MyLine)s.Deserialize(result);
            Assert.IsNull(actual.Start, "Line start should be ignored");
            Assert.IsNull(actual.End, "Line end should be ignored");
            // converters should not be called on ignored properties
            Assert.AreEqual(0, MyLinePointConverter.ConvertFromCount, "Property ConvertFrom not called correct amount of times");
            Assert.AreEqual(0, MyLinePointConverter.ConvertToCount, "Property ConvertTo not called correct amount of times");
        }
    }
}
