using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;
using System.IO;

namespace JsonExSerializerTests.Mocks
{
    public class JsonWriterMock : JsonWriter
    {
        public JsonWriterMock(TextWriter writer, bool indent)
            : base(writer, indent)
        {
        }

        protected override void WriteTypeInfo(Type t)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override IJsonWriter WriteObject(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
