using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using System.IO;
using JsonExSerializer;
using System.Text.RegularExpressions;
using JsonExSerializerTests.Mocks;
using System.Diagnostics;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class JsonWriterTests
    {
        /// <summary>
        /// Results will be written here for each test
        /// </summary>
        protected StringWriter stringWriter;

        /// <summary>
        /// This will be the instance used to test
        /// </summary>
        protected JsonWriter jsonWriter;

        [SetUp]
        public void Setup()
        {
            stringWriter = new StringWriter();
            jsonWriter = new JsonWriterMock(stringWriter, true);
        }

        [TearDown]
        public void TearDown()
        {
            stringWriter.Dispose();
            jsonWriter.Dispose();
            stringWriter = null;
            jsonWriter = null;
        }

        /// <summary>
        /// Asserts that the string matches what was written to the stringWriter without
        /// regard to whitespace.
        /// </summary>
        /// <param name="textToMatch">the text to match against</param>
        /// <param name="description">error description</param>
        public void AssertMatch(string textToMatch, string description)
        {
            // remove whitespace
            string result = Regex.Replace(stringWriter.ToString(), @"\s*", "");
            Assert.AreEqual(result, textToMatch, description);
        }

        [RowTest]
        [Row(true, "True")]
        [Row(false,"False")]
        public void TestBoolValue(bool bVal, string expected)
        {
            jsonWriter.Value(bVal);
            AssertMatch(expected, "bool value written incorrectly");
        }

        [RowTest]
        [Row((byte) 255, "255")]
        [Row(0, "0")]
        [Row(long.MinValue, "-9223372036854775808")]
        [Row(long.MaxValue, "9223372036854775807")]
        [Row((short) 122, "122")]
        public void TestLongValue(long lVal, string expected)
        {
            jsonWriter.Value(lVal);
            AssertMatch(expected, "long value written incorrectly");
        }

        [RowTest]
        [Row(float.MinValue, "-3.40282347E+38")]
        [Row(3.14727f, "3.14727")]
        [Row(float.PositiveInfinity, "Infinity")]
        [Row(float.NegativeInfinity, "-Infinity")]
        [Row(0, "0")]
        public void TestFloatValue(float fVal, string expected)
        {
            jsonWriter.Value(fVal);
            AssertMatch(expected, "float value written incorrectly");
        }

        [RowTest]
        [Row(double.MinValue, "-1.7976931348623157E+308")]
        [Row(3.14727, "3.14727")]
        [Row(-1.999999,"-1.999999")]
        [Row(double.PositiveInfinity, "Infinity")]
        [Row(double.NegativeInfinity, "-Infinity")]
        [Row(0, "0")]
        public void TestDoubleValue(double dVal, string expected)
        {
            jsonWriter.Value(dVal);
            AssertMatch(expected, "double value written incorrectly");
        }

        [RowTest]
        [Row("simple", "\"simple\"")]
        [Row("white space", "\"white space\"")]
        [Row("embedded \"double quotes\"", "\"embedded \\\"double quotes\\\"\"")]
        [Row("embedded 'single quotes'", "\"embedded 'single quotes'\"")]
        public void TestQuotedValue(string sVal, string expected)
        {
            jsonWriter.QuotedValue(sVal);
            string actual = stringWriter.ToString().Trim();
            Assert.AreEqual(expected, actual, "string value written incorrectly");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestWriteValueTwiceFails()
        {
            jsonWriter.Value(1).Value(2);
        }
    }
}
