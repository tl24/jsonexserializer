using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer;
using JsonExSerializerTests.Mocks;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class FieldTests
    {
        [Test]
        public void TestIntField()
        {
            MockFields src = new MockFields();
            src.IntValue = 23;
            Serializer s = Serializer.GetSerializer(typeof(MockFields));
            string result = s.Serialize(src);
            MockFields dest = (MockFields) s.Deserialize(result);
            Assert.AreEqual(23, dest.IntValue);
        }

        [Test]
        public void WhenHasProtectedField_ItsNotSerialized()
        {
            MockFields src = new MockFields();
            src.SetProtected(true);
            Serializer s = Serializer.GetSerializer(typeof(MockFields));
            string result = s.Serialize(src);
            MockFields dest = (MockFields)s.Deserialize(result);
            Assert.IsFalse(dest.GetProtected());            
        }

        [Test]
        public void WhenHasPublicObjectField_ObjectIsSerialized()
        {
            MockFields src = new MockFields();
            SimpleObject so = new SimpleObject();
            so.IntValue = 23;
            src.SimpleObj = so;
            Serializer s = Serializer.GetSerializer(typeof(MockFields));
            string result = s.Serialize(src);
            MockFields dest = (MockFields)s.Deserialize(result);
            Assert.AreEqual(23, dest.SimpleObj.IntValue);

        }
    }
}
