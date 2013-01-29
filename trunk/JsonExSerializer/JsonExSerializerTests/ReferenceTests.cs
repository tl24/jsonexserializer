using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializerTests.Mocks;
using JsonExSerializer;
using System.Collections;
using JsonExSerializer.TypeConversion;

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
        [ExpectedException(typeof(InvalidOperationException))]
        public void CircularReferenceError()
        {
            Serializer s = new Serializer();
            s.Settings.ReferenceWritingType = ReferenceOption.ErrorCircularReferences;
            string result = s.Serialize(simple);
            MockReferenceObject actual = s.Deserialize<MockReferenceObject>(result);
        }


        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeepCircularReferenceError()
        {
            Serializer s = new Serializer();
            s.Settings.ReferenceWritingType = ReferenceOption.ErrorCircularReferences;
            string result = s.Serialize(deep);
            MockReferenceObject actual = s.Deserialize<MockReferenceObject>(result);
        }

        [Test]
        public void CircularReferenceIgnore()
        {
            Serializer s = new Serializer();
            s.Settings.ReferenceWritingType = ReferenceOption.IgnoreCircularReferences;
            string result = s.Serialize(simple);
            MockReferenceObject actual = s.Deserialize<MockReferenceObject>(result);
            Assert.IsNull(actual.Reference.Reference);
        }

        [Test]
        public void DeepCircularReferenceIgnore()
        {
            Serializer s = new Serializer();
            s.Settings.ReferenceWritingType = ReferenceOption.IgnoreCircularReferences;
            string result = s.Serialize(deep);
            MockReferenceObject actual = s.Deserialize<MockReferenceObject>(result);
            Assert.IsNull(actual.Reference.Reference.Reference.Reference);

        }

        [Test]
        public void CircularReferenceWrite()
        {
            Serializer s = new Serializer();
            s.Settings.ReferenceWritingType = ReferenceOption.WriteIdentifier;
            string result = s.Serialize(simple);
            MockReferenceObject actual = s.Deserialize<MockReferenceObject>(result);
            Assert.AreSame(simple, simple.Reference.Reference, "References not equal");
        }

        [Test]
        public void TestBackwardsCompatibleReference()
        {
            Serializer s = new Serializer();
            s.Settings.ReferenceWritingType = ReferenceOption.WriteIdentifier;
            s.Settings.IsCompact = true;
            string result = s.Serialize(simple);
            result = result.Replace("$", "this");
            MockReferenceObject actual = s.Deserialize<MockReferenceObject>(result);
            Assert.AreSame(simple, simple.Reference.Reference, "References not equal");
        }

        [Test]
        public void DeepCircularReferenceWrite()
        {
            Serializer s = new Serializer();
            s.Settings.ReferenceWritingType = ReferenceOption.WriteIdentifier;
            string result = s.Serialize(deep);
            MockReferenceObject actual = s.Deserialize<MockReferenceObject>(result);
            Assert.AreSame(deep.Reference, deep.Reference.Reference.Reference.Reference, "References not equal");
        }

        [Test]
        public void BackwardsCompatibleDeepWrite()
        {
            Serializer s = new Serializer();
            s.Settings.ReferenceWritingType = ReferenceOption.WriteIdentifier;
            s.Settings.IsCompact = true;
            string result = s.Serialize(deep);
            result = result.Replace("$['Reference']", "this.Reference");
            MockReferenceObject actual = s.Deserialize<MockReferenceObject>(result);
            Assert.AreSame(deep.Reference, deep.Reference.Reference.Reference.Reference, "References not equal");
        }

        [Test]
        public void TestCollectionIndexReference()
        {
            MockReferenceObject[] mockArray = new MockReferenceObject[] { simple };
            Serializer s = new Serializer();
            s.Settings.ReferenceWritingType = ReferenceOption.WriteIdentifier;
            string result = s.Serialize(mockArray);
            MockReferenceObject[] actual = s.Deserialize<MockReferenceObject[]>(result);
            Assert.AreSame(actual[0], actual[0].Reference.Reference, "reference inside collection not equal");
        }

        [Test]
        public void WhenSameString_ShouldNotBeReference()
        {
            string refString = "foo";
            ArrayList al = new ArrayList();
            al.Add(refString);
            al.Add(refString);
            Serializer s = new Serializer();
            s.Settings.ReferenceWritingType = ReferenceOption.WriteIdentifier;
            string result = s.Serialize(al);
            StringAssert.NotLike(result, "this");
            StringAssert.NotLike(result, @"\$");
        }

        [Test]
        public void ReferenceShouldNotWriteCast()
        {
            MockSubReferenceObject msr = new MockSubReferenceObject();
            msr.Reference = msr;
            Serializer s = new Serializer();
            s.Settings.ReferenceWritingType = ReferenceOption.WriteIdentifier;
            s.Settings.IsCompact = true;
            string result = s.Serialize(msr);
            StringAssert.NotLike(result, "MockSubReferenceObject");
        }

        [Test]
        public void TypeConverterControlsHowReferencesWritten()
        {
            Type intType = typeof(int);
            ArrayList list = new ArrayList();
            list.Add(intType);
            list.Add(intType);
            Serializer s = new Serializer();
            s.Settings.ReferenceWritingType = ReferenceOption.WriteIdentifier;
            s.Settings.RegisterTypeConverter(typeof(Type), new TypeToStringConverter());
            string result = s.Serialize(list);
            StringAssert.NotLike(result, @"\$");
            StringAssert.NotLike(result, "this");
        }
    }
}
