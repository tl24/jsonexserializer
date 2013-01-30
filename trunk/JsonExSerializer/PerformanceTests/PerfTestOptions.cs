using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceTests
{
    public class PerfTestOptions
    {
        [Flags]
        public enum RunTypes
        {
            None = 0,
            Binary = 1,
            Xml = 2,
            Json = 4,
            Tokens = 8,
            Parser = 16,
        }

        public RunTypes Run;
        public int ObjectCount = 100;
        public int Iterations = 2500;
        public bool Serialize = false;
        public bool Deserialize = false;
    }
}
