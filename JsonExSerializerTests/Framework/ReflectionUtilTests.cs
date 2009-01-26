using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using System.Reflection;
using JsonExSerializer.Framework;

namespace JsonExSerializerTests.Framework
{
    [TestFixture]
    public class ReflectionUtilTests
    {
        [Test("TestGetAttribute_AttributeExists")]
        public void TestGetAttribute_AttributeExists()
        {
            MethodInfo thisMethod = this.GetType().GetMethod("TestGetAttribute_AttributeExists");
            TestAttribute ta = ReflectionUtils.GetAttribute<TestAttribute>(thisMethod, false);
            Assert.IsNotNull(ta, "No attribute returned");
            Assert.AreEqual("TestGetAttribute_AttributeExists", ta.Description, "Wrong attribute returned");
        }

        [Test]
        public void TestGetAttribute_NoAttributes()
        {
            MethodInfo noAttr = this.GetType().GetMethod("NoAttributes");
            TestAttribute ta = ReflectionUtils.GetAttribute<TestAttribute>(noAttr, false);
            Assert.IsNull(ta, "Wrong attribute returned");
        }

        [Test]
        public void AreEquivalentTypes_WhenTypesSame_ReturnTrue()
        {
            Type t = typeof(int);
            Type b = typeof(int);

            Assert.IsTrue(ReflectionUtils.AreEquivalentTypes(t, b));
        }

        [Test]
        public void AreEquivalentTypes_WhenTypesDifferent_ReturnFalse()
        {
            Type t = typeof(int);
            Type b = typeof(string);

            Assert.IsFalse(ReflectionUtils.AreEquivalentTypes(t, b));
        }

        [Test]
        public void AreEquivalentTypes_WhenNullableTypeAndUnderlyingType_ReturnTrue()
        {
            Type t = typeof(int);
            Type b = typeof(int?);

            Assert.IsTrue(ReflectionUtils.AreEquivalentTypes(t, b));
        }

        public void NoAttributes() { }
    }
}
