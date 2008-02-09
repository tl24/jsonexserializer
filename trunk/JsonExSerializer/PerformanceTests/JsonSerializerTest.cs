using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;
using System.IO;

namespace PerformanceTests
{
    public class JsonSerializerTest : AbstractPerfTestBase
    {
        private Serializer serializer;

        public override void InitSerializer(Type t)
        {
            serializer = Serializer.GetSerializer(t);
        }

        #region ISerializer Members

        public override string FileName
        {
            get { return "test.jsx"; }
        }

        public override string SerializerType
        {
            get { return "JsonExSerializer"; }
        }

        public override void Serialize(object o)
        {
            using (StreamWriter fs = new StreamWriter(FileName, false))
            {
                serializer.Serialize(o, fs);
            }
        }

        public override object Deserialize(Type t)
        {
            using (StreamReader fr = new StreamReader(FileName))
            {
                return serializer.Deserialize(fr);
            }
        }

        #endregion
    }
}
