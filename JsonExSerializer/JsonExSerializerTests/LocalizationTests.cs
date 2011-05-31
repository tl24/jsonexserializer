using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using System.Threading;
using System.Globalization;
using JsonExSerializer;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class LocalizationTests
    {
        [RowTest]
        [Row(120000.324, "120000.324", "en-US", "de-DE")]
        [Row(120000.324, "120000.324", "de-DE", "en-US")]
		[Row(24.00, "24", "en-US", "de-DE")]
		[Row(24.00, "24", "de-DE", "en-US")]
        public void DoublesSerializeCorrectlyInAnyCulture(double value, string expectedValue, string startingCulture, string targetCulture)
        {
            TestSerializationInMultipleLocales(value, expectedValue, startingCulture, targetCulture);
        }

        [RowTest]
        [Row(-0.324f, "-0.324", "en-US", "de-DE")]
        [Row(-1.999f, "-1.999", "de-DE", "en-US")]
        public void FloatsSerializeCorrectlyInAnyCulture(float value, string expectedValue, string startingCulture, string targetCulture)
        {
            TestSerializationInMultipleLocales(value, expectedValue, startingCulture, targetCulture);
        }

        [RowTest]
        [Row(120000.324, "120000.324", "en-US", "de-DE")]
        [Row(120000.324, "120000.324", "de-DE", "en-US")]
        [Row(24.00, "24", "en-US", "de-DE")]
        [Row(24.00, "24", "de-DE", "en-US")]
        public void DecimalsSerializeCorrectlyInAnyCulture(decimal value, string expectedValue, string startingCulture, string targetCulture)
        {
            TestSerializationInMultipleLocales(value, expectedValue, startingCulture, targetCulture);
        }

        [RowTest]
        [Row("en-US", "de-DE", DateTimeKind.Utc)]
        [Row("de-DE", "en-US", DateTimeKind.Utc)]
        [Row("en-US", "de-DE", DateTimeKind.Local)]
        [Row("de-DE", "en-US", DateTimeKind.Local)]
        public void DatesSerializeCorrectlyInAnyCulture(string startingCulture, string targetCulture, DateTimeKind kind)
        {
            DateTime value = new DateTime(2000, 1, 15, 15, 30, 45, kind);
            string expectedValue = "2000-01-15T15:30:45.0000000Z";
            TestSerializationInMultipleLocales(value, "", startingCulture, targetCulture);
        }

        [RowTest]
        [Row(true, "true", "en-US", "de-DE")]
        [Row(false, "false", "de-DE", "en-US")]
        public void BoolSerializeCorrectlyInAnyCulture(bool value, string expectedValue, string startingCulture, string targetCulture)
        {
            // Just in case the string is locale sensitive
            TestSerializationInMultipleLocales(value, expectedValue, startingCulture, targetCulture);
        }

        protected void TestSerializationInMultipleLocales<T>(T sourceValue, string expectedValue, string startingCulture, string targetCulture)
        {
            Exception ex = null;
            ThreadStart func = delegate()
            {
                try
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(startingCulture);
                    Serializer s = new Serializer(typeof(T));
                    s.Config.IsCompact = true;
                    s.Config.OutputTypeComment = false;
                    string result = s.Serialize(sourceValue);
                    if (!string.IsNullOrEmpty(expectedValue))
                        Assert.AreEqual(result, expectedValue);
                    Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(targetCulture);
                    s = new Serializer(typeof(T));
                    T deserializedResult = (T)s.Deserialize(result);
                    Assert.AreEqual(sourceValue, deserializedResult, typeof(T).Name + " deserialized incorrectly in different culture");
                }
                catch (Exception e)
                {
                    ex = e;
                }
            };
            Thread t = new Thread(func);
            t.Start();
            t.Join();
            if (ex != null)
                throw ex;
        }
    }
}
