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

        public void NoAttributes() { }
    }
}
