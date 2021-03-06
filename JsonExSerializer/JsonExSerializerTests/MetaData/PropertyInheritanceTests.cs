﻿using System;
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
            SerializerSettings context = new SerializerSettings();
            TypeData<ChildClass> child = new TypeData<ChildClass>(context);
            Assert.IsNotNull(child.Property(t => t.BaseProperty), "BaseProperty");
            Assert.IsNotNull(child.Property(t => t.ChildProperty), "ChildProperty");
        }

        [Test]
        public void NonOverridenBaseProperty_SharedByChildren()
        {
            SerializerSettings context = new SerializerSettings();
            ITypeData<ChildClass> child = context.Type<ChildClass>();
            ITypeData<BaseClass> baseClass = context.Type<BaseClass>();

            Assert.AreSame(baseClass.Property(t => t.BaseProperty), child.Property(t => t.BaseProperty), "child BaseProperty");
        }

        [Test]
        public void OverridenBaseProperty_NotSharedByChildren()
        {
            SerializerSettings context = new SerializerSettings();
            ITypeData child = context.Types[typeof(ChildClass)];
            ITypeData baseClass = context.Types[typeof(BaseClass)];

            Assert.AreNotSame(baseClass.FindProperty("OverriddenProperty"), child.FindProperty("OverriddenProperty"), "child OverriddenProperty");
        }

        [Test]
        public void ShadowedBaseProperty_NotSharedByChildren()
        {
            SerializerSettings context = new SerializerSettings();
            ITypeData child = context.Types[typeof(ChildClass)];
            ITypeData baseClass = context.Types[typeof(BaseClass)];

            Assert.AreNotSame(baseClass.FindProperty("NewProperty"), child.FindProperty("NewProperty"), "child NewProperty");
        }

        [Test]
        public void IgnoredBaseProperty_SharedByChildren()
        {
            SerializerSettings context = new SerializerSettings();
            ITypeData child = context.Types[typeof(ChildClass)];
            ITypeData baseClass = context.Types[typeof(BaseClass)];

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
