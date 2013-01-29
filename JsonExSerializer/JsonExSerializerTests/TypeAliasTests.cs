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
            string result = s.Serialize<object>(1);
            StringAssert.FullMatch(result, @"\s*\(int\)\s*1\s*");
            int i = (int) s.Deserialize<object>(result);
            Assert.AreEqual(1, i, "Deserialize");
        }

        [Test]
        public void NonGenericClass()
        {
            Serializer s = GetSerializer();
            s.Settings.TypeAliases.Add(typeof(ArrayList), "array");
            string result = s.Serialize<object>(new ArrayList());
            StringAssert.FullMatch(result, @"\s*\(array\)\s*\[\s*\]\s*");
            ArrayList targetList = s.Deserialize<ArrayList>(result);
            Assert.AreEqual(0, targetList.Count, "Deserialize");
        }

        [Test]
        public void AliasedGenericArgument()
        {
            Serializer s = GetSerializer();
            s.Settings.TypeAliases.Add(typeof(ArrayList), "array");
            string result = s.Serialize<object>(new List<ArrayList>());
            StringAssert.FullMatch(result, @"\s*\(System\.Collections\.Generic\.List<array>\)\s*\[\s*\]\s*");
            List<ArrayList> targetList = s.Deserialize<List<ArrayList>>(result);
            Assert.AreEqual(0, targetList.Count, "Deserialize");
        }

        [Test]
        public void AssemblyAliasTest()
        {
            Serializer s = GetSerializer();            
            MockValueType source = new MockValueType(9, 6);
            s.Settings.TypeAliases.Assemblies.Add(typeof(MockValueType).Assembly);
            string result = s.Serialize<object>(source);
            StringAssert.FullMatch(result, @"\s*\(JsonExSerializerTests\.Mocks\.MockValueType\)\s*{""X"":9, ""Y"":6}");
            MockValueType target = s.Deserialize<MockValueType>(result);
            Assert.AreEqual(source, target, "Deserialize");
        }

        [Test]
        public void AliasedOpenGenericType()
        {
            Serializer s = GetSerializer();
            s.Settings.TypeAliases.Add(typeof(Dictionary<,>), "dictionary");
            string result = s.Serialize<object>(new Dictionary<string, int>());
            StringAssert.FullMatch(result, @"\s*\(dictionary<string,int>\)\s*\{\s*\}\s*");
            Dictionary<string, int> targetDictionary = s.Deserialize<Dictionary<string, int>>(result);
            Assert.AreEqual(0, targetDictionary.Count, "Deserialize");
        }
        private static Serializer GetSerializer()
        {
            Serializer s = new Serializer();
            s.Settings.IsCompact = true;
            return s;
        }
    }
}
