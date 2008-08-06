using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceTests
{
    class Program
    {
        public static void Main(string[] args)
        {
            //new BinarySerializerTest().RunTests();
            //new XmlSerializerTest().RunTests();
            //new JsonSerializerTest().RunTests();
            new JsonDynamicTests().RunTests();
            //CreateTests.RunCreateTests(10000000);
            //CreateTests.RunCreateTests(10000000);
        }
    }
}
