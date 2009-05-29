using System;

namespace PerformanceTests
{
    public sealed class TestResult
    {
        private string _testName;
        private string _serializerType;
        private long _fileSize;
        private int _iterations;
        private int _objectCount;
        private long _totalTimeMS;

        public TestResult(string TestName, string SerializerType, long FileSize, int Iterations, int ObjectCount, long TotalTimeMS)
        {
            _testName = TestName;
            _serializerType = SerializerType;
            _fileSize = FileSize;
            _iterations = Iterations;
            _objectCount = ObjectCount;
            _totalTimeMS = TotalTimeMS;
        }

        public void WriteToConsole()
        {
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine(TestName);
            Console.WriteLine(string.Format("Serializer Type: {0}", SerializerType));
            Console.WriteLine(string.Format("File Size      : {0} bytes", FileSize));
            Console.WriteLine(string.Format("Iterations     : {0}", Iterations));
            Console.WriteLine(string.Format("Object Count   : {0}", ObjectCount));
            Console.WriteLine(string.Format("Total Time     : {0}ms", TotalTimeMS));
            Console.WriteLine(string.Format("Avg Time       : {0:f3}ms per iteration", AverageTime));
        }

        public string TestName
        {
            get { return this._testName; }
        }

        public string SerializerType
        {
            get { return this._serializerType; }
        }

        public long FileSize
        {
            get { return this._fileSize; }
        }

        public int Iterations
        {
            get { return this._iterations; }
        }

        public int ObjectCount
        {
            get { return this._objectCount; }
        }

        public long TotalTimeMS
        {
            get { return this._totalTimeMS; }
        }

        public double AverageTime
        {
            get { return TotalTimeMS / (double)Iterations; }
        }
    }
}
