using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer;

namespace JsonExSerializerTests.ReadOnlyPropertyTests
{
    [TestFixture]
    public class ReadOnlyPropertyTest
    {
        [Test]
        public void OnReadOnlyCollection_CollectionIsSerialized()
        {
            CollParent parent = new CollParent();
            parent.Items.Add(new CollItem("2"));
            parent.Items.Add(new CollItem("Two"));
            Serializer s = Serializer.GetSerializer(typeof(CollParent));
            string result = s.Serialize(parent);
            CollParent actual = (CollParent)s.Deserialize(result);
            Assert.AreEqual(2, actual.Items.Count, "Wrong item count on readonly collection");
            Assert.AreEqual("2", actual.Items[0].Value, "Wrong item in first position");
            Assert.AreEqual("Two", actual.Items[1].Value, "Wrong item in first position");
        }

        [Test]
        public void OnReadOnlyObject_ObjectIsSerialized()
        {
            ObjectParent parent = new ObjectParent();
            parent.Item.Value = "TestValue";
            Serializer s = Serializer.GetSerializer(typeof(ObjectParent));
            string result = s.Serialize(parent);
            ObjectParent actual = (ObjectParent)s.Deserialize(result);
            Assert.AreEqual("TestValue", actual.Item.Value, "Readonly object value not set properly");
        }

        [Test]
        public void OnReadOnlyArray_ExceptionIsThrown()
        {
            ArrayParent parent = new ArrayParent();
            Serializer s = Serializer.GetSerializer(typeof(ArrayParent));
            string result = s.Serialize(parent);
            bool thrown = false;
            try
            {
                ArrayParent actual = (ArrayParent)s.Deserialize(result);
            }
            catch (InvalidOperationException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown, "Expected InvalidOperationException to be thrown when attempting to update get-only array property");
        }
    }
}
