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
    public class TypeBindingConfigTests
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
            /*
            Serializer s = Serializer.GetSerializer(typeof(object), "TestRegisterTypeConverter");
            IJsonTypeConverter typeConverter = s.Context.GetConverter(typeof(SimpleObject));
            IJsonTypeConverter propConverter = s.Context.GetConverter(typeof(SimpleObject).GetProperty("BoolValue"));
            Assert.IsNotNull(typeConverter, "No converter for simple object registered");
            Assert.IsNotNull(propConverter, "No converter for simple object, BoolValue property registered");
             */
        }
    }
}
