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
        private ISerializerSettings context;
        [SetUp]
        public void Setup()
        {
            serializer = new Serializer("SimpleSettingsConfig");
            context = serializer.Settings;
        }

        [Test]
        public void TestIsCompactValue()
        {
            Assert.IsTrue(context.IsCompact);
        }

        [Test]
        public void TestOutputTypeInformationValue()
        {
            Assert.IsFalse(context.OutputTypeInformation);
        }

        [Test]
        public void TestReferenceWritingTypeValue()
        {
            Assert.AreEqual(ReferenceOption.WriteIdentifier, context.ReferenceWritingType);
        }
    }
}
