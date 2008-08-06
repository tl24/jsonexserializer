using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializerTests.Mocks;
using JsonExSerializer;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class ReferenceTests
    {
        private MockReferenceObject simple = null;
        private MockReferenceObject deep = null;

        [SetUp]
        public void TestSetup()
        {
            CreateSimpleReference();
            CreateDeepReference();
        }

        private void CreateSimpleReference()
        {            
            simple = new MockReferenceObject();
            MockReferenceObject m2 = new MockReferenceObject();
            simple.Name = "m1";
            m2.Name = "m2";
            simple.Reference = m2;
            m2.Reference = simple;
        }

        private void CreateDeepReference()
        {
            deep = new MockReferenceObject();
            MockReferenceObject m2 = new MockReferenceObject();
            MockReferenceObject m3 = new MockReferenceObject();
            MockReferenceObject m4 = new MockReferenceObject();

            deep.Name = "m1";
            m2.Name = "m2";
            m3.Name = "m3";
            m4.Name = "m4";
            deep.Reference = m2;
            m2.Reference = m3;
            m3.Reference = m4;
            m4.Reference = m2;

        }

        [Test]
        [ExpectedException(typeof(JsonExSerializationException))]
        public void CircularReferenceError()
        {
            Serializer s = new Serializer(typeof(MockReferenceObject));
            s.Context.ReferenceWritingType = SerializationContext.ReferenceOption.ErrorCircularReferences;
            string result = s.Serialize(simple);
            MockReferenceObject actual = (MockReferenceObject)s.Deserialize(result);
        }


        [Test]
        [ExpectedException(typeof(JsonExSerializationException))]
        public void DeepCircularReferenceError()
        {
            Serializer s = new Serializer(typeof(MockReferenceObject));
            s.Context.ReferenceWritingType = SerializationContext.ReferenceOption.ErrorCircularReferences;
            string result = s.Serialize(deep);
            MockReferenceObject actual = (MockReferenceObject)s.Deserialize(result);
        }

        [Test]
        public void CircularReferenceIgnore()
        {
            Serializer s = new Serializer(typeof(MockReferenceObject));
            s.Context.ReferenceWritingType = SerializationContext.ReferenceOption.IgnoreCircularReferences;
            string result = s.Serialize(simple);
            MockReferenceObject actual = (MockReferenceObject)s.Deserialize(result);
            Assert.IsNull(actual.Reference.Reference);
        }

        [Test]
        public void DeepCircularReferenceIgnore()
        {
            Serializer s = new Serializer(typeof(MockReferenceObject));
            s.Context.ReferenceWritingType = SerializationContext.ReferenceOption.IgnoreCircularReferences;
            string result = s.Serialize(deep);
            MockReferenceObject actual = (MockReferenceObject)s.Deserialize(result);
            Assert.IsNull(actual.Reference.Reference.Reference.Reference);

        }

        [Test]
        public void CircularReferenceWrite()
        {
            Serializer s = new Serializer(typeof(MockReferenceObject));
            s.Context.ReferenceWritingType = SerializationContext.ReferenceOption.WriteIdentifier;
            string result = s.Serialize(simple);
            MockReferenceObject actual = (MockReferenceObject)s.Deserialize(result);
            Assert.AreSame(simple, simple.Reference.Reference, "References not equal");
        }

        [Test]
        public void DeepCircularReferenceWrite()
        {
            Serializer s = new Serializer(typeof(MockReferenceObject));
            s.Context.ReferenceWritingType = SerializationContext.ReferenceOption.WriteIdentifier;
            string result = s.Serialize(deep);
            MockReferenceObject actual = (MockReferenceObject)s.Deserialize(result);
            Assert.AreSame(deep.Reference, deep.Reference.Reference.Reference.Reference, "References not equal");
        }

        [Test]
        public void TestCollectionIndexReference()
        {
            MockReferenceObject[] mockArray = new MockReferenceObject[] { simple };
            Serializer s = new Serializer(typeof(MockReferenceObject));
            s.Context.ReferenceWritingType = SerializationContext.ReferenceOption.WriteIdentifier;
            string result = s.Serialize(mockArray);
            MockReferenceObject[] actual = (MockReferenceObject[]) s.Deserialize(result);
            Assert.AreSame(actual[0], actual[0].Reference.Reference, "reference inside collection not equal");

        }
    }
}
