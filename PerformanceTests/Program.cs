using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceTests
{
    class Program
    {
        private static bool RunBinary = false;
        private static bool RunXml = false;
        private static bool RunJson = false;
        private static int ObjectCount = 100;
        private static int Iterations = 2500;
        private static bool Serialize = false;
        private static bool Deserialize = false;

        public static void Main(string[] args)
        {
            bool help = false;
            ProcessOptions(args, ref help);
            if (help)
            {
                ShowHelp();
                return;
            }

            if (RunBinary)
                new BinarySerializerTest(ObjectCount,Iterations).RunTests(Serialize, Deserialize);
            if (RunXml)
                new XmlSerializerTest(ObjectCount, Iterations).RunTests(Serialize, Deserialize);
            if (RunJson)
                new JsonSerializerTest(ObjectCount, Iterations).RunTests(Serialize, Deserialize);
            //new JsonDynamicTests().RunTests();
            //CreateTests.RunCreateTests(10000000);
            //CreateTests.RunCreateTests(10000000);
        }

        public static void ProcessOptions(string[] args, ref bool IsHelp)
        {
            int i = 0;
            IsHelp = false;
            while (i < args.Length)
            {
                string arg = args[i].ToLower();
                if (arg.StartsWith("-xml"))
                    RunXml = true;
                else if (arg.StartsWith("-json"))
                    RunJson = true;
                else if (arg.StartsWith("-bin"))
                    RunBinary = true;
                else if (arg.StartsWith("-i"))
                {
                    i++;
                    Iterations = int.Parse(args[i]);
                }
                else if (arg.StartsWith("-o"))
                {
                    i++;
                    ObjectCount = int.Parse(args[i]);
                }
                else if (arg.StartsWith("-help"))
                {
                    IsHelp = true;
                    break;
                }
                else if (arg.StartsWith("-s"))
                    Serialize = true;
                else if (arg.StartsWith("-d"))
                    Deserialize = true;

                i++;
            }

            if (!RunBinary && !RunJson && !RunXml)
            {
                RunBinary = RunJson = RunXml = true;
            }
            if (!Serialize && !Deserialize)
                Serialize = Deserialize = true;
        }

        public static void ShowHelp()
        {
            Console.WriteLine("PerformanceTests command line options:");
            Console.WriteLine("-xml           : Run the XmlSerializer Test");
            Console.WriteLine("-json          : Run the JsonExSerializer Test");
            Console.WriteLine("-bin[ary]      : Run the BinarySerializer Test");
            Console.WriteLine("-i[terations]  : Number of test iterations");
            Console.WriteLine("-o[bjectcount] : Number of objects to use for the tests");
            Console.WriteLine("-s[erialize]   : Run serializer tests");
            Console.WriteLine("-d[eserialize] : Run deserializer tests");
        }
    }
}
