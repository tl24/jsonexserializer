using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
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
            DefaultConverterFactory factory = new DefaultConverterFactory();
            bool f = factory.HasConverter(typeof(Guid));

            Assert.IsTrue(f, "Guid should have implicit TypeConverter through System.ComponentModel framework");

            f = factory.HasConverter(typeof(SimpleObject));

            Assert.IsFalse(f, "SimpleObject should not have converter");

            
            f = factory.HasConverter(typeof(SimpleObject).GetProperty("ByteValue"));

            Assert.IsFalse(f, "SimpleObject.ByteValue property does not have a converter");

            // test primitives
            f = factory.HasConverter(typeof(int));
            Assert.IsFalse(f, "No converters for primitive types");

            f = factory.HasConverter(typeof(string));
            Assert.IsFalse(f, "No converters for string");
        }

        [Test]
        public void ConvertGuidTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(Guid));
            Guid g = Guid.NewGuid();
            string result = s.Serialize(g);
            Guid actual = (Guid)s.Deserialize(result);
            Assert.AreEqual(g, actual, "Guid test failed");
        }

        [Test]
        public void ClassAttributeTest()
        {
            Serializer serializer = Serializer.GetSerializer(typeof(MyImmutablePoint));
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

            Serializer s = Serializer.GetSerializer(expected.GetType());
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
            Serializer s = Serializer.GetSerializer(typeof(SelfConverter));
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

            Serializer s = Serializer.GetSerializer(typeof(MyLine));
            string result = s.Serialize(line);
            MyLine actual = (MyLine)s.Deserialize(result);
            Assert.AreEqual(line.Start, actual.Start, "Line start not equal");
            Assert.AreEqual(line.End, actual.End, "Line end not equal");
            // make sure the property converter overrode the converter declared on the type
            Assert.AreEqual(1, MyLinePointConverter.ConvertFromCount, "Property ConvertFrom not called correct amount of times");
            Assert.AreEqual(1, MyLinePointConverter.ConvertToCount, "Property ConvertTo not called correct amount of times");
        }

        /// <summary>
        /// Test property that has multiple convert attributes on it
        /// </summary>
        [Test]
        public void ChainedConversionTest()
        {
            ChainedConversionMock mock = new ChainedConversionMock();
            mock.StringProp = new StringHolder("True");
            Serializer s = Serializer.GetSerializer(typeof(ChainedConversionMock));
            string result = s.Serialize(mock);
            ChainedConversionMock actual = (ChainedConversionMock)s.Deserialize(result);
            Assert.AreEqual(mock.StringProp.StringProp, actual.StringProp.StringProp, "Chained conversion failed");
        }

        [Test]
        public void IgnorePropertyTest()
        {
            MyLine line = new MyLine();
            line.Start = new MyImmutablePoint(1, 5);
            line.End = new MyImmutablePoint(2, 12);

            Serializer s = Serializer.GetSerializer(typeof(MyLine));
            // ignore properties (Use both methods)
            s.Context.IgnoreProperty(typeof(MyLine), "Start");
            s.Context.IgnoreProperty(typeof(MyLine).GetProperty("End"));
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
