using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;

namespace JsonExSerializerTests
{
    public class NestedFieldTest
    {

        public void TestSerializeNestedField()
        {
            Serializer ser = new Serializer(typeof(Msg));
            Console.WriteLine(ser.Serialize(new Msg()));
        }

        public class Hdr
        {
            [JsonExProperty]
            public string A { get { return "A"; } }
        }

        public class Msg
        {
            public Hdr Header = new Hdr();
            public string Body { get { return "Message"; } }
            public Msg() { }
        }
    }
}
