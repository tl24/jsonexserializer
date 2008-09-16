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


        public JsonSerializerTest(int ObjectCount, int Iterations)
            : base(ObjectCount, Iterations)
        {
        }

        public override void InitSerializer(Type t)
        {            
            serializer = new Serializer(t);
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

    public class JsonDynamicTests : JsonSerializerTest
    {

        public JsonDynamicTests(int ObjectCount, int Iterations)
            : base(ObjectCount, Iterations)
        {
        }

        public override void InitSerializer(Type t)
        {
            base.InitSerializer(t);
            serializer.Context.TypeHandlerFactory = new CustomTypeHandlerFactory(typeof(DynamicTypeHandler), serializer.Context);
        }
    }
}
