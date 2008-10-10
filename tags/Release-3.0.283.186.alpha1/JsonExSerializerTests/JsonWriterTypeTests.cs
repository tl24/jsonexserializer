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
            jsonWriter.WriteCast(typeof(sbyte))
                .WriteValue(33);

            AssertMatch("(System.SByte)33", "Single Item Cast");
        }

        [Test]
        public void ArrayCast()
        {
            jsonWriter.WriteCast(typeof(ArrayList))
                .WriteArrayStart()
                    .WriteValue(1)
                    .WriteValue(2)
                .WriteArrayEnd();

            AssertMatch("(System.Collections.ArrayList)[1,2]", "Array Cast");
        }

        [Test]
        public void ObjectCast()
        {
            jsonWriter.WriteCast(typeof(SimpleObject))
                .WriteObjectStart()
                    .WriteKey("StringValue")
                    .WriteQuotedValue("Test")
                .WriteObjectEnd();

            AssertMatch("(\"JsonExSerializerTests.Mocks.SimpleObject,JsonExSerializerTests\"){\"StringValue\":\"Test\"}", "Object Cast");
        }

        [Test]
        public void DoubleCast()
        {
            long x = (int)(long)3;

            jsonWriter.WriteCast(typeof(int))
                .WriteCast(typeof(long))
                .WriteValue(3);

            AssertMatch("(System.Int32)(System.Int64)3", "Double cast");
        }

        [Test]
        public void CastInsideArray()
        {
            jsonWriter.WriteCast(typeof(ArrayList))
                .WriteArrayStart()
                    .WriteCast(typeof(int)).WriteValue(1)
                    .WriteCast(typeof(int)).WriteValue(2)
                .WriteArrayEnd();

            AssertMatch("(System.Collections.ArrayList)[(System.Int32)1,(System.Int32)2]", "Array Cast");
        }

        [Test]
        public void CastInsideObject()
        {
            jsonWriter.WriteCast(typeof(SimpleObject))
                .WriteObjectStart()
                    .WriteKey("ByteValue")
                    .WriteCast(typeof(byte)).WriteValue(255)
                .WriteObjectEnd();

            AssertMatch("(\"JsonExSerializerTests.Mocks.SimpleObject,JsonExSerializerTests\"){\"ByteValue\":(System.Byte)255}", "Object Cast");
        }

        
        [Test]
        public void EmptyConstructor()
        {
            jsonWriter.WriteConstructorStart(typeof(string))
                .WriteConstructorArgsStart()
                .WriteConstructorArgsEnd()
                .WriteConstructorEnd();
            AssertMatch("new System.String()", "Empty Constructor");
        }

        [Test]
        public void SimpleArgsConstructor()
        {            
            jsonWriter.WriteConstructorStart(typeof(string))
                .WriteConstructorArgsStart()
                .WriteQuotedValue("mystring")
                .WriteConstructorArgsEnd()
                .WriteConstructorEnd();
            AssertMatch("new System.String(\"mystring\")", "Simple Args Constructor");
        }

        [Test]
        public void ArgsWithCastConstructor()
        {            
            jsonWriter.WriteConstructorStart(typeof(string))
                .WriteConstructorArgsStart()
                .WriteCast(typeof(char))
                .WriteQuotedValue("a")
                .WriteValue(10)
                .WriteConstructorArgsEnd()
                .WriteConstructorEnd();
            AssertMatch("new System.String((System.Char)\"a\",10)", "Args with castConstructor");
        }

        [Test]
        public void ArrayInArgsConstructor()
        {            
            jsonWriter.WriteConstructorStart(typeof(string))
                .WriteConstructorArgsStart()
                .WriteCast("char[]")
                .WriteArrayStart()
                    .WriteQuotedValue("t")
                    .WriteQuotedValue("e")
                    .WriteQuotedValue("s")
                    .WriteQuotedValue("t")
                    .WriteQuotedValue("e")
                    .WriteQuotedValue("d")
                .WriteArrayEnd()
                .WriteValue(0)
                .WriteValue(4)
                .WriteConstructorArgsEnd()
                .WriteConstructorEnd();
            AssertMatch("new System.String((char[])[\"t\",\"e\",\"s\",\"t\",\"e\",\"d\"],0,4)", "Array In Args Constructor");
        }

        [Test]
        public void TestObjectInCtorArgs()
        {
            jsonWriter.WriteConstructorStart(typeof(SimpleObject))
                .WriteConstructorArgsStart()
                .WriteObjectStart()
                    .WriteKey("x")
                    .WriteValue(10)
                    .WriteKey("y")
                    .WriteValue(-10)
                .WriteObjectEnd()
                .WriteValue(0)
                .WriteValue(4)
                .WriteConstructorArgsEnd()
                .WriteConstructorEnd();
            AssertMatch("new \"JsonExSerializerTests.Mocks.SimpleObject,JsonExSerializerTests\"({\"x\":10,\"y\":-10},0,4)", "TestObjectInCtorArgs Constructor");
        }

        [Test]
        public void TestConstructorWithInitializer()
        {
            jsonWriter.WriteConstructorStart(typeof(SimpleObject))
                .WriteConstructorArgsStart()
                .WriteObjectStart()
                    .WriteKey("x")
                    .WriteValue(10)
                    .WriteKey("y")
                    .WriteValue(-10)
                .WriteObjectEnd()
                .WriteConstructorArgsEnd()
                .WriteObjectStart()
                .WriteKey("z")
                .WriteValue(-20)
                .WriteKey("q")
                .WriteValue(0)
                .WriteObjectEnd()
                .WriteConstructorEnd();
            AssertMatch("new \"JsonExSerializerTests.Mocks.SimpleObject,JsonExSerializerTests\"({\"x\":10,\"y\":-10}){\"z\":-20,\"q\":0}", "Test Constructor With Initializer");
        }
    }
}
