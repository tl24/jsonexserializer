using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer;
using JsonExSerializer.MetaData;

namespace JsonExSerializerTests.MetaData
{
    [TestFixture]
    public class PropertyInheritanceTests
    {
        [Test]
        public void ChildClassReceivesBaseClassPropertiesPlusItsOwn()
        {
            SerializationContext context = new SerializationContext();
            TypeData child = new TypeData(typeof(ChildClass), context);
            Assert.IsNotNull(child.FindProperty("BaseProperty"), "BaseProperty");
            Assert.IsNotNull(child.FindProperty("ChildProperty"), "ChildProperty");
        }

        [Test]
        public void NonOverridenBaseProperty_SharedByChildren()
        {
            SerializationContext context = new SerializationContext();
            TypeData child = context.TypeHandlerFactory[typeof(ChildClass)];
            TypeData baseClass = context.TypeHandlerFactory[typeof(BaseClass)];

            Assert.AreSame(baseClass.FindProperty("BaseProperty"), child.FindProperty("BaseProperty"), "child BaseProperty");
        }

        [Test]
        public void OverridenBaseProperty_NotSharedByChildren()
        {
            SerializationContext context = new SerializationContext();
            TypeData child = context.TypeHandlerFactory[typeof(ChildClass)];
            TypeData baseClass = context.TypeHandlerFactory[typeof(BaseClass)];

            Assert.AreNotSame(baseClass.FindProperty("OverriddenProperty"), child.FindProperty("OverriddenProperty"), "child OverriddenProperty");
        }

        [Test]
        public void ShadowedBaseProperty_NotSharedByChildren()
        {
            SerializationContext context = new SerializationContext();
            TypeData child = context.TypeHandlerFactory[typeof(ChildClass)];
            TypeData baseClass = context.TypeHandlerFactory[typeof(BaseClass)];

            Assert.AreNotSame(baseClass.FindProperty("NewProperty"), child.FindProperty("NewProperty"), "child NewProperty");
        }

        [Test]
        public void IgnoredBaseProperty_SharedByChildren()
        {
            SerializationContext context = new SerializationContext();
            TypeData child = context.TypeHandlerFactory[typeof(ChildClass)];
            TypeData baseClass = context.TypeHandlerFactory[typeof(BaseClass)];

            Assert.AreSame(baseClass.FindProperty("IgnoredProperty"), child.FindProperty("IgnoredProperty"), "child IgnoredProperty");
        }

        public class BaseClass
        {
            public BaseClass(string BaseConstructorProperty)
            {
            }

            public string BaseProperty
            {
                get { return ""; }
                set { }
            }

            [ConstructorParameter]
            public string BaseConstructorProperty
            {
                get { return ""; }
            }

            public virtual int OverriddenProperty
            {
                get { return 0; }
                set { }
            }

            public int NewProperty
            {
                get { return 0; }
                set { }
            }

            [JsonExIgnore]
            public float IgnoredProperty
            {
                get { return 0f; }
                set { }
            }
        }

        public class ChildClass : BaseClass
        {
            public ChildClass(string BaseConstructorProperty)
                : base(BaseConstructorProperty)
            {
            }

            public bool ChildProperty
            {
                get { return true; }
                set { }
            }

            public override int OverriddenProperty
            {
                get { return base.OverriddenProperty; }
                set { base.OverriddenProperty = value; }
            }

            public new int NewProperty
            {
                get { return 1; }
                set { }
            }
        }
    }
}
