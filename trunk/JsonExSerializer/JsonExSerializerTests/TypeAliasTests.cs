using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer;
using System.Collections;
using JsonExSerializerTests.Mocks;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class TypeAliasTests
    {
        [Test]
        public void PrimitivesAliasedByDefault()
        {
            Serializer s = GetSerializer();
            string result = s.Serialize(1);
            StringAssert.FullMatch(result, @"\s*\(int\)\s*1\s*");
            int i = (int)s.Deserialize(result);
            Assert.AreEqual(1, i, "Deserialize");
        }

        [Test]
        public void NonGenericClass()
        {
            Serializer s = GetSerializer();
            s.Context.TypeAliases.Add(typeof(ArrayList), "array");
            string result = s.Serialize(new ArrayList());
            StringAssert.FullMatch(result, @"\s*\(array\)\s*\[\s*\]\s*");
            ArrayList targetList = (ArrayList)s.Deserialize(result);
            Assert.AreEqual(0, targetList.Count, "Deserialize");
        }

        [Test]
        public void AliasedGenericArgument()
        {
            Serializer s = GetSerializer();
            s.Context.TypeAliases.Add(typeof(ArrayList), "array");
            string result = s.Serialize(new List<ArrayList>());
            StringAssert.FullMatch(result, @"\s*\(System\.Collections\.Generic\.List<array>\)\s*\[\s*\]\s*");
            List<ArrayList> targetList = (List<ArrayList>)s.Deserialize(result);
            Assert.AreEqual(0, targetList.Count, "Deserialize");
        }

        [Test]
        public void AssemblyAliasTest()
        {
            Serializer s = GetSerializer();            
            MockValueType source = new MockValueType(9, 6);
            s.Context.TypeAliases.Assemblies.Add(typeof(MockValueType).Assembly);
            string result = s.Serialize(source);
            StringAssert.FullMatch(result, @"\s*\(JsonExSerializerTests\.Mocks\.MockValueType\)\s*{""X"":9, ""Y"":6}");
            MockValueType target = (MockValueType)s.Deserialize(result);
            Assert.AreEqual(source, target, "Deserialize");
        }

        [Test]
        public void AliasedOpenGenericType()
        {
            Serializer s = GetSerializer();
            s.Context.TypeAliases.Add(typeof(Dictionary<,>), "dictionary");
            string result = s.Serialize(new Dictionary<string, int>());
            StringAssert.FullMatch(result, @"\s*\(dictionary<string,int>\)\s*\{\s*\}\s*");
            Dictionary<string, int> targetDictionary = (Dictionary<string, int>)s.Deserialize(result);
            Assert.AreEqual(0, targetDictionary.Count, "Deserialize");
        }
        private static Serializer GetSerializer()
        {
            Serializer s = new Serializer(typeof(object));
            s.Context.OutputTypeComment = false;
            s.Context.IsCompact = true;
            return s;
        }
    }
}
