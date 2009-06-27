using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceTests
{
    public class PerfTestOptions
    {
        public bool RunBinary = false;
        public bool RunXml = false;
        public bool RunJson = false;
        public bool RunTokens = false;
        public int ObjectCount = 100;
        public int Iterations = 2500;
        public bool Serialize = false;
        public bool Deserialize = false;
    }
}
