using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using JsonExSerializer.TypeConversion;
using JsonExSerializerTests.Mocks;
using System.ComponentModel;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class ConversionTests
    {

        [Test]
        public void TestHasConverter()
        {
            TypeConverterFactory factory = new TypeConverterFactory();
            bool f = factory.HasConverter(typeof(Guid));

            Assert.IsTrue(f, "Guid should have implicit TypeConverter through System.ComponentModel framework");

            f = factory.HasConverter(typeof(SimpleObject));

            Assert.IsFalse(f, "SimpleObject should not have converter");

            
            f = factory.HasConverter(typeof(SimpleObject).GetProperty("ByteValue"));

            Assert.IsFalse(f, "SimpleObject.ByteValue property does not have a converter");

            // test an unexpected type
            f = factory.HasConverter(typeof(SimpleObject).GetConstructor(new Type[0]));
            Assert.IsFalse(f, "Can't test for converter from a constructor");

            // test primitives
            f = factory.HasConverter(typeof(int));
            Assert.IsFalse(f, "No converters for primitive types");

            f = factory.HasConverter(typeof(string));
            Assert.IsFalse(f, "No converters for string");
        }

        [Test]
        public void ConvertGuidTest()
        {
            TypeConverterFactory factory = new TypeConverterFactory();
            if (factory.HasConverter(typeof(Guid)))
            {
                IJsonTypeConverter converter = factory.GetConverter(typeof(Guid));
                Guid g = Guid.NewGuid();
                object o = converter.ConvertFrom(g);
                Assert.IsTrue(o is string, "Converter should convert guid to string");
                Guid result = (Guid) converter.ConvertTo(o);

                Assert.AreEqual(g, result, "Guid did not convert correctly");
            }
            else
            {
                Assert.Fail("Guid should have a typeconverter");
            }

        }
    }
}
