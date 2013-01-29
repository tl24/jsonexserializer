using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;
using System.IO;
using JsonExSerializer.MetaData;
using PerformanceTests.TestDomain;

namespace PerformanceTests
{
    public class JsonSerializerTest : AbstractPerfTestBase
    {
        protected Serializer serializer;
        protected Type serializedType;

        public JsonSerializerTest(PerfTestOptions options)
            : base(options)
        {
        }

        public override void InitSerializer(Type t)
        {
            serializedType = t;
            serializer = new Serializer();
            //serializer.Context.TypeHandlerFactory = new CustTypeHandlerFactory(serializer.Context);
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
                serializer.Serialize(o, fs, serializedType);
            }
        }

        public override object Deserialize(Type t)
        {
            using (StreamReader fr = new StreamReader(FileName))
            {
                return serializer.Deserialize(fr, t);
            }
        }

        #endregion
    }

    public class JsonDynamicTests : JsonSerializerTest
    {

        public JsonDynamicTests(PerfTestOptions options)
            : base(options)
        {
        }

        public override void InitSerializer(Type t)
        {
            base.InitSerializer(t);
            serializer.Settings.TypeHandlerFactory = new CustomTypeDataRepository(typeof(DynamicTypeData), serializer.Settings);
        }
    }
}
