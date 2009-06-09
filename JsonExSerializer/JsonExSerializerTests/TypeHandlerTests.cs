using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer;
using JsonExSerializer.MetaData;
using JsonExSerializerTests.Mocks;
using JsonExSerializer.Collections;
using MbUnit.Core.Exceptions;
using JsonExSerializer.MetaData.Attributes;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class TypeHandlerTests
    {
        [Test]
        public void IsEmpty_OnEmptyClass_ReturnsTrue()
        {
            SerializationContext context = new SerializationContext();
            TypeData EmptyClassHandler = new TypeData(typeof(EmptyClass), context);
            Assert.IsTrue(EmptyClassHandler.IsEmpty, "IsEmpty should return true on class with no properties/fields");
        }

        [Test]
        public void IsEmpty_OnClassWithOnlyIgnoredFields_ReturnsTrue()
        {
            SerializationContext context = new SerializationContext();
            TypeData IgnoredClassHandler = new TypeData(typeof(IgnoredFieldClass), context);
            Assert.IsTrue(IgnoredClassHandler.IsEmpty, "IsEmpty should return true on class with all properties/fields ignored");
        }

        [Test]
        public void IsEmpty_OnNonEmptyClass_ReturnsFalse()
        {
            SerializationContext context = new SerializationContext();
            TypeData SimpleObjectHandler = new TypeData(typeof(SimpleObject), context);
            Assert.IsFalse(SimpleObjectHandler.IsEmpty, "IsEmpty should return false on class with properties/fields");
        }

        [Test]
        public void IsCollection_ReturnsAttributeValueForJsonExCollection()
        {
            SerializationContext context = new SerializationContext();
            TypeData CollectionTypeHandler = context.TypeHandlerFactory[typeof(StronglyTypedCollection)];
            Assert.IsTrue(CollectionTypeHandler.IsCollection(), "Strongly Typed collection is a collection");
            Assert.IsInstanceOfType(typeof(StronglyTypedCollectionHandler), CollectionTypeHandler.CollectionHandler, "Wrong collection handler");
            Assert.AreSame(typeof(string), CollectionTypeHandler.CollectionHandler.GetItemType(typeof(StronglyTypedCollection)), "Wrong collection item type");
        }

        [Test]
        public void IsCollection_ReturnsDefaultValueForJsonExCollection_WhenItemTypeOnlySpecified()
        {
            SerializationContext context = new SerializationContext();
            TypeData CollectionTypeHandler = context.TypeHandlerFactory[typeof(StronglyTypedCollection2)];
            Assert.IsTrue(CollectionTypeHandler.IsCollection(), "Strongly Typed collection is a collection");
            Assert.IsInstanceOfType(typeof(CollectionHandlerWrapper), CollectionTypeHandler.CollectionHandler, "Wrong collection handler");
            Assert.AreSame(typeof(string), CollectionTypeHandler.CollectionHandler.GetItemType(typeof(StronglyTypedCollection)), "Wrong collection item type");
        }

        [Test]
        public void FindProperty_FindsIgnoredProperties()
        {
            SerializationContext context = new SerializationContext();
            TypeData handler = context.GetTypeHandler(typeof(IgnoredFieldClass));
            IPropertyData fieldHandler = handler.FindProperty("IVal");
            Assert.IsNotNull(fieldHandler, "Ignored property not found");
        }

        [Test]
        public void IgnoreProperty_DoesNotDeleteField()
        {
            SerializationContext context = new SerializationContext();
            TypeData handler = context.GetTypeHandler(typeof(SimpleObject));
            foreach(IPropertyData prop in handler.AllProperties)
                ;  // force properties to load

            handler.IgnoreProperty("IntValue");
            bool found = false;
            foreach (IPropertyData prop in handler.AllProperties)
                if (prop.Name == "IntValue")
                    found = true;
            Assert.IsTrue(found, "Ignored property deleted");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void IgnoreProperty_WhenFieldDoesntExist_ThrowsError()
        {
            SerializationContext context = new SerializationContext();
            TypeData handler = context.GetTypeHandler(typeof(SimpleObject));
                handler.IgnoreProperty("Foo");
        }

        [Test]
        public void AttributeProcessors_CustomProcessor_XmlIgnore()
        {
            SerializationContext context = new SerializationContext();
            context.TypeHandlerFactory.AttributeProcessors.Add(new XmlIgnoreAttributeProcessor());
            TypeData handler = context.GetTypeHandler(typeof(XmlIgnoreMock));
            Assert.IsTrue(handler.FindProperty("Salary").Ignored, "XmlIgnore attribute not ignored");
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
