using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;
using JsonExSerializerTests.Mocks;
using MbUnit.Framework;
using System.Diagnostics;
using System.Globalization;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class SerializePrimitiveTests
    {

        [Row(32L)]
        [Row(0L)]
        [Row(-23L)]
        [Row(long.MaxValue)]
        [Row(long.MinValue)]
        [RowTest]
        public void SerializeLongTest(long expected)
        {
            Serializer s = new Serializer(typeof(long));
            s.Config.SetJsonStrictOptions();
            string result = s.Serialize(expected);
            Assert.AreEqual(expected.ToString(CultureInfo.InvariantCulture), result.Trim(), "long did not serialize correctly");
            long actual = (long)s.Deserialize(result);
            Assert.AreEqual(expected, actual, "long did not deserialize correctly");
        }


        [Row(32)]
        [Row(0)]
        [Row(-23)]
        [Row(int.MaxValue)]
        [Row(int.MinValue)]
        [RowTest]
        public void SerializeIntTest(int expected)
        {
            Serializer s = new Serializer(typeof(int));
            s.Config.SetJsonStrictOptions();
            string result = s.Serialize(expected);
            Assert.AreEqual(expected.ToString(CultureInfo.InvariantCulture), result.Trim(), "Int did not serialize correctly");
        }

        [Row(32u)]
        [Row(0u)]
        [Row(uint.MaxValue)]
        [Row(uint.MinValue)]
        [RowTest]
        public void SerializeUIntTest(uint expected)
        {
            Serializer s = new Serializer(typeof(uint));
            s.Config.SetJsonStrictOptions();
            string result = s.Serialize(expected);
            Assert.AreEqual(expected.ToString(CultureInfo.InvariantCulture), result.Trim(), "UInt did not serialize correctly");
        }

        [Row(32)]
        [Row(0)]
        [Row(-23)]
        [Row(short.MaxValue)]
        [Row(short.MinValue)]
        [RowTest]
        public void SerializeShortTest(short expected)
        {
            Serializer s = new Serializer(typeof(short));
            s.Config.SetJsonStrictOptions();
            string result = s.Serialize(expected);
            Assert.AreEqual(expected.ToString(CultureInfo.InvariantCulture), result.Trim(), "Short did not serialize correctly");
        }

        [Row(true, "true")]
        [Row(false, "false")]
        [RowTest]
        public void SerializeBoolTest(bool Value, string Expected)
        {
            Serializer s = new Serializer(typeof(bool));
            s.Config.SetJsonStrictOptions();
            string result = s.Serialize(Value);
            Assert.AreEqual(Expected, result.Trim(), "Bool did not serialize correctly");
        }

        [Row(32.34f)]
        [Row(0.0f)]
        [Row(-23.234f)]
        [Row(float.MaxValue)]
        [Row(float.MinValue)]
        [Row(float.NaN)]
        [Row(float.PositiveInfinity)]
        /*[Row(float.NegativeInfinity)]*/
        [RowTest]
        public void SerializeFloatTest(float expected)
        {
            Serializer s = new Serializer(typeof(float));
            s.Config.SetJsonStrictOptions();
            float actual;
            string result = s.Serialize(expected);
            Assert.AreEqual(expected, float.Parse(result, CultureInfo.InvariantCulture), "float did not serialize correctly");
            actual = (float)s.Deserialize(result);
            Assert.AreEqual(expected, actual, "float did not deserialize correctly");
        }

        [Row(32.34)]
        [Row(0.0)]
        [Row(-23.234)]
        [Row(double.MaxValue)]
        [Row(double.MinValue)]
        [Row(double.NaN)]
        [Row(double.PositiveInfinity)]
        /*[Row(double.NegativeInfinity)]*/
        [RowTest]
        public void SerializeDoubleTest(double expected)
        {
            Serializer s = new Serializer(typeof(double));
            s.Config.SetJsonStrictOptions();
            string result = s.Serialize(expected);
            Assert.AreEqual(expected, double.Parse(result, CultureInfo.InvariantCulture), "double did not serialize correctly");
            double actual = (double)s.Deserialize(result);
            Assert.AreEqual(expected, actual, "double did not deserialize correctly");
        }

        [Row(0xff)]
        [Row(0x0)]
        [Row(0x1)]
        [RowTest]
        public void SerializeByteTest(byte expected)
        {
            Serializer s = new Serializer(typeof(byte));
            s.Config.SetJsonStrictOptions();
            string result = s.Serialize(expected);
            Assert.AreEqual(expected, byte.Parse(result, CultureInfo.InvariantCulture), "byte did not serialize correctly");
            byte actual = (byte)s.Deserialize(result);
            Assert.AreEqual(expected, actual, "byte did not deserialize correctly");
        }

        [Test]
        public void SerializeStringTest()
        {
            Serializer s = new Serializer(typeof(string));
            s.Config.SetJsonStrictOptions();
            string expected = "simple";
            string result = s.Serialize(expected);
            Assert.AreEqual("\"simple\"", result, "String did not serialize correctly.");
        }

        [Test]
        public void SerializeSpecialCharacterStringTest()
        {
            Serializer s = new Serializer(typeof(string));
            string expected = "Are you sure? (y\\n)\r\n";
            string result = s.Serialize(expected);
            string actual = (string) s.Deserialize(result);
            Assert.AreEqual(expected, actual, "String with special characters did not serialize correctly.");
        }

        [Test]
        public void SerializeEnumTest()
        {
            Serializer s = new Serializer(typeof(SimpleEnum));
            string result = s.Serialize(SimpleEnum.EnumValue2);
            SimpleEnum se = (SimpleEnum)s.Deserialize(result);
            Assert.AreEqual(SimpleEnum.EnumValue2, se, "Enum values not correct");
        }

        [Test]
        public void SerializeFlagsEnumTest()
        {
            Serializer s = new Serializer(typeof(MockFlagsEnum));
            MockFlagsEnum expected = MockFlagsEnum.BitOne | MockFlagsEnum.BitFour | MockFlagsEnum.BitFive;
            string result = s.Serialize(expected);
            MockFlagsEnum actual = (MockFlagsEnum)s.Deserialize(result);
            Assert.AreEqual(expected, actual, "Flags Enum values not correct");
        }

        [Test]
        public void TestCastAlias()
        {
            Serializer s = new Serializer(typeof(object));
            int expected = 3456;
            string result = s.Serialize(expected);
            int actual = (int)s.Deserialize(result);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DeserializeEmptyString()
        {
            Serializer s = new Serializer(typeof(object));
            object value = s.Deserialize("");
            Assert.IsNull(value, "Deserialize empty string should be null");
        }

        [Test]
        public void SerializeDateTest()
        {
            TestDate(new DateTime(1970, 1, 1));
            TestDate(new DateTime(2000,1,1,0,0,0));
            TestDate(new DateTime(2010, 2, 28, 23, 23, 23));
            TestDate(new DateTime(2004, 2, 29, 15, 59, 59));
        }

        public void TestDate(DateTime expected)
        {
            Serializer s = new Serializer(typeof(DateTime));
            s.Config.SetJsonStrictOptions();
            string result = s.Serialize(expected);
            DateTime actual = (DateTime)s.Deserialize(result);
            Assert.AreEqual(expected, actual, "DateTime did not deserialize correctly");
        }

        [Row("1.10")]
        [Row("0.1")]
        [Row("79228162514264337593543950335")] // Max value
        [Row("-79228162514264337593543950335")] // Min value
        [Row("0")]
        [RowTest]
        public void SerializeDecimalTest(string decimalString) {
            Debug.WriteLine(decimal.MinValue.ToString(CultureInfo.InvariantCulture));
            decimal expected = decimal.Parse(decimalString, CultureInfo.InvariantCulture);
            Serializer s = new Serializer(typeof(decimal));
            s.Config.SetJsonStrictOptions();
            string result = s.Serialize(expected);
            Assert.AreEqual(expected, decimal.Parse(result, CultureInfo.InvariantCulture), "decimal did not serialize correctly");
            decimal actual = (decimal)s.Deserialize(result);
            Assert.AreEqual(expected, actual, "decimal did not deserialize correctly");
        }

        [Test]
        public void SerializeStringEscapesTest()
        {
            Serializer s = new Serializer(typeof(string));
            string data = "\u0000\u0001\u0002\r\n\t\f\b\"/\\";
            string result = s.Serialize(data);
            string actual = (string)s.Deserialize(result);
            Assert.AreEqual(data, actual, "Escaped strings don't match");
        }

        [Row("00000000-0000-0000-0000-000000000000", false, "\"{0}\"")]
        [Row("94208f5f-bc84-414c-b6c6-36d8b701f1ee", false, "\"{0}\"")]
        [Row(null, false, "null")]
        [Row("00000000-0000-0000-0000-000000000000", true, "(System.Guid)\"{0}\"")]
        [Row("94208f5f-bc84-414c-b6c6-36d8b701f1ee", true, "(System.Guid)\"{0}\"")]
        [Row(null, true, "null")]
        [RowTest]
        public void NullableGuid(string guidString, bool useObjectType, string resultFormat)
        {
            Guid? value;
            if (guidString == null)
                value = null;
            else
                value = new Guid(guidString);

            Serializer s;
            if (useObjectType)
                s = new Serializer(typeof(object));
            else
                s = new Serializer(typeof(Guid?));
            s.Config.IsCompact = true;
            s.Config.OutputTypeComment = false;
            string result = s.Serialize(value);
            Assert.AreEqual(string.Format(resultFormat, guidString), result, "Expected serialization output");
            Guid? r = (Guid?)s.Deserialize(result);
            Assert.AreEqual(value, r);
        }
    }
}
