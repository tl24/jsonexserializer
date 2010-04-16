using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer.Framework.Expressions;

namespace JsonExSerializerTests.Expressions
{
    [TestFixture]
    public class JsonPathTests
    {
        [Test]
        public void TestRoot()
        {
            JsonPath path = new JsonPath(JsonPath.Root);
            Assert.AreEqual(JsonPath.Root, path.Top);
        }

        [Test]
        public void TestMultiPath()
        {
            JsonPath path = new JsonPath(JsonPath.Root);
            path = path.Append("foo");
            path = path.Append("bar");
            Assert.AreEqual(JsonPath.Root + "['foo']['bar']", path.ToString());
        }

        [Test]
        public void TestParse()
        {
            JsonPath path = new JsonPath(JsonPath.Root + "['foo']['bar'][0]");
            Assert.AreEqual(JsonPath.Root, path.Top);
            path = path.ChildReference();
            Assert.AreEqual("foo", path.Top);
            path = path.ChildReference();
            Assert.AreEqual("bar", path.Top);
            path = path.ChildReference();
            Assert.AreEqual(0, path.TopAsInt);
        }

        [Test]
        public void TestImmutableAdd()
        {
            JsonPath path = new JsonPath(JsonPath.Root);
            JsonPath path2 = path.Append("foo");
            Assert.AreNotSame(path, path2);
        }

        [Test]
        public void TestImmutableChild()
        {
            JsonPath path = new JsonPath(JsonPath.Root);
            path = path.Append("foo");
            JsonPath path2 = path.ChildReference();
            Assert.AreNotSame(path, path2);
        }
    }
}
