using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceTests
{
    class Program
    {
        public static void Main(string[] args)
        {
            bool help = false;
            PerfTestOptions options = ProcessOptions(args, ref help);
            if (help)
            {
                ShowHelp();
                return;
            }

            if ((options.Run & PerfTestOptions.RunTypes.Binary) > 0)
                new BinarySerializerTest(options).RunTests();
            if ((options.Run & PerfTestOptions.RunTypes.Xml) > 0)
                new XmlSerializerTest(options).RunTests();
            if ((options.Run & PerfTestOptions.RunTypes.Json) > 0)
                new JsonSerializerTest(options).RunTests();
            if ((options.Run & PerfTestOptions.RunTypes.Tokens) > 0)
                new TokenStreamTest(options).RunTests();
            if ((options.Run & PerfTestOptions.RunTypes.Parser) > 0)
                new ParserTests(options).RunTests();
            if ((options.Run & PerfTestOptions.RunTypes.Dynamic) > 0)
                new JsonDynamicTests(options).RunTests();

            //new JsonDynamicTests().RunTests();
            //CreateTests.RunCreateTests(10000000);
            //CreateTests.RunCreateTests(10000000);
        }

        public static PerfTestOptions ProcessOptions(string[] args, ref bool IsHelp)
        {
            int i = 0;
            IsHelp = false;
            PerfTestOptions options = new PerfTestOptions();
            while (i < args.Length)
            {
                string arg = args[i].ToLower();
                if (arg.StartsWith("-xml"))
                    options.Run |= PerfTestOptions.RunTypes.Xml;
                else if (arg.StartsWith("-json"))
                    options.Run |= PerfTestOptions.RunTypes.Json;
                else if (arg.StartsWith("-bin"))
                    options.Run |= PerfTestOptions.RunTypes.Binary;
                else if (arg.StartsWith("-tok"))
                    options.Run |= PerfTestOptions.RunTypes.Tokens;
                else if (arg.StartsWith("-parse"))
                    options.Run |= PerfTestOptions.RunTypes.Parser;
                else if (arg.StartsWith("-dyn"))
                    options.Run |= PerfTestOptions.RunTypes.Dynamic;
                else if (arg.StartsWith("-i"))
                {
                    i++;
                    options.Iterations = int.Parse(args[i]);
                }
                else if (arg.StartsWith("-o"))
                {
                    i++;
                    options.ObjectCount = int.Parse(args[i]);
                }
                else if (arg.StartsWith("-help"))
                {
                    IsHelp = true;
                    break;
                }
                else if (arg.StartsWith("-s"))
                    options.Serialize = true;
                else if (arg.StartsWith("-d"))
                    options.Deserialize = true;

                i++;
            }

            if (options.Run == 0)
            {
                options.Run = PerfTestOptions.RunTypes.Binary | PerfTestOptions.RunTypes.Xml | PerfTestOptions.RunTypes.Json;
            }
            if (!options.Serialize && !options.Deserialize)
                options.Serialize = options.Deserialize = true;

            return options;
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
