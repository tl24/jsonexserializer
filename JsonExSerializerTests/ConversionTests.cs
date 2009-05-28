using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer.TypeConversion;
using JsonExSerializerTests.Mocks;
using System.ComponentModel;
using JsonExSerializer;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class ConversionTests
    {

        [TearDown]
        public void teardown()
        {
            MyLinePointConverter.clear();
        }

        [Test]
        public void TestHasConverter()
        {
            SerializationContext ctx = new SerializationContext();
            bool f = ctx.TypeHandlerFactory[typeof(Guid)].HasConverter; 

            Assert.IsTrue(f, "Guid should have implicit TypeConverter through System.ComponentModel framework");

            f = ctx.TypeHandlerFactory[typeof(SimpleObject)].HasConverter;

            Assert.IsFalse(f, "SimpleObject should not have converter");


            f = ctx.TypeHandlerFactory[typeof(SimpleObject)].FindProperty("ByteValue").HasConverter;

            Assert.IsFalse(f, "SimpleObject.ByteValue property does not have a converter");

            // test primitives
            f = ctx.TypeHandlerFactory[typeof(int)].HasConverter;
            Assert.IsFalse(f, "No converters for primitive types");

            f = ctx.TypeHandlerFactory[typeof(string)].HasConverter;
            Assert.IsFalse(f, "No converters for string");
        }

        [Test]
        public void ConvertGuidTest()
        {
            Serializer s = new Serializer(typeof(Guid));
            Guid g = Guid.NewGuid();
            string result = s.Serialize(g);
            Guid actual = (Guid)s.Deserialize(result);
            Assert.AreEqual(g, actual, "Guid test failed");
        }

        [Test]
        public void ClassAttributeTest()
        {
            Serializer serializer = new Serializer(typeof(MyImmutablePoint));
            MyImmutablePoint expectedPt = new MyImmutablePoint(12, -10);
            string result = serializer.Serialize(expectedPt);
            MyImmutablePoint actualPt = (MyImmutablePoint) serializer.Deserialize(result);
            Assert.AreEqual(expectedPt, actualPt, "MyImmutablePoint class not serialized correctly");
        }

        [Test]
        public void CallbackTest()
        {
            MockCallbackObject expected = new MockCallbackObject();
            expected.Name = "callback";

            Serializer s = new Serializer(expected.GetType());
            string result = s.Serialize(expected);
            Assert.AreEqual(1, expected.BeforeSerializeCount, "BeforeSerialize incorrect count");
            Assert.AreEqual(1, expected.AfterSerializeCount, "AfterSerialize incorrect count");

            MockCallbackObject actual = (MockCallbackObject)s.Deserialize(result);
            Assert.AreEqual(1, actual.AfterDeserializeCount, "AfterDeserialize incorrect count");
        }

        [Test]
        public void SelfConversionTest()
        {
            SelfConverter expected = new SelfConverter();
            Serializer s = new Serializer(typeof(SelfConverter));
            string result = s.Serialize(expected);
            SelfConverter actual = (SelfConverter)s.Deserialize(result);
            Assert.AreEqual(expected, actual, "Selfconversion failed");
        }

        [Test]
        public void PropertyConversionTest()
        {
            MyLine line = new MyLine();
            line.Start = new MyImmutablePoint(1, 5);
            line.End = new MyImmutablePoint(2, 12);

            Serializer s = new Serializer(typeof(MyLine));
            string result = s.Serialize(line);
            MyLine actual = (MyLine)s.Deserialize(result);
            Assert.AreEqual(line.Start, actual.Start, "Line start not equal");
            Assert.AreEqual(line.End, actual.End, "Line end not equal");
            // make sure the property converter overrode the converter declared on the type
            Assert.AreEqual(1, MyLinePointConverter.ConvertFromCount, "Property ConvertFrom not called correct amount of times");
            Assert.AreEqual(1, MyLinePointConverter.ConvertToCount, "Property ConvertTo not called correct amount of times");
        }

        [Test]
        public void IgnorePropertyTest()
        {
            MyLine line = new MyLine();
            line.Start = new MyImmutablePoint(1, 5);
            line.End = new MyImmutablePoint(2, 12);

            Serializer s = new Serializer(typeof(MyLine));
            // ignore properties (Use both methods)
            s.Context.IgnoreProperty(typeof(MyLine), "Start");
            s.Context.IgnoreProperty(typeof(MyLine), "End");
            string result = s.Serialize(line);
            MyLine actual = (MyLine)s.Deserialize(result);
            Assert.IsNull(actual.Start, "Line start should be ignored");
            Assert.IsNull(actual.End, "Line end should be ignored");
            // converters should not be called on ignored properties
            Assert.AreEqual(0, MyLinePointConverter.ConvertFromCount, "Property ConvertFrom not called correct amount of times");
            Assert.AreEqual(0, MyLinePointConverter.ConvertToCount, "Property ConvertTo not called correct amount of times");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRegisterPrimitiveTypeConverter()
        {
            Serializer s = new Serializer(typeof(object));
            s.Context.RegisterTypeConverter(typeof(int), new MyImmutablePointConverter());
        }

        [Test]
        public void DictionaryToListTest()
        {
            Serializer s = new Serializer(typeof(Dictionary<string, SimpleObject>));
            DictionaryToListConverter converter = new DictionaryToListConverter();
            converter.Context = "StringValue";
            s.Context.RegisterTypeConverter(typeof(Dictionary<string, SimpleObject>), converter);
            Dictionary<string, SimpleObject> dictionary = new Dictionary<string, SimpleObject>();
            dictionary["One"] = new SimpleObject();
            dictionary["One"].StringValue = "One";
            dictionary["One"].IntValue = 1;

            dictionary["Two"] = new SimpleObject();
            dictionary["Two"].StringValue = "Two";
            dictionary["Two"].IntValue = 2;

            object list = converter.ConvertFrom(dictionary, s.Context);
            Assert.IsTrue(s.Context.TypeHandlerFactory[list.GetType()].IsCollection(), "Converted list is not a collection");

            Dictionary<string, SimpleObject> targetDictionary = (Dictionary<string, SimpleObject>) converter.ConvertTo(list, dictionary.GetType(), s.Context);
            Assert.AreEqual(2, targetDictionary.Count, "Wrong number of items");
            Assert.IsTrue(targetDictionary.ContainsKey("One"), "Key (One) not in converted dictionary");
            Assert.IsTrue(targetDictionary.ContainsKey("Two"), "Key (Two) not in converted dictionary");

            Assert.AreEqual(1, targetDictionary["One"].IntValue, "One Value wrong");
            Assert.AreEqual(2, targetDictionary["Two"].IntValue, "Two Value wrong");
        }

        [Test]
        public void TypeToStringTest_NotAliased()
        {
            Serializer s = new Serializer(typeof(Type));
            s.Context.IsCompact = true;
            s.Context.OutputTypeComment = false;
            s.Context.RegisterTypeConverter(typeof(Type), new TypeToStringConverter());
            string result = s.Serialize(typeof(SimpleObject));

            Assert.AreEqual("\"" + typeof(SimpleObject).AssemblyQualifiedName + "\"", result, "Type serialized improperly");

            Type typeResult = (Type) s.Deserialize(result);
            Assert.AreEqual(typeof(SimpleObject), typeResult, "Type deserialized improperly");
        }

        [Test]
        public void TypeToStringTest_Aliased()
        {
            Serializer s = new Serializer(typeof(Type));
            s.Context.IsCompact = true;
            s.Context.OutputTypeComment = false;
            s.Context.RegisterTypeConverter(typeof(Type), new TypeToStringConverter());
            string result = s.Serialize(typeof(int));

            Assert.AreEqual("\"int\"", result, "Type serialized improperly");

            Type typeResult = (Type)s.Deserialize(result);
            Assert.AreEqual(typeof(int), typeResult, "Type deserialized improperly");
        }
    }
}
