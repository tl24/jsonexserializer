using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace PerformanceTests
{
    public class BinarySerializerTest : AbstractPerfTestBase
    {
        BinaryFormatter binFormatter;

        public BinarySerializerTest(PerfTestOptions options)
            : base(options)
        {
        }

        public override void InitSerializer(Type t)
        {
            binFormatter = new BinaryFormatter();
            
        }
        public override void Serialize(object o)
        {
            using (FileStream fs = new FileStream(FileName, FileMode.Create))
            {
                binFormatter.Serialize(fs, o);
            }
        }

        public override object Deserialize(Type t)
        {
            using (FileStream fs = new FileStream(FileName, FileMode.Open))
            {
                return binFormatter.Deserialize(fs);
            }
        }

        public override string FileName
        {
            get { return "test.bin"; }
        }

        public override string SerializerType
        {
            get { return "BinarySerializer"; }
        }

    }
}
