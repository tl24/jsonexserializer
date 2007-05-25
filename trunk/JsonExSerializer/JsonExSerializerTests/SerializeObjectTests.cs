using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using JsonExSerializer;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class SerializeObjectTests
    {

        [Test]
        public void SimpleObjectTest()
        {
            SimpleObject so = new SimpleObject();
            so.BoolValue = true;
            so.ByteValue = 0x89;
            so.CharValue = 'h';
            so.DoubleValue = double.MaxValue;
            so.FloatValue = float.MaxValue;
            so.IntValue = int.MaxValue;
            so.LongValue = int.MinValue;
            so.ShortValue = short.MaxValue;
            so.StringValue = "The simple string";

            Serializer s = Serializer.GetSerializer(typeof(SimpleObject));
            string result = s.Serialize(so);
            SimpleObject o2 = (SimpleObject) s.Deserialize(result);
        }
    }
}
