using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using JsonExSerializer;
using System.Collections;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class DeserializeCollectionsTests
    {
        [Test]
        public void StringCollectionTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(ArrayList));
            string str = "[ \"one\", \"two\", \"three\" ]";
            object result = s.Deserialize(str);
        }
    }
}
