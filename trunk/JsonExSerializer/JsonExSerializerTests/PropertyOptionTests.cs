using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;
using JsonExSerializerTests.Mocks;
using MbUnit.Framework;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class PropertyOptionTests
    {
        Serializer serializer;

        [SetUp]
        public void TestSetup()
        {
            serializer = new Serializer();
        }

        [Test]
        public void IgnoredPropertyAction_WhenIgnored_PropertyNotSet()
        {
            serializer.Settings.IgnoredPropertyAction = IgnoredPropertyOption.Ignore;
            string result = @" { IgnoredProp: 'NotIgnored' }";
            SpecializedMock mock = serializer.Deserialize<SpecializedMock>(result);
            Assert.AreNotEqual("NotIgnored", mock.IgnoredProp, "IgnoredProp not ignored");
        }

        [Test]
        public void IgnoredPropertyAction_WhenSetIfPossible_WriteablePropertyIsSet()
        {
            serializer.Settings.IgnoredPropertyAction = IgnoredPropertyOption.SetIfPossible;
            string result = @" { IgnoredProp: 'NotIgnored' }";
            SpecializedMock mock = serializer.Deserialize<SpecializedMock>(result);
            Assert.AreEqual("NotIgnored", mock.IgnoredProp, "IgnoredProp not set");
        }

        [Test]
        public void IgnoredPropertyAction_WhenSetIfPossible_ReadonlyPropertyNotSet()
        {
            serializer.Settings.IgnoredPropertyAction = IgnoredPropertyOption.SetIfPossible;
            string result = @" { Count: 22 }";
            SpecializedMock mock = serializer.Deserialize<SpecializedMock>(result);
            Assert.AreNotEqual(22, mock.Count, "Readonly property set");
        }

        [Test]
        public void IgnoredPropertyAction_WhenThrowException_IgnoredPropertyThrowsException()
        {
            serializer.Settings.IgnoredPropertyAction = IgnoredPropertyOption.ThrowException;
            string result = @" { IgnoredProp: 'NotIgnored' }";
            bool exception = false;
            try
            {
                SpecializedMock mock = serializer.Deserialize<SpecializedMock>(result);
            }
            catch
            {
                exception = true;
            }

            Assert.IsTrue(exception, "Exception not thrown for ignored property when ThrowException set");
        }

        [Test]
        public void MissingPropertyAction_WhenThrowException_MissingPropertyThrowsException()
        {
            serializer.Settings.MissingPropertyAction = MissingPropertyOptions.ThrowException;
            string result = @" { Foo: 'Bar' }";
            bool exception = false;
            try
            {
                SpecializedMock mock = serializer.Deserialize<SpecializedMock>(result);
            }
            catch
            {
                exception = true;
            }

            Assert.IsTrue(exception, "Exception not thrown for missing property when MissingPropertyOptions.ThrowException set");
        }

        [Test]
        public void MissingPropertyAction_WhenIgnore_MissingPropertyIsIgnored()
        {
            serializer.Settings.MissingPropertyAction = MissingPropertyOptions.Ignore;
            string result = @" { Foo: 'Bar', Name: 'Special' }";
            SpecializedMock mock = serializer.Deserialize<SpecializedMock>(result);
            Assert.AreEqual("Special", mock.Name);
        }
    }


}
