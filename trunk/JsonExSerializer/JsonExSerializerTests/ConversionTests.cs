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
    }
}
