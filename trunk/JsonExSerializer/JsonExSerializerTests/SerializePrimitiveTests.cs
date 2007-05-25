using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using JsonExSerializer;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class SerializePrimitiveTests
    {

        [Test]
        public void SerializeLongTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(long));
            long val = 32;
            string result = s.Serialize(val);
            Assert.AreEqual(val.ToString(), result.Trim(), "long Positive did not serialize correctly");

            val = 0;
            result = s.Serialize(val);
            Assert.AreEqual(val.ToString(), result.Trim(), "long Zero did not serialize correctly");

            val = -23;
            result = s.Serialize(val);
            Assert.AreEqual(val.ToString(), result.Trim(), "long Negative did not serialize correctly");

        }


        [Test]
        public void SerializeIntTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(int));
            int val = 32;
            string result = s.Serialize(val);
            Assert.AreEqual(val.ToString(), result.Trim(), "Int Positive did not serialize correctly");

            val = 0;
            result = s.Serialize(val);
            Assert.AreEqual(val.ToString(), result.Trim(), "Int Zero did not serialize correctly");

            val = -23;
            result = s.Serialize(val);
            Assert.AreEqual(val.ToString(), result.Trim(), "Int Negative did not serialize correctly");

        }

        [Test]
        public void SerializeShortTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(short));
            short val = 32;
            string result = s.Serialize(val);
            Assert.AreEqual(val.ToString(), result.Trim(), "Short Positive did not serialize correctly");

            val = 0;
            result = s.Serialize(val);
            Assert.AreEqual(val.ToString(), result.Trim(), "Short Zero did not serialize correctly");

            val = -23;
            result = s.Serialize(val);
            Assert.AreEqual(val.ToString(), result.Trim(), "Short Negative did not serialize correctly");

        }

        [Test]
        public void SerializeBoolTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(bool));
            bool val = true;
            string result = s.Serialize(val);
            Assert.AreEqual(val.ToString(), result.Trim(), "Bool true did not serialize correctly");

            val = false;

            result = s.Serialize(val);
            Assert.AreEqual(val.ToString(), result.Trim(), "Bool false did not serialize correctly");
        }

        [Test]
        public void SerializeFloatTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(short));
            float val = 32.34f;
            string result = s.Serialize(val);
            Assert.AreEqual(val, float.Parse(result), "float Positive did not serialize correctly");

            val = 0.0f;
            result = s.Serialize(val);
            Assert.AreEqual(val, float.Parse(result), "float Zero did not serialize correctly");

            val = -23.234f;
            result = s.Serialize(val);
            Assert.AreEqual(val, float.Parse(result), "float Negative did not serialize correctly");

        }

        [Test]
        public void SerializeDoubleTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(double));
            double val = 32.34;
            string result = s.Serialize(val);
            Assert.AreEqual(val, double.Parse(result), "double Positive did not serialize correctly");

            val = 0.0;
            result = s.Serialize(val);
            Assert.AreEqual(val, double.Parse(result), "double Zero did not serialize correctly");

            val = -23.234;
            result = s.Serialize(val);
            Assert.AreEqual(val, double.Parse(result), "double Negative did not serialize correctly");
        }

        [Test]
        public void SerializeByteTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(byte));
            byte val = 0xff;
            string result = s.Serialize(val);
            Assert.AreEqual(val, byte.Parse(result), "byte Positive did not serialize correctly");

            val = 0x0;
            result = s.Serialize(val);
            Assert.AreEqual(val, byte.Parse(result), "byte Zero did not serialize correctly");


            val = 0x1;
            result = s.Serialize(val);
            Assert.AreEqual(val, byte.Parse(result), "byte 1 did not serialize correctly");
        }

        [Test]
        public void SerializeStringTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(string));
            string val = "simple";
            string result = s.Serialize(val);
            Assert.AreEqual("\"simple\"", result, "String did not serialize correctly.");

        }
    }
}
