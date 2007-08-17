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
                .ArrayStart()
                    .Value(true)
                    .Value(1)
                    .QuotedValue("test")
                    .Value(3.14f)
                    .Value(345.8765)
                .ArrayEnd();
            AssertMatch("[True,1,\"test\",3.14,345.8765]", "simple array test");
        }

        [Test]
        public void ArrayOfArraysTest()
        {
            jsonWriter
                .ArrayStart()
                    .Value(1)
                    .ArrayStart()
                        .QuotedValue("one")
                    .ArrayEnd()
                    .Value(2)
                    .ArrayStart()
                        .QuotedValue("two")
                        .Value(3)
                    .ArrayEnd()
                .ArrayEnd();

            AssertMatch("[1,[\"one\"],2,[\"two\",3]]", "Array of arrays test");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValueAfterArrayIsErrorTest()
        {
            jsonWriter
                .ArrayStart()
                    .Value(1)
                .ArrayEnd()
                .Value(true);    // this should cause an error, can only write one value/object/array            
        }

        [Test]
        public void EmptyArray()
        {
            jsonWriter
                .ArrayStart()
                .ArrayEnd();
            AssertMatch("[]", "The empty array");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void KeyInArrayIsErrorTest()
        {
            jsonWriter
                .ArrayStart()
                    .Key("invalidKey")  // key's not allowed within array
                    .QuotedValue("myvalue")
                .ArrayEnd();
        }

        [Test]
        public void EmptyObjectTest()
        {
            jsonWriter
                .ObjectStart()
                .ObjectEnd();
            AssertMatch("{}", "The empty object");
        }

        [Test]
        public void OneItemObjectTest()
        {
            jsonWriter
                .ObjectStart()
                    .Key("KeyText")
                    .QuotedValue("ValueText")
                .ObjectEnd();

            AssertMatch("{\"KeyText\":\"ValueText\"}", "One item object test");
        }

        [Test]
        public void MultipleItemObjectTest()
        {
            jsonWriter
                .ObjectStart()
                    .Key("StringValue")
                    .QuotedValue("ValueText")
                    .Key("LongVal")
                    .Value(1234)
                    .Key("BoolVal")
                    .Value(true)
                .ObjectEnd();

            AssertMatch("{\"StringValue\":\"ValueText\",\"LongVal\":1234,\"BoolVal\":True}", "");
        }

        [Test]
        public void ObjectOfObjectsTest()
        {
            jsonWriter
                .ObjectStart()
                    .Key("StringValue")
                    .QuotedValue("ValueText")
                    .Key("LongVal")
                    .Value(1234)
                    .Key("BoolVal")
                    .Value(true)
                    .Key("ComplexObject")
                    .ObjectStart()
                        .Key("KeyText")
                        .QuotedValue("ValueText")
                    .ObjectEnd()
                .ObjectEnd();
            AssertMatch("{\"StringValue\":\"ValueText\",\"LongVal\":1234,\"BoolVal\":True,\"ComplexObject\":{\"KeyText\":\"ValueText\"}}", "");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValueAfterObjectIsErrorTest()
        {
            jsonWriter
                .ObjectStart()
                    .Key("test")
                    .Value(1)
                .ObjectEnd()
                .Value(true);    // this should cause an error, can only write one value/object/array            
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MissingKeyObjectTest()
        {
            jsonWriter
                .ObjectStart()
                    // key is missing
                    .Value(123)
                .ObjectEnd();
        }

        [Test]
        public void ObjectInArrayTest()
        {
            jsonWriter
                .ArrayStart()
                    .Value(true)
                    .Value(1)
                    .QuotedValue("test")
                    .Value(3.14f)
                    .Value(345.8765)
                    .ObjectStart()
                        .Key("KeyText")
                        .QuotedValue("ValueText")
                    .ObjectEnd()
                .ArrayEnd();
            AssertMatch("[True,1,\"test\",3.14,345.8765,{\"KeyText\":\"ValueText\"}]", "object inside array test");
        }

        [Test]
        public void ArrayInObjectTest()
        {
            jsonWriter
                .ObjectStart()
                    .Key("FirstArray")
                    .ArrayStart()
                        .Value(123)
                        .Value(true)
                    .ArrayEnd()
                    .Key("KeyText")
                    .QuotedValue("ValueText")
                    .Key("LastArray")
                    .ArrayStart()
                        .QuotedValue("last")
                    .ArrayEnd()
                .ObjectEnd();

            AssertMatch("{\"FirstArray\":[123,True],\"KeyText\":\"ValueText\",\"LastArray\":[\"last\"]}", "One item object test");
        }
    }
}
