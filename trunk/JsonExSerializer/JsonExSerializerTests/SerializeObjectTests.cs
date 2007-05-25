using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using JsonExSerializer;
using JsonExSerializerTests.Mocks;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class SerializeObjectTests
    {

        [Test]
        public void SimpleObjectTest()
        {
            SimpleObject src = new SimpleObject();
            src.BoolValue = true;
            src.ByteValue = 0x89;
            src.CharValue = 'h';
            src.DoubleValue = double.MaxValue;
            src.FloatValue = float.MaxValue;
            src.IntValue = int.MaxValue;
            src.LongValue = int.MinValue;
            src.ShortValue = short.MaxValue;
            src.StringValue = "The simple string";

            Serializer s = Serializer.GetSerializer(typeof(SimpleObject));
            string result = s.Serialize(src);
            SimpleObject dst = (SimpleObject) s.Deserialize(result);
            ValidateSimpleObjects(src, dst);
        }

        public void SemiComplexObjectTest()
        {
            SimpleObject src = new SimpleObject();
            src.BoolValue = true;
            src.ByteValue = 0x99;
            src.CharValue = '\n';
            src.DoubleValue = 1234.4567;
            src.FloatValue = float.MinValue;
            src.IntValue = int.MinValue;
            src.LongValue = int.MaxValue;
            src.ShortValue = short.MinValue;
            src.StringValue = "The\nmultiline\nstring";
            SemiComplexObject complex = new SemiComplexObject();
            complex.SimpleObject = src;
            complex.Name = "MyComplexObject";

            Serializer s = Serializer.GetSerializer(typeof(SemiComplexObject));
            string result = s.Serialize(complex);
            SemiComplexObject complexDest = (SemiComplexObject) s.Deserialize(result);
            Assert.AreEqual(complex.Name, complexDest.Name, "SemiComplex Name not deserialized correctly");
            ValidateSimpleObjects(complex.SimpleObject, complexDest.SimpleObject);            
        }

        public void ValidateSimpleObjects(SimpleObject src, SimpleObject dst)
        {
            Assert.AreEqual(src.BoolValue, dst.BoolValue, "SimpleObject.BoolValue not equal");
            Assert.AreEqual(src.ByteValue, dst.ByteValue, "SimpleObject.ByteValue not equal");
            Assert.AreEqual(src.CharValue, dst.CharValue, "SimpleObject.CharValue not equal");
            Assert.AreEqual(src.DoubleValue, dst.DoubleValue, "SimpleObject.DoubleValue not equal");
            Assert.AreEqual(src.FloatValue, dst.FloatValue, "SimpleObject.FloatValue not equal");
            Assert.AreEqual(src.IntValue, dst.IntValue, "SimpleObject.IntValue not equal");
            Assert.AreEqual(src.LongValue, dst.LongValue, "SimpleObject.LongValue not equal");
            Assert.AreEqual(src.ShortValue, dst.ShortValue, "SimpleObject.ShortValue not equal");
            Assert.AreEqual(src.StringValue, dst.StringValue, "SimpleObject.StringValue not equal");
        }
    }
}
