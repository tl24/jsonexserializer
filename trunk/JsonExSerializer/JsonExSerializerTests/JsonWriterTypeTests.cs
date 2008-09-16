using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using System.Collections;
using JsonExSerializerTests.Mocks;
using System.Diagnostics;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class JsonWriterTypeTests : JsonWriterTestBase
    {

        /// <summary>
        /// Asserts that the string matches what was written to the stringWriter without
        /// regard to whitespace.
        /// </summary>
        /// <param name="textToMatch">the text to match against</param>
        /// <param name="description">error description</param>
        public override void AssertMatch(string textToMatch, string description)
        {
            Debug.WriteLine(description);
            Debug.WriteLine(stringWriter.ToString());
            // remove whitespace
            base.AssertMatch(textToMatch, description);
        }

        public virtual void AssertCtorMatch(string textToMatch, string description)
        {

        }
        [Test]
        public void SingleItemCast()
        {
            jsonWriter.Cast(typeof(sbyte))
                .Value(33);

            AssertMatch("(System.SByte)33", "Single Item Cast");
        }

        [Test]
        public void ArrayCast()
        {
            jsonWriter.Cast(typeof(ArrayList))
                .ArrayStart()
                    .Value(1)
                    .Value(2)
                .ArrayEnd();

            AssertMatch("(System.Collections.ArrayList)[1,2]", "Array Cast");
        }

        [Test]
        public void ObjectCast()
        {
            jsonWriter.Cast(typeof(SimpleObject))
                .ObjectStart()
                    .Key("StringValue")
                    .QuotedValue("Test")
                .ObjectEnd();

            AssertMatch("(\"JsonExSerializerTests.Mocks.SimpleObject,JsonExSerializerTests\"){\"StringValue\":\"Test\"}", "Object Cast");
        }

        [Test]
        public void DoubleCast()
        {
            long x = (int)(long)3;

            jsonWriter.Cast(typeof(int))
                .Cast(typeof(long))
                .Value(3);

            AssertMatch("(System.Int32)(System.Int64)3", "Double cast");
        }

        [Test]
        public void CastInsideArray()
        {
            jsonWriter.Cast(typeof(ArrayList))
                .ArrayStart()
                    .Cast(typeof(int)).Value(1)
                    .Cast(typeof(int)).Value(2)
                .ArrayEnd();

            AssertMatch("(System.Collections.ArrayList)[(System.Int32)1,(System.Int32)2]", "Array Cast");
        }

        [Test]
        public void CastInsideObject()
        {
            jsonWriter.Cast(typeof(SimpleObject))
                .ObjectStart()
                    .Key("ByteValue")
                    .Cast(typeof(byte)).Value(255)
                .ObjectEnd();

            AssertMatch("(\"JsonExSerializerTests.Mocks.SimpleObject,JsonExSerializerTests\"){\"ByteValue\":(System.Byte)255}", "Object Cast");
        }

        
        [Test]
        public void EmptyConstructor()
        {
            jsonWriter.ConstructorStart(typeof(string))
                .ConstructorArgsStart()
                .ConstructorArgsEnd()
                .ConstructorEnd();
            AssertMatch("new System.String()", "Empty Constructor");
        }

        [Test]
        public void SimpleArgsConstructor()
        {            
            jsonWriter.ConstructorStart(typeof(string))
                .ConstructorArgsStart()
                .QuotedValue("mystring")
                .ConstructorArgsEnd()
                .ConstructorEnd();
            AssertMatch("new System.String(\"mystring\")", "Simple Args Constructor");
        }

        [Test]
        public void ArgsWithCastConstructor()
        {            
            jsonWriter.ConstructorStart(typeof(string))
                .ConstructorArgsStart()
                .Cast(typeof(char))
                .QuotedValue("a")
                .Value(10)
                .ConstructorArgsEnd()
                .ConstructorEnd();
            AssertMatch("new System.String((System.Char)\"a\",10)", "Args with castConstructor");
        }

        [Test]
        public void ArrayInArgsConstructor()
        {            
            jsonWriter.ConstructorStart(typeof(string))
                .ConstructorArgsStart()
                .Cast("char[]")
                .ArrayStart()
                    .QuotedValue("t")
                    .QuotedValue("e")
                    .QuotedValue("s")
                    .QuotedValue("t")
                    .QuotedValue("e")
                    .QuotedValue("d")
                .ArrayEnd()
                .Value(0)
                .Value(4)
                .ConstructorArgsEnd()
                .ConstructorEnd();
            AssertMatch("new System.String((char[])[\"t\",\"e\",\"s\",\"t\",\"e\",\"d\"],0,4)", "Array In Args Constructor");
        }

        [Test]
        public void TestObjectInCtorArgs()
        {
            jsonWriter.ConstructorStart(typeof(SimpleObject))
                .ConstructorArgsStart()
                .ObjectStart()
                    .Key("x")
                    .Value(10)
                    .Key("y")
                    .Value(-10)
                .ObjectEnd()
                .Value(0)
                .Value(4)
                .ConstructorArgsEnd()
                .ConstructorEnd();
            AssertMatch("new \"JsonExSerializerTests.Mocks.SimpleObject,JsonExSerializerTests\"({\"x\":10,\"y\":-10},0,4)", "TestObjectInCtorArgs Constructor");
        }

        [Test]
        public void TestConstructorWithInitializer()
        {
            jsonWriter.ConstructorStart(typeof(SimpleObject))
                .ConstructorArgsStart()
                .ObjectStart()
                    .Key("x")
                    .Value(10)
                    .Key("y")
                    .Value(-10)
                .ObjectEnd()
                .ConstructorArgsEnd()
                .ObjectStart()
                .Key("z")
                .Value(-20)
                .Key("q")
                .Value(0)
                .ObjectEnd()
                .ConstructorEnd();
            AssertMatch("new \"JsonExSerializerTests.Mocks.SimpleObject,JsonExSerializerTests\"({\"x\":10,\"y\":-10}){\"z\":-20,\"q\":0}", "Test Constructor With Initializer");
        }
    }
}
