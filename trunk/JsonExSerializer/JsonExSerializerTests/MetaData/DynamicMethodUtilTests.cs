using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializerTests.Mocks;
using System.Reflection;
using JsonExSerializer.MetaData;
using System.Diagnostics;

namespace JsonExSerializerTests.MetaData
{
    [TestFixture]
    public class DynamicMethodUtilTests
    {
        [Test]
        public void StructGetterTest()
        {
            MockValueType pt = new MockValueType(3, 2);
            PropertyInfo property = pt.GetType().GetProperty("X");
            DynamicMethodUtil.GenericGetter getter = DynamicMethodUtil.CreatePropertyGetter(property);
            Assert.AreEqual(pt.X, getter(pt), "Struct getter incorrect");
        }

        [Test]
        public void ObjectGetterTest()
        {
            MyPointConstructor pt = new MyPointConstructor(3, 2);
            PropertyInfo property = pt.GetType().GetProperty("X");
            DynamicMethodUtil.GenericGetter getter = DynamicMethodUtil.CreatePropertyGetter(property);
            Assert.AreEqual(pt.X, getter(pt), "Struct getter incorrect");
        }

        [Test]
        public void ObjectSetterValueTest()
        {
            SimpleObject o = new SimpleObject();
            PropertyInfo property = o.GetType().GetProperty("IntValue");
            DynamicMethodUtil.GenericSetter setter = DynamicMethodUtil.CreatePropertySetter(property);
            setter(o, 32);
            Assert.AreEqual(32, o.IntValue, "Int Property Not Set");
        }

        [Test]
        public void ObjectSetterReferenceTest()
        {
            SimpleObject o = new SimpleObject();
            PropertyInfo property = o.GetType().GetProperty("StringValue");
            DynamicMethodUtil.GenericSetter setter = DynamicMethodUtil.CreatePropertySetter(property);
            setter(o, "TestValue");
            Assert.AreEqual("TestValue", o.StringValue, "String Property Not Set");
        }

        [Test]
        public void ObjectSetterValuePerfTest()
        {
            SimpleObject o = new SimpleObject();
            PropertyInfo property = o.GetType().GetProperty("IntValue");
            MethodInfo method = property.GetSetMethod();
            int iterations = 1000;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < iterations; i++)
            {
                method.Invoke(o, new object[] { 32 });
            }
            sw.Stop();
            Debug.WriteLine("Refelection Test: " + sw.ElapsedMilliseconds);

            sw.Reset();
            sw.Start();
            DynamicMethodUtil.GenericSetter setter = DynamicMethodUtil.CreatePropertySetter(property);
            for (int i = 0; i < iterations; i++)
            {
                setter(o, 32);
            }
            sw.Stop();
            Debug.WriteLine("Refelection Test: " + sw.ElapsedMilliseconds);

        }

    }
}
