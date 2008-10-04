using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using System.IO;
using JsonExSerializer;
using System.Text.RegularExpressions;
using JsonExSerializerTests.Mocks;
using JsonExSerializer.Framework;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class JsonWriterTestBase
    {
        /// <summary>
        /// Results will be written here for each test
        /// </summary>
        protected StringWriter stringWriter;

        /// <summary>
        /// This will be the instance used to test
        /// </summary>
        protected IJsonWriter jsonWriter;

        [SetUp]
        public virtual void Setup()
        {
            stringWriter = new StringWriter();
            jsonWriter = new JsonWriter(stringWriter, true);
        }

        [TearDown]
        public virtual void TearDown()
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
        public virtual void AssertMatch(string textToMatch, string description)
        {
            // remove whitespace
            //string result = Regex.Replace(stringWriter.ToString(), @"\s*", "");
            string result = Regex.Replace(stringWriter.ToString(), @"(?<!new)\s*", "");
            
            Assert.AreEqual(result, textToMatch, description);
        }
    }
}
