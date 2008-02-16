using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializerTests.Mocks;
using JsonExSerializer;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class SerializeCtorTests
    {

        [Test]
        public void SimpleConstructorNoInitTest()
        {
            MyPointConstructor pt = new MyPointConstructor(3, 9);
            Serializer s = new Serializer(pt.GetType());
            string result = s.Serialize(pt);
            MyPointConstructor actual = (MyPointConstructor)s.Deserialize(result);
            Assert.AreEqual(pt, actual, "Simple Constructor with no initializer failed");
        }
    }
}
