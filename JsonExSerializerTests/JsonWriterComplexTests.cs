using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using System.IO;
using JsonExSerializer;
using JsonExSerializerTests.Mocks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class JsonWriterComplexTests : JsonWriterTestBase
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

        [Test]
        public void SimpleArrayTest()
        {
            jsonWriter
                .WriteArrayStart()
                    .WriteValue(true)
                    .WriteValue(1)
                    .WriteQuotedValue("test")
                    .WriteValue(3.14f)
                    .WriteValue(345.8765)
                .WriteArrayEnd();
            AssertMatch("[true,1,\"test\",3.14,345.8765]", "simple array test");
        }

        [Test]
        public void ArrayOfArraysTest()
        {
            jsonWriter
                .WriteArrayStart()
                    .WriteArrayStart()
                        .WriteQuotedValue("one")
                    .WriteArrayEnd()
                    .WriteValue(1)
                    .WriteArrayStart()
                        .WriteQuotedValue("two")
                        .WriteValue(3)
                    .WriteArrayEnd()
                    .WriteValue(2)
                .WriteArrayEnd();

            AssertMatch("[[\"one\"],1,[\"two\",3],2]", "Array of arrays test");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValueAfterArrayIsErrorTest()
        {
            jsonWriter
                .WriteArrayStart()
                    .WriteValue(1)
                .WriteArrayEnd()
                .WriteValue(true);    // this should cause an error, can only write one value/object/array            
        }

        [Test]
        public void EmptyArray()
        {
            jsonWriter
                .WriteArrayStart()
                .WriteArrayEnd();
            AssertMatch("[]", "The empty array");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void KeyInArrayIsErrorTest()
        {
            jsonWriter
                .WriteArrayStart()
                    .WriteKey("invalidKey")  // key's not allowed within array
                    .WriteQuotedValue("myvalue")
                .WriteArrayEnd();
        }

        [Test]
        public void EmptyObjectTest()
        {
            jsonWriter
                .WriteObjectStart()
                .WriteObjectEnd();
            AssertMatch("{}", "The empty object");
        }

        [Test]
        public void OneItemObjectTest()
        {
            jsonWriter
                .WriteObjectStart()
                    .WriteKey("KeyText")
                    .WriteQuotedValue("ValueText")
                .WriteObjectEnd();

            AssertMatch("{\"KeyText\":\"ValueText\"}", "One item object test");
        }

        [Test]
        public void MultipleItemObjectTest()
        {
            jsonWriter
                .WriteObjectStart()
                    .WriteKey("StringValue")
                    .WriteQuotedValue("ValueText")
                    .WriteKey("LongVal")
                    .WriteValue(1234)
                    .WriteKey("BoolVal")
                    .WriteValue(true)
                .WriteObjectEnd();

            AssertMatch("{\"StringValue\":\"ValueText\",\"LongVal\":1234,\"BoolVal\":true}", "");
        }

        [Test]
        public void ObjectOfObjectsTest()
        {
            jsonWriter
                .WriteObjectStart()
                    .WriteKey("StringValue")
                    .WriteQuotedValue("ValueText")
                    .WriteKey("LongVal")
                    .WriteValue(1234)
                    .WriteKey("BoolVal")
                    .WriteValue(true)
                    .WriteKey("ComplexObject")
                    .WriteObjectStart()
                        .WriteKey("KeyText")
                        .WriteQuotedValue("ValueText")
                    .WriteObjectEnd()
                .WriteObjectEnd();
            AssertMatch("{\"StringValue\":\"ValueText\",\"LongVal\":1234,\"BoolVal\":true,\"ComplexObject\":{\"KeyText\":\"ValueText\"}}", "");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValueAfterObjectIsErrorTest()
        {
            jsonWriter
                .WriteObjectStart()
                    .WriteKey("test")
                    .WriteValue(1)
                .WriteObjectEnd()
                .WriteValue(true);    // this should cause an error, can only write one value/object/array            
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MissingKeyObjectTest()
        {
            jsonWriter
                .WriteObjectStart()
                    // key is missing
                    .WriteValue(123)
                .WriteObjectEnd();
        }

        [Test]
        public void ObjectInArrayTest()
        {
            jsonWriter
                .WriteArrayStart()
                    .WriteValue(true)
                    .WriteValue(1)
                    .WriteQuotedValue("test")
                    .WriteValue(3.14f)
                    .WriteValue(345.8765)
                    .WriteObjectStart()
                        .WriteKey("KeyText")
                        .WriteQuotedValue("ValueText")
                    .WriteObjectEnd()
                .WriteArrayEnd();
            AssertMatch("[true,1,\"test\",3.14,345.8765,{\"KeyText\":\"ValueText\"}]", "object inside array test");
        }

        [Test]
        public void ArrayInObjectTest()
        {
            jsonWriter
                .WriteObjectStart()
                    .WriteKey("FirstArray")
                    .WriteArrayStart()
                        .WriteValue(123)
                        .WriteValue(true)
                    .WriteArrayEnd()
                    .WriteKey("KeyText")
                    .WriteQuotedValue("ValueText")
                    .WriteKey("LastArray")
                    .WriteArrayStart()
                        .WriteQuotedValue("last")
                    .WriteArrayEnd()
                .WriteObjectEnd();

            AssertMatch("{\"FirstArray\":[123,true],\"KeyText\":\"ValueText\",\"LastArray\":[\"last\"]}", "One item object test");
        }
    }
}
