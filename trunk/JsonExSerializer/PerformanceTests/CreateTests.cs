using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using PerformanceTests.TestDomain;
using System.Reflection;
using System.Reflection.Emit;

namespace PerformanceTests
{
    public class CreateTests
    {
        private int iterations = 0;

        public CreateTests(int iterations) {
            this.iterations = iterations;
        }

        public static void RunCreateTests(int iterations)
        {
            CreateTests tests = new CreateTests(iterations);
            tests.DirectTest();
            tests.ActivatorTest();
            tests.DelegateTest();
            tests.ReflectionTest();
        }

        public void DirectTest()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < iterations; i++)
            {
                Customer c = new Customer();
            }
            sw.Stop();
            Console.WriteLine("DirectTest:  " + (sw.ElapsedMilliseconds / (double) iterations));
        }

        public void ActivatorTest()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < iterations; i++)
            {
                Customer c = (Customer) Activator.CreateInstance(typeof(Customer));
            }
            sw.Stop();
            Console.WriteLine("ActivatorTest:  " + (sw.ElapsedMilliseconds / (double) iterations));
        }

        public void DelegateTest()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ConstructDelegate cd = BuildObjectConstructor();
            for (int i = 0; i < iterations; i++)
            {
                Customer c = (Customer)cd();
            }
            sw.Stop();
            Console.WriteLine("DelegateTest:  " + (sw.ElapsedMilliseconds / (double)iterations));
        }

        private ConstructDelegate BuildObjectConstructor()
        {
            Type t = typeof(Customer);
            ConstructorInfo cInfo = t.GetConstructor(Type.EmptyTypes);
            DynamicMethod method = new DynamicMethod(string.Concat("_ctor", "Customer", "_"), typeof(object), Type.EmptyTypes, t);
            ILGenerator generator = method.GetILGenerator();
            // declare return value
            generator.DeclareLocal(typeof(object));
            generator.Emit(OpCodes.Newobj, cInfo);
            //generator.Emit(OpCodes.Stloc_0);
            generator.Emit(OpCodes.Ret);

            return (ConstructDelegate)method.CreateDelegate(typeof(ConstructDelegate));
        }

        private delegate object ConstructDelegate();


        public void ReflectionTest()
        {
            Type t = typeof(Customer);
            ConstructorInfo cInfo = t.GetConstructor(Type.EmptyTypes);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < iterations; i++)
            {
                Customer c = (Customer) cInfo.Invoke(null);
            }
            sw.Stop();
            Console.WriteLine("ReflectionTest:  " + (sw.ElapsedMilliseconds / (double)iterations));
        }
    }
}
