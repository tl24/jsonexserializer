using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using JsonExSerializer;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class DeserializePrimitiveTests
    {

        [Test]
        public void DeserializeIntTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(int));
            int value = 32;
            int result = (int)s.Deserialize(value.ToString());
            Assert.AreEqual(value, result, "Int not deserialized correctly");

            result = (int)s.Deserialize("  " + value.ToString() + "  ");
            Assert.AreEqual(value, result, "Int not deserialized correctly with whitespace");

            value = -44;
            result = (int)s.Deserialize("  " + value.ToString() + "  ");
            Assert.AreEqual(value, result, "Negative Int not deserialized correctly with whitespace");

        }

        [Test]
        public void DeserializeLongTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(long));
            long value = 32;
            long result = (long)s.Deserialize(value.ToString());
            Assert.AreEqual(value, result, "long not deserialized correctly");

            result = (long)s.Deserialize("  " + value.ToString() + "  ");
            Assert.AreEqual(value, result, "long not deserialized correctly with whitespace");

            value = -44;
            result = (long)s.Deserialize("  " + value.ToString() + "  ");
            Assert.AreEqual(value, result, "Negative long not deserialized correctly with whitespace");

        }

        [Test]
        public void DeserializeShortTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(short));
            short value = 32;
            short result = (short)s.Deserialize(value.ToString());
            Assert.AreEqual(value, result, "short not deserialized correctly");

            result = (short)s.Deserialize("  " + value.ToString() + "  ");
            Assert.AreEqual(value, result, "short not deserialized correctly with whitespace");

            value = -44;
            result = (short)s.Deserialize("  " + value.ToString() + "  ");
            Assert.AreEqual(value, result, "Negative short not deserialized correctly with whitespace");

        }

        [Test]
        public void DeserializeFloatTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(float));
            float value = 32.44f;
            float result = (float)s.Deserialize(value.ToString());
            Assert.AreEqual(value, result, "float not deserialized correctly");

            result = (float)s.Deserialize("  " + value.ToString() + "  ");
            Assert.AreEqual(value, result, "float not deserialized correctly with whitespace");

            value = 0.0f;
            result = (float)s.Deserialize(value.ToString());
            Assert.AreEqual(value, result, "float zero not deserialized correctly");

            value = -44.56f;
            result = (float)s.Deserialize("  " + value.ToString() + "  ");
            Assert.AreEqual(value, result, "Negative float not deserialized correctly with whitespace");

            value = float.MaxValue;
            result = (float)s.Deserialize(value.ToString("R"));
            Assert.AreEqual(value, result, "Error deserializing float max value");
        }

        [Test]
        public void DeserializedoubleTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(double));
            double value = 32.44;
            double result = (double)s.Deserialize(value.ToString());
            Assert.AreEqual(value, result, "double not deserialized correctly");

            result = (double)s.Deserialize("  " + value.ToString() + "  ");
            Assert.AreEqual(value, result, "double not deserialized correctly with whitespace");

            value = 0.0;
            result = (double)s.Deserialize(value.ToString());
            Assert.AreEqual(value, result, "double zero not deserialized correctly");

            value = -44.56;
            result = (double)s.Deserialize("  " + value.ToString() + "  ");
            Assert.AreEqual(value, result, "Negative double not deserialized correctly with whitespace");

        }

        [Test]
        public void DeserializeBoolTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(bool));
            bool value = true;
            bool result = (bool)s.Deserialize(value.ToString());
            Assert.AreEqual(value, result, "Bool true not deserialized correctly");

            result = (bool)s.Deserialize("  " + value.ToString() + "  ");
            Assert.AreEqual(value, result, "Bool true not deserialized correctly with whitespace");

            value = false;
            result = (bool)s.Deserialize("  " + value.ToString() + "  ");
            Assert.AreEqual(value, result, "Bool false not deserialized correctly with whitespace");
        }

        [Test]
        public void DeserializeStringTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(string));
            string value = "test";
            string result = (string)s.Deserialize("\"" + value + "\"");
            Assert.AreEqual(value, result, "Simple double quote string did not deserialize correctly");

            value = "I say \"here, here\".";
            result = (string)s.Deserialize("\"" + value.Replace("\"", "\\\"") + "\"");
            Assert.AreEqual(value, result, "Embedded quotes double quote string did not deserialize correctly");
        }
    }
}
