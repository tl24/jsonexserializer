using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;
using JsonExSerializerTests.Mocks;
using MbUnit.Framework;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class IgnorePropertyActionTests
    {
        Serializer serializer;

        [SetUp]
        public void TestSetup()
        {
            serializer = new Serializer(typeof(SpecializedMock));
        }

        [Test]
        public void IgnoredPropertyAction_WhenIgnored_PropertyNotSet()
        {
            serializer.Context.IgnoredPropertyAction = SerializationContext.IgnoredPropertyOption.Ignore;
            string result = @" { IgnoredProp: 'NotIgnored' }";
            SpecializedMock mock = (SpecializedMock) serializer.Deserialize(result);
            Assert.AreNotEqual("NotIgnored", mock.IgnoredProp, "IgnoredProp not ignored");
        }

        [Test]
        public void IgnoredPropertyAction_WhenSetIfPossible_WriteablePropertyIsSet()
        {
            serializer.Context.IgnoredPropertyAction = SerializationContext.IgnoredPropertyOption.SetIfPossible;
            string result = @" { IgnoredProp: 'NotIgnored' }";
            SpecializedMock mock = (SpecializedMock)serializer.Deserialize(result);
            Assert.AreEqual("NotIgnored", mock.IgnoredProp, "IgnoredProp not set");
        }

        [Test]
        public void IgnoredPropertyAction_WhenSetIfPossible_ReadonlyPropertyNotSet()
        {
            serializer.Context.IgnoredPropertyAction = SerializationContext.IgnoredPropertyOption.SetIfPossible;
            string result = @" { Count: 22 }";
            SpecializedMock mock = (SpecializedMock)serializer.Deserialize(result);
            Assert.AreNotEqual(22, mock.Count, "Readonly property set");
        }

        [Test]
        public void IgnoredPropertyAction_WhenThrowException_IgnoredPropertyThrowsException()
        {
            serializer.Context.IgnoredPropertyAction = SerializationContext.IgnoredPropertyOption.ThrowException;
            string result = @" { IgnoredProp: 'NotIgnored' }";
            bool exception = false;
            try
            {
                SpecializedMock mock = (SpecializedMock)serializer.Deserialize(result);
            }
            catch (Exception e)
            {
                exception = true;
            }

            Assert.IsTrue(exception, "Exception not thrown for ignored property when ThrowException set");
        }
    }


}
