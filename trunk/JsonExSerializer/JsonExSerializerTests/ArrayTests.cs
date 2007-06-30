using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using JsonExSerializer;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class ArrayTests
    {
        [Test]
        public void StringArrayTest()
        {
            string[] value = { "one", "two", "three", "four" };
            Serializer s = Serializer.GetSerializer(value.GetType());
            string result = s.Serialize(value);

            string[] actual = (string[]) s.Deserialize(result);

            CompareArrays<string>(value, actual);
        }

        [Test]
        public void IntArrayTest()
        {
            int[] value = { int.MinValue, 0, int.MaxValue, -2, 35 };
            Serializer s = Serializer.GetSerializer(value.GetType());
            string result = s.Serialize(value);

            int[] actual = (int[])s.Deserialize(result);

            CompareArrays<int>(value, actual);
        }

        [Test]
        public void ObjectArrayTest()
        {            
            int ivalue = 32;
            short svalue = 12;
            string strvalue = "test";
            float fvalue = 34.5f;
            double dvalue = 23456.9888d;
            bool boolvalue = true;
            sbyte sbvalue = 0x1e;
            ushort usvalue = 234;
            uint uivalue = 333;

            object[] value = { ivalue, svalue, strvalue, fvalue, dvalue, boolvalue, sbvalue, usvalue, uivalue };
            Serializer s = Serializer.GetSerializer(value.GetType());
            string result = s.Serialize(value);
            object[] actual = (object[])s.Deserialize(result);
            Assert.AreEqual(ivalue, actual[0]);
            Assert.AreEqual(svalue, actual[1]);
            Assert.AreEqual(strvalue, actual[2]);
            Assert.AreEqual(fvalue, actual[3]);
            Assert.AreEqual(dvalue, actual[4]);
            Assert.AreEqual(boolvalue, actual[5]);
            Assert.AreEqual(sbvalue, actual[6]);
            Assert.AreEqual(usvalue, actual[7]);
            Assert.AreEqual(uivalue, actual[8]);
        }

        protected void CompareArrays<T>(T[] a1, T[] a2)
        {
            int max = Math.Max(a1.Length, a2.Length);
            for (int i = 0; i < max; i++)
            {
                Assert.AreEqual(a1[i], a2[i], "Array elements at index: " + i + " are not equal.");
            }
        }
    }
}
