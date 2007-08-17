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

            AssertMatch("(JsonExSerializerTests.Mocks.SimpleObject){\"StringValue\":\"Test\"}", "Object Cast");
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

            AssertMatch("(JsonExSerializerTests.Mocks.SimpleObject){\"ByteValue\":(System.Byte)255}", "Object Cast");
        }

        /*
        [Test]
        [Ignore]
        public void EmptyConstructor()
        {
            jsonWriter.ConstructorStart(typeof(string))
                .ConstructorEnd();
            AssertMatch("new System.String()", "Empty Constructor");
        }
         */
    }
}
