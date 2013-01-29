using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class DeserializePrimitiveTests
    {

        [Test]
        public void DeserializeIntTest()
        {
            Serializer s = new Serializer();
            int value = 32;
            int result = s.Deserialize<int>(value.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(value, result, "Int not deserialized correctly");

            result = s.Deserialize<int>("  " + value.ToString(CultureInfo.InvariantCulture) + "  ");
            Assert.AreEqual(value, result, "Int not deserialized correctly with whitespace");

            value = -44;
            result = s.Deserialize<int>("  " + value.ToString(CultureInfo.InvariantCulture) + "  ");
            Assert.AreEqual(value, result, "Negative Int not deserialized correctly with whitespace");

        }

        [Test]
        public void DeserializeLongTest()
        {
            Serializer s = new Serializer();
            long value = 32;
            long result = s.Deserialize<long>(value.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(value, result, "long not deserialized correctly");

            result = s.Deserialize<long>("  " + value.ToString(CultureInfo.InvariantCulture) + "  ");
            Assert.AreEqual(value, result, "long not deserialized correctly with whitespace");

            value = -44;
            result = s.Deserialize<long>("  " + value.ToString(CultureInfo.InvariantCulture) + "  ");
            Assert.AreEqual(value, result, "Negative long not deserialized correctly with whitespace");

        }

        [Test]
        public void DeserializeShortTest()
        {
            Serializer s = new Serializer();
            short value = 32;
            short result = s.Deserialize<short>(value.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(value, result, "short not deserialized correctly");

            result = s.Deserialize<short>("  " + value.ToString(CultureInfo.InvariantCulture) + "  ");
            Assert.AreEqual(value, result, "short not deserialized correctly with whitespace");

            value = -44;
            result = s.Deserialize<short>("  " + value.ToString(CultureInfo.InvariantCulture) + "  ");
            Assert.AreEqual(value, result, "Negative short not deserialized correctly with whitespace");

        }

        [Test]
        public void DeserializeFloatTest()
        {
            Serializer s = new Serializer();
            float value = 32.44f;
            float result = s.Deserialize<float>(value.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(value, result, "float not deserialized correctly");

            result = s.Deserialize<float>("  " + value.ToString(CultureInfo.InvariantCulture) + "  ");
            Assert.AreEqual(value, result, "float not deserialized correctly with whitespace");

            value = 0.0f;
            result = s.Deserialize<float>(value.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(value, result, "float zero not deserialized correctly");

            value = -44.56f;
            result = s.Deserialize<float>("  " + value.ToString(CultureInfo.InvariantCulture) + "  ");
            Assert.AreEqual(value, result, "Negative float not deserialized correctly with whitespace");

            value = float.MaxValue;
            result = s.Deserialize<float>(value.ToString("R", CultureInfo.InvariantCulture));
            Assert.AreEqual(value, result, "Error deserializing float max value");
        }

        [Test]
        public void DeserializedoubleTest()
        {
            Serializer s = new Serializer();
            double value = 32.44;
            double result = s.Deserialize<double>(value.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(value, result, "double not deserialized correctly");

            result = s.Deserialize<double>("  " + value.ToString(CultureInfo.InvariantCulture) + "  ");
            Assert.AreEqual(value, result, "double not deserialized correctly with whitespace");

            value = 0.0;
            result = s.Deserialize<double>(value.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(value, result, "double zero not deserialized correctly");

            value = -44.56;
            result = s.Deserialize<double>("  " + value.ToString(CultureInfo.InvariantCulture) + "  ");
            Assert.AreEqual(value, result, "Negative double not deserialized correctly with whitespace");

        }

        [Test]
        public void DeserializeBoolTest()
        {
            Serializer s = new Serializer();
            bool value = true;
            bool result = s.Deserialize<bool>(value.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(value, result, "Bool true not deserialized correctly");

            result = s.Deserialize<bool>("  " + value.ToString(CultureInfo.InvariantCulture) + "  ");
            Assert.AreEqual(value, result, "Bool true not deserialized correctly with whitespace");

            value = false;
            result = s.Deserialize<bool>("  " + value.ToString(CultureInfo.InvariantCulture) + "  ");
            Assert.AreEqual(value, result, "Bool false not deserialized correctly with whitespace");
        }

        [Test]
        public void DeserializeStringTest()
        {
            Serializer s = new Serializer();
            string value = "test";
            string result = s.Deserialize<string>("\"" + value + "\"");
            Assert.AreEqual(value, result, "Simple double quote string did not deserialize correctly");

            value = "I say \"here, here\".";
            result = s.Deserialize<string>("\"" + value.Replace("\"", "\\\"") + "\"");
            Assert.AreEqual(value, result, "Embedded quotes double quote string did not deserialize correctly");
        }

        [Test]
        public void UnicodeEscapeTest()
        {
            Serializer s = new Serializer();
            string expected = "\u0164\u0112\u0161\u0164";
            string toDeserialize = @"""\u0164\u0112\u0161\u0164""";
            string result = s.Deserialize<string>(toDeserialize);
            Assert.AreEqual(expected, result, "Unicode escaped string did not deserialize correctly");


        }
    }
}
