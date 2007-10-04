using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializerTests.Mocks;
using System.Collections;
using JsonExSerializer;
using System.Diagnostics;
using System.IO;

namespace JsonExSerializerTests
{
    [TestFixture]
    [Ignore]
    public class PerfTests
    {
        [Row(100, 100)]
        [Row(1000, 10000)]
        [Row(100, 5000)]
        [RowTest]
        public void SerializeTest(int objectCount, int iterations)
        {
            TestObjectFactory factory = new TestObjectFactory(objectCount);
            object o = factory.ProduceObjects();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < iterations; i++)
            {
                Serializer s = Serializer.GetSerializer(o.GetType());
                using (StreamWriter fs = new StreamWriter("test.jsx", false))
                {
                    s.Serialize(o, fs);
                }
            }
            sw.Stop();
            Debug.WriteLine("Serialization Performance Test");
            Debug.WriteLine(string.Format("Iterations: {0}", iterations));
            Debug.WriteLine(string.Format("Object Count: {0}", objectCount));
            Debug.WriteLine(string.Format("Total Time: {0}ms", sw.ElapsedMilliseconds));
            Debug.WriteLine(string.Format("Avg Time: {0}ms per iteration", sw.ElapsedMilliseconds / iterations));
        }

        [Row(100, 100)]
        [Row(1000, 10000)]
        [Row(100, 5000)]
        [RowTest]
        public void DeserializeTest(int objectCount, int iterations)
        {
            TestObjectFactory factory = new TestObjectFactory(objectCount);
            object o = factory.ProduceObjects();
            Serializer s = Serializer.GetSerializer(o.GetType());
            using (StreamWriter fs = new StreamWriter("test.jsx", false))
            {
                s.Serialize(o, fs);
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < iterations; i++)
            {
                s = Serializer.GetSerializer(o.GetType());
                using (StreamReader fr = new StreamReader("test.jsx"))
                {
                    object newObject = s.Deserialize(fr);
                }
            }
            sw.Stop();
            Debug.WriteLine("Deserialization Performance Test");
            Debug.WriteLine(string.Format("Iterations: {0}", iterations));
            Debug.WriteLine(string.Format("Object Count: {0}", objectCount));
            Debug.WriteLine(string.Format("Total Time: {0}ms", sw.ElapsedMilliseconds));
            Debug.WriteLine(string.Format("Avg Time: {0}ms per iteration", sw.ElapsedMilliseconds / iterations));
        }

    }

    public class TestObjectFactory
    {
        private int _maxObjectCount;
        private int _objectCount;

        public TestObjectFactory(int maxObjectCount)
        {
            _maxObjectCount = maxObjectCount;
        }

        public object ProduceObjects()
        {
            _objectCount = 0;
            return ProduceDictionary();
        }

        private object ProduceDictionary()
        {            
            Dictionary<string, ArrayList> objects = new Dictionary<string, ArrayList>();
            int index = 1;
            while (_objectCount < _maxObjectCount)
            {
                ArrayList al = new ArrayList();
                for (int i = 0; i < index && _objectCount < _maxObjectCount; i++, _objectCount++)
                {
                    SemiComplexObject sco = new SemiComplexObject(_objectCount);
                    sco.Name = "Name" + _objectCount;
                    sco.SimpleObject = ProduceSimpleObject();
                }
                objects["key" + index] = al;
                index *= 2;
            }
            return objects;
        }

        private SimpleObject ProduceSimpleObject()
        {
            SimpleObject so = new SimpleObject();
            so.BoolValue = true;
            so.ByteValue = 128;
            so.CharValue = 'z';
            so.DoubleValue = double.MaxValue;
            so.EnumValue = SimpleEnum.EnumValue2;
            so.FloatValue = float.PositiveInfinity;
            so.IntValue = 32;
            so.LongValue = 23400000;
            so.ShortValue = 32000;
            so.StringValue = "This is a simple object test";
            return so;
        }
    }
}
