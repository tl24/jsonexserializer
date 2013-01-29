using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer;
using System.Collections;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class ArrayTests
    {
        [Test]
        public void StringArrayTest()
        {
            string[] value = { "one", "two", "three", "four" };
            Serializer s = new Serializer();
            string result = s.Serialize(value);

            string[] actual = s.Deserialize<string[]>(result);

            CompareArrays<string>(value, actual);
        }

        [Test]
        public void IntArrayTest()
        {
            int[] value = { int.MinValue, 0, int.MaxValue, -2, 35 };
            Serializer s = new Serializer();
            string result = s.Serialize(value);

            int[] actual = s.Deserialize<int[]>(result);

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
            Serializer s = new Serializer();
            string result = s.Serialize(value);
            object[] actual = s.Deserialize<object[]>(result);
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

        [Test]
        public void EmptyArrayTest()
        {
            List<int> source = new List<int>();
            Serializer s = new Serializer();
            s.Settings.IsCompact = true;
            string result = s.Serialize(source);
            StringAssert.FullMatch(result, @"\s*\[\s*\]\s*");
            List<int> target = s.Deserialize<List<int>>(result);
            Assert.AreEqual(0, target.Count, "List count");
        }

        [Test]
        public void SingleItemArrayTest()
        {
            List<int> source = new List<int>();
            Serializer s = new Serializer();
            s.Settings.IsCompact = true;
            source.Add(5);
            string result = s.Serialize(source);
            StringAssert.FullMatch(result, @"\s*\[\s*5\s*\]\s*");
            List<int> target = s.Deserialize<List<int>>(result);
            Assert.AreEqual(1, target.Count, "List count");
            Assert.AreEqual(5, target[0], "single item");
        }

        [Test]
        public void MissingCommaBetweenArrayItemsThrowsException()
        {
            Serializer s = new Serializer();
            string result = @"[1 2]";
            try
            {
                object obj = s.Deserialize<List<int>>(result);
                Assert.Fail("No exception thrown for object with missing comma");
            }
            catch (ParseException)
            {
            }
            catch (Exception e)
            {
                Assert.Fail("Wrong exception type thrown: " + e);
            }
        }

        protected void CompareArrays<T>(T[] a1, T[] a2)
        {
            int max = Math.Max(a1.Length, a2.Length);
            for (int i = 0; i < max; i++)
            {
                Assert.AreEqual(a1[i], a2[i], "Array elements at index: " + i + " are not equal.");
            }
        }

        [Test]
        public void TestArrayCast()
        {
            object[] data = { 1, "string", new ArrayList() };
            Serializer s = new Serializer();
            string result = s.Serialize(data);
            object[] actual = s.Deserialize<object[]>(result);
            Assert.AreEqual(data.Length, actual.Length, "Invalid counts");
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] is ICollection && actual[i] is ICollection)
                    CollectionAssert.AreEqual((ICollection) data[i], (ICollection) actual[i]);
                else
                    Assert.AreEqual(data[i], actual[i], "Elements " + i + " not equal");
            }
        }
    }
}
