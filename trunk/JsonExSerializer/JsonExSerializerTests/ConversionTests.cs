using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using JsonExSerializer.TypeConversion;
using JsonExSerializerTests.Mocks;

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

            f = factory.HasConverter(typeof(SimpleObject));
        }
    }
}
