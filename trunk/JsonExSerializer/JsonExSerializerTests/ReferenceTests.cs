using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using JsonExSerializerTests.Mocks;
using JsonExSerializer;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class ReferenceTests
    {
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void CircularReferenceError()
        {
            MockReferenceObject m1 = new MockReferenceObject();
            MockReferenceObject m2 = new MockReferenceObject();
            m1.Name = "m1";
            m2.Name = "m2";
            m1.Reference = m2;
            m2.Reference = m1;

            Serializer s = Serializer.GetSerializer(typeof(MockReferenceObject));
            s.Context.ReferenceWritingType = SerializationContext.ReferenceOption.ErrorCircularReferences;
            string result = s.Serialize(m1);
            MockReferenceObject actual = (MockReferenceObject)s.Deserialize(result);
        }


        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void DeepCircularReferenceError()
        {
            MockReferenceObject m1 = new MockReferenceObject();
            MockReferenceObject m2 = new MockReferenceObject();
            MockReferenceObject m3 = new MockReferenceObject();
            MockReferenceObject m4 = new MockReferenceObject();

            m1.Name = "m1";
            m2.Name = "m2";
            m3.Name = "m3";
            m4.Name = "m4";
            m1.Reference = m2;
            m2.Reference = m3;
            m3.Reference = m4;
            m4.Reference = m2;

            Serializer s = Serializer.GetSerializer(typeof(MockReferenceObject));
            s.Context.ReferenceWritingType = SerializationContext.ReferenceOption.ErrorCircularReferences;
            string result = s.Serialize(m1);
            MockReferenceObject actual = (MockReferenceObject)s.Deserialize(result);
        }

        [Test]
        public void CircularReferenceIgnore()
        {
            MockReferenceObject m1 = new MockReferenceObject();
            MockReferenceObject m2 = new MockReferenceObject();
            m1.Name = "m1";
            m2.Name = "m2";
            m1.Reference = m2;
            m2.Reference = m1;

            Serializer s = Serializer.GetSerializer(typeof(MockReferenceObject));
            s.Context.ReferenceWritingType = SerializationContext.ReferenceOption.IgnoreCircularReferences;
            string result = s.Serialize(m1);
            MockReferenceObject actual = (MockReferenceObject)s.Deserialize(result);
            Assert.IsNull(actual.Reference.Reference);
        }

        [Test]
        public void DeepCircularReferenceIgnore()
        {
            MockReferenceObject m1 = new MockReferenceObject();
            MockReferenceObject m2 = new MockReferenceObject();
            MockReferenceObject m3 = new MockReferenceObject();
            MockReferenceObject m4 = new MockReferenceObject();

            m1.Name = "m1";
            m2.Name = "m2";
            m3.Name = "m3";
            m4.Name = "m4";
            m1.Reference = m2;
            m2.Reference = m3;
            m3.Reference = m4;
            m4.Reference = m2;

            Serializer s = Serializer.GetSerializer(typeof(MockReferenceObject));
            s.Context.ReferenceWritingType = SerializationContext.ReferenceOption.IgnoreCircularReferences;
            string result = s.Serialize(m1);
            MockReferenceObject actual = (MockReferenceObject)s.Deserialize(result);
            Assert.IsNull(actual.Reference.Reference.Reference.Reference);

        }

    }
}
