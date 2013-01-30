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

namespace JsonExSerializerTests.MetaData
{
    [TestFixture]
    public class TypeHandlerTests
    {

        [Test]
        public void IsCollection_ReturnsAttributeValueForJsonExCollection()
        {
            SerializerSettings context = new SerializerSettings();
            ITypeData CollectionTypeHandler = context.Types[typeof(StronglyTypedCollection)];
            Assert.IsTrue(CollectionTypeHandler.IsCollection(), "Strongly Typed collection is a collection");
            Assert.IsInstanceOfType(typeof(StronglyTypedCollectionHandler), CollectionTypeHandler.CollectionHandler, "Wrong collection handler");
            Assert.AreSame(typeof(string), CollectionTypeHandler.CollectionHandler.GetItemType(typeof(StronglyTypedCollection)), "Wrong collection item type");
        }

        [Test]
        public void IsCollection_ReturnsDefaultValueForJsonExCollection_WhenItemTypeOnlySpecified()
        {
            SerializerSettings context = new SerializerSettings();
            ITypeData CollectionTypeHandler = context.Types[typeof(StronglyTypedCollection2)];
            Assert.IsTrue(CollectionTypeHandler.IsCollection(), "Strongly Typed collection is a collection");
            Assert.IsInstanceOfType(typeof(CollectionHandlerWrapper), CollectionTypeHandler.CollectionHandler, "Wrong collection handler");
            Assert.AreSame(typeof(string), CollectionTypeHandler.CollectionHandler.GetItemType(typeof(StronglyTypedCollection)), "Wrong collection item type");
        }

        [Test]
        public void FindProperty_FindsIgnoredProperties()
        {
            SerializerSettings context = new SerializerSettings();
            ITypeData handler = context.Type<IgnoredFieldClass>();
            IPropertyData fieldHandler = handler.FindProperty("IVal");
            Assert.IsNotNull(fieldHandler, "Ignored property not found");
        }

        [Test]
        public void IgnoreProperty_DoesNotDeleteField()
        {
            SerializerSettings context = new SerializerSettings();
            ITypeData handler = context.Type<SimpleObject>();
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
            SerializerSettings context = new SerializerSettings();
            ITypeData handler = context.Type<SimpleObject>();
                handler.IgnoreProperty("Foo");
        }

        [Test]
        public void AttributeProcessors_CustomProcessor_XmlIgnore()
        {
            SerializerSettings context = new SerializerSettings();
            context.Types.AttributeProcessors.Add(new XmlIgnoreAttributeProcessor());
            ITypeData handler = context.Type<XmlIgnoreMock>();
            Assert.IsTrue(handler.FindProperty("Salary").Ignored, "XmlIgnore attribute not ignored");
        }

        [Test]
        public void Ignored_WhenInternalSetter_DefaultsToTrue()
        {
            SerializerSettings context = new SerializerSettings();
            ITypeData handler = context.Types[typeof(NonWritableProperty)];
            Assert.IsTrue(handler.FindProperty("InternalInt").Ignored);
        }

        [Test]
        public void Ignored_WhenPrivateSetter_DefaultsToTrue()
        {
            SerializerSettings context = new SerializerSettings();
            ITypeData handler = context.Types[typeof(NonWritableProperty)];
            Assert.IsTrue(handler.FindProperty("PrivateInt").Ignored);
        }

        [Test]
        public void Ignored_WhenProtectedSetter_DefaultsToTrue()
        {
            SerializerSettings context = new SerializerSettings();
            ITypeData handler = context.Types[typeof(NonWritableProperty)];
            Assert.IsTrue(handler.FindProperty("ProtectedInt").Ignored);
        }

        [Test]
        public void Ignored_WhenNoGetter_DefaultsToTrue()
        {
            SerializerSettings context = new SerializerSettings();
            ITypeData handler = context.Types[typeof(NonWritableProperty)];
            Assert.IsTrue(handler.FindProperty("NoGetter").Ignored);
        }

        [Test]
        public void Ignored_WhenNoSetter_DefaultsToTrue()
        {
            SerializerSettings context = new SerializerSettings();
            ITypeData handler = context.Types[typeof(NonWritableProperty)];
            Assert.IsTrue(handler.FindProperty("NoSetter").Ignored);
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


        public class NonWritableProperty
        {

            public int InternalInt
            {
                get { return 0; }
                internal set {  }
            }

            public int PrivateInt
            {
                get { return 0; }
                private set {  }
            }

            public int ProtectedInt
            {
                get { return 0; }
                protected set {  }
            }

            public int NoGetter
            {
                set {  }
            }

            public int NoSetter
            {
                get { return 0; }
            }
        }

    }

    
}
