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
            Serializer s = new Serializer("TypeBindingAdd");
            Assert.AreEqual("SimpleObject", s.Settings.TypeAliases[typeof(SimpleObject)]);
            // check that defaults are still mapped
            Assert.AreEqual("int", s.Settings.TypeAliases[typeof(int)]);
        }

        [Test]
        public void TestRemoveBinding()
        {
            Serializer s = new Serializer("TypeBindingRemove");
            // verify int is not mapped
            // <remove type="System.Int32, mscorlib" />
            Assert.IsNull(s.Settings.TypeAliases[typeof(int)]);
            // verify float is not mapped
            // <remove alias="float" />
            Assert.IsNull(s.Settings.TypeAliases[typeof(float)]);
        }

        [Test]
        public void TestClearAddBinding()
        {
            Serializer s = new Serializer("TypeBindingClearAdd");
            Assert.AreEqual("SimpleObject", s.Settings.TypeAliases[typeof(SimpleObject)]);
            // check that defaults are not mapped
            Assert.IsNull(s.Settings.TypeAliases[typeof(int)]);
        }

        [Test]
        public void TestRegisterTypeConverter()
        {
            Serializer s = new Serializer("TestRegisterTypeConverter");
            IJsonTypeConverter typeConverter = s.Settings.Type<SimpleObject>().TypeConverter;
            IJsonTypeConverter propConverter = s.Settings.Type<SimpleObject>().FindProperty("BoolValue").TypeConverter;
            Assert.IsNotNull(typeConverter, "No converter for simple object registered");
            Assert.IsNotNull(propConverter, "No converter for simple object, BoolValue property registered");
        }

        [Test]
        public void TestCollectionHandler()
        {
            Serializer s = new Serializer("TestCollectionHandlers");
            MockCollection coll = new MockCollection("test");
            string result = s.Serialize(coll);
            MockCollection actual = s.Deserialize<MockCollection>(result);
            Assert.AreEqual(coll.Value(), actual.Value(), "MockCollectionHandler not configured correctly");
        }

        [Test]
        public void IgnorePropertyTest()
        {
            MyLine line = new MyLine();
            line.Start = new MyImmutablePoint(1, 5);
            line.End = new MyImmutablePoint(2, 12);

            Serializer s = new Serializer("TestIgnoreProperties");
            string result = s.Serialize(line);
            MyLine actual = s.Deserialize<MyLine>(result);
            Assert.IsNull(actual.Start, "Line start should be ignored");
            Assert.IsNull(actual.End, "Line end should be ignored");
            // converters should not be called on ignored properties
            Assert.AreEqual(0, MyLinePointConverter.ConvertFromCount, "Property ConvertFrom not called correct amount of times");
            Assert.AreEqual(0, MyLinePointConverter.ConvertToCount, "Property ConvertTo not called correct amount of times");
        }

        [Test]
        public void TestMultipleSections()
        {
            //this test to fix issue 55
            Serializer s = new Serializer("TestMultipleSections");
            Assert.IsTrue(s.Settings.Type<SimpleObject>().HasConverter, "SimpleObject converter");
            Assert.IsTrue(s.Settings.Type<MyLine>().FindPropertyByName("Start").Ignored, "MyLine.Start Ignored");
        }
        [TearDown]
        public void Teardown()
        {
            MyLinePointConverter.clear();
        }
    }
}
