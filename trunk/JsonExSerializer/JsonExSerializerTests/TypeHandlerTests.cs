using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer;
using JsonExSerializer.MetaData;
using JsonExSerializerTests.Mocks;
using JsonExSerializer.Collections;
using MbUnit.Core.Exceptions;

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

        [Test]
        public void FindProperty_FindsIgnoredProperties()
        {
            SerializationContext context = new SerializationContext();
            TypeHandler handler = context.GetTypeHandler(typeof(IgnoredFieldClass));
            AbstractPropertyHandler fieldHandler = handler.FindProperty("IVal");
            Assert.IsNotNull(fieldHandler, "Ignored property not found");
        }

        [Test]
        public void IgnoreProperty_DoesNotDeleteField()
        {
            SerializationContext context = new SerializationContext();
            TypeHandler handler = context.GetTypeHandler(typeof(SimpleObject));
            foreach(AbstractPropertyHandler prop in handler.AllProperties)
                ;  // force properties to load

            handler.IgnoreProperty("IntValue");
            bool found = false;
            foreach (AbstractPropertyHandler prop in handler.AllProperties)
                if (prop.Name == "IntValue")
                    found = true;
            Assert.IsTrue(found, "Ignored property deleted");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void IgnoreProperty_WhenFieldDoesntExist_ThrowsError()
        {
            SerializationContext context = new SerializationContext();
            TypeHandler handler = context.GetTypeHandler(typeof(SimpleObject));
                handler.IgnoreProperty("Foo");
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
