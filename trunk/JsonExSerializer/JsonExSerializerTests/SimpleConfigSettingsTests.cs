using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class SimpleConfigSettingsTests
    {
        private Serializer serializer;
        private SerializationContext context;
        [SetUp]
        public void Setup()
        {
            serializer = new Serializer(typeof(object), "SimpleSettingsConfig");
            context = serializer.Context;
        }

        [Test]
        public void TestIsCompactValue()
        {
            Assert.IsTrue(context.IsCompact);
        }

        [Test]
        public void TestOutputTypeCommentValue()
        {
            Assert.IsFalse(context.OutputTypeComment);
        }

        [Test]
        public void TestOutputTypeInformationValue()
        {
            Assert.IsFalse(context.OutputTypeInformation);
        }

        [Test]
        public void TestReferenceWritingTypeValue()
        {
            Assert.AreEqual(SerializationContext.ReferenceOption.WriteIdentifier, context.ReferenceWritingType);
        }
    }
}
