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
        protected PerfTestOptions options;
        public AbstractPerfTestBase(PerfTestOptions options)
        {
            this.options = options;
            GenerateTestData(options);
        }

        protected virtual void GenerateTestData(PerfTestOptions options)
        {
            TestObjectFactory factory = new TestObjectFactory(options.ObjectCount);
            _testData = factory.ProduceObjects();
        }

        public virtual List<TestResult> RunTests()
        {
            List<TestResult> results = new List<TestResult>();
            if (options.Serialize)
                results.Add(SerializeTest());
            if (options.Deserialize)
                results.Add(DeserializeTest());
            return results;
        }

        public virtual TestResult SerializeTest()
        {
            Stopwatch sw = new Stopwatch();
            PrepareForSerializerTest();
            sw.Start();
            for (int i = 0; i < options.Iterations; i++)
            {
                Serialize(_testData);
            }
            sw.Stop();

            TestResult result = new TestResult(
                "Serialization Performance Test",
                SerializerType,
                new FileInfo(FileName).Length,
                options.Iterations,
                options.ObjectCount,
                sw.ElapsedMilliseconds
             );
            result.WriteToConsole();
            return result;
        }

        protected virtual void PrepareForSerializerTest()
        {
            InitSerializer(_testData.GetType());
        }

        public virtual TestResult DeserializeTest()
        {
            Type t = _testData.GetType();
            PrepareForDeserializeTest();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < options.Iterations; i++)
            {
                object newObject = Deserialize(t);
            }
            sw.Stop();
            TestResult result = new TestResult(
                "Deserialization Performance Test",
                SerializerType,
                new FileInfo(FileName).Length,
                options.Iterations,
                options.ObjectCount,
                sw.ElapsedMilliseconds
             );
            result.WriteToConsole();
            return result;
        }

        protected virtual void PrepareForDeserializeTest()
        {
            InitSerializer(_testData.GetType());
            Serialize(_testData);
        }

        public abstract void Serialize(object o);
        public abstract object Deserialize(Type t);
        public abstract string FileName { get; }
        public abstract string SerializerType { get; }
        public abstract void InitSerializer(Type t);
    }
}
