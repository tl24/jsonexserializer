using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using JsonExSerializer;
using System.Diagnostics;
using System.IO;
using PerformanceTests;

namespace PerformanceTests
{
   
    public abstract class AbstractPerfTestBase
    {

        private object _testData;
        private int _objectCount;
        private int _iterations;

        public AbstractPerfTestBase(int ObjectCount, int Iterations)
        {
            _objectCount = ObjectCount;
            _iterations = Iterations;
            TestObjectFactory factory = new TestObjectFactory(_objectCount);
            _testData = factory.ProduceObjects();
        }
        public List<TestResult> RunTests()
        {
            List<TestResult> results = new List<TestResult>();
            //results.Add(SerializeTest(_objectCount, 1000));
            //results.Add(DeserializeTest(_objectCount, 1000));

            //results.Add(SerializeTest(_objectCount, 10000));
            results.Add(SerializeTest(_objectCount, _iterations));

            //results.Add(DeserializeTest(_objectCount, 10000));
            results.Add(DeserializeTest(_objectCount, _iterations));
            return results;
        }

        public TestResult SerializeTest(int objectCount, int iterations)
        {
            Stopwatch sw = new Stopwatch();
            InitSerializer(_testData.GetType());
            sw.Start();
            for (int i = 0; i < iterations; i++)
            {
                Serialize(_testData);
            }
            sw.Stop();

            TestResult result = new TestResult(
                "Serialization Performance Test",
                SerializerType,
                new FileInfo(FileName).Length,
                iterations,
                objectCount,
                sw.ElapsedMilliseconds
             );
            result.WriteToConsole();
            return result;
        }

        public TestResult DeserializeTest(int objectCount, int iterations)
        {
            Type t = _testData.GetType();
            InitSerializer(t);
            Serialize(_testData);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < iterations; i++)
            {
                object newObject = Deserialize(t);
            }
            sw.Stop();
            TestResult result = new TestResult(
                "Deserialization Performance Test",
                SerializerType,
                new FileInfo(FileName).Length,
                iterations,
                objectCount,
                sw.ElapsedMilliseconds
             );
            result.WriteToConsole();
            return result;
        }

        public abstract void Serialize(object o);
        public abstract object Deserialize(Type t);
        public abstract string FileName { get; }
        public abstract string SerializerType { get; }
        public abstract void InitSerializer(Type t);
    }
}
