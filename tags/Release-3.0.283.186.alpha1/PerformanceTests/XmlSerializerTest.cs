using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace PerformanceTests
{
    public class XmlSerializerTest : AbstractPerfTestBase
    {
        private XmlSerializer serializer;

        public XmlSerializerTest(int ObjectCount, int Iterations)
            : base(ObjectCount, Iterations)
        {
        }

        public override void InitSerializer(Type t)
        {
            serializer = new XmlSerializer(t);
        }

        public override void Serialize(object o)
        {
            using (StreamWriter sw = new StreamWriter(FileName))
            {
                serializer.Serialize(sw, o);
            }
        }

        public override object Deserialize(Type t)
        {
            using (StreamReader sr = new StreamReader(FileName))
            {
                return serializer.Deserialize(sr);
            }
        }

        public override string FileName
        {
            get { return "xmltest.xml"; }
        }

        public override string SerializerType
        {
            get { return "XmlSerializer"; }
        }
    }
}
