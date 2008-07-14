using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer;
using JsonExSerializer.MetaData;
using JsonExSerializerTests.Mocks;
using JsonExSerializer.Collections;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class TypeHandlerTests
    {
        [Test]
        public void IsEmpty_OnEmptyClass_ReturnsTrue()
        {
            SerializationContext context = new SerializationContext();
            TypeHandler EmptyClassHandler = new TypeHandler(typeof(EmptyClass), context);
            Assert.IsTrue(EmptyClassHandler.IsEmpty, "IsEmpty should return true on class with no properties/fields");
        }

        [Test]
        public void IsEmpty_OnClassWithOnlyIgnoredFields_ReturnsTrue()
        {
            SerializationContext context = new SerializationContext();
            TypeHandler IgnoredClassHandler = new TypeHandler(typeof(IgnoredFieldClass), context);
            Assert.IsTrue(IgnoredClassHandler.IsEmpty, "IsEmpty should return true on class with all properties/fields ignored");
        }

        [Test]
        public void IsEmpty_OnNonEmptyClass_ReturnsFalse()
        {
            SerializationContext context = new SerializationContext();
            TypeHandler SimpleObjectHandler = new TypeHandler(typeof(SimpleObject), context);
            Assert.IsFalse(SimpleObjectHandler.IsEmpty, "IsEmpty should return false on class with properties/fields");
        }

        [Test]
        public void IsCollection_ReturnsAttributeValueForJsonExCollection()
        {
            SerializationContext context = new SerializationContext();
            TypeHandler CollectionTypeHandler = new TypeHandler(typeof(StronglyTypedCollection), context);
            Assert.IsTrue(CollectionTypeHandler.IsCollection(), "Strongly Typed collection is a collection");
            Assert.IsInstanceOfType(typeof(StronglyTypedCollectionHandler), CollectionTypeHandler.GetCollectionHandler(), "Wrong collection handler");
            Assert.AreSame(typeof(string), CollectionTypeHandler.GetCollectionHandler().GetItemType(typeof(StronglyTypedCollection)), "Wrong collection item type");
        }

        [Test]
        public void IsCollection_ReturnsDefaultValueForJsonExCollection_WhenItemTypeOnlySpecified()
        {
            SerializationContext context = new SerializationContext();
            TypeHandler CollectionTypeHandler = new TypeHandler(typeof(StronglyTypedCollection2), context);
            Assert.IsTrue(CollectionTypeHandler.IsCollection(), "Strongly Typed collection is a collection");
            Assert.IsInstanceOfType(typeof(CollectionHandlerWrapper), CollectionTypeHandler.GetCollectionHandler(), "Wrong collection handler");
            Assert.AreSame(typeof(string), CollectionTypeHandler.GetCollectionHandler().GetItemType(typeof(StronglyTypedCollection)), "Wrong collection item type");
        }

        public class EmptyClass
        {
        }

        public class IgnoredFieldClass
        {
            private int iVal;

            [JsonExIgnore]
            public int IVal
            {
                get { return this.iVal; }
                set { this.iVal = value; }
            }
        }




    }

    
}
