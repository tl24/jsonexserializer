using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer;
using JsonExSerializerTests.Mocks;
using JsonExSerializer.MetaData;

namespace JsonExSerializerTests.MetaData
{
    [TestFixture]
    public class ConstructorParameterTests
    {
        [Test]
        public void TestNamedOnlyExact()
        {
            SerializerSettings context = new SerializerSettings();
            TypeData typeData = new TypeData<NamedOnlyExactConstructor>(context);
            IList<IPropertyData> propData = typeData.ConstructorParameters;
            List<IPropertyData> expected = new List<IPropertyData>();
            expected.Add(typeData.FindProperty("StringPropA"));
            expected.Add(typeData.FindProperty("StringPropB"));
            CollectionAssert.AreElementsEqual(expected, propData);
        }

        [Test]
        public void TestNamedOnlyIgnoreCase()
        {
            SerializerSettings context = new SerializerSettings();
            TypeData typeData = new TypeData<NamedOnlyIgnoreCaseConstructor>(context);
            IList<IPropertyData> propData = typeData.ConstructorParameters;
            List<IPropertyData> expected = new List<IPropertyData>();
            expected.Add(typeData.FindProperty("StringPropA"));
            expected.Add(typeData.FindProperty("StringPropB"));
            CollectionAssert.AreElementsEqual(expected, propData);
        }

        [Test]
        public void TestMixed()
        {
            SerializerSettings context = new SerializerSettings();
            TypeData typeData = new TypeData<MixedExactConstructor>(context);
            IList<IPropertyData> propData = typeData.ConstructorParameters;
            List<IPropertyData> expected = new List<IPropertyData>();
            expected.Add(typeData.FindProperty("StringPropA"));
            expected.Add(typeData.FindProperty("StringPropB"));
            expected.Add(typeData.FindProperty("IntProp"));
            CollectionAssert.AreElementsEqual(expected, propData);
        }

        [Test]
        public void TestNamedArgumentsNotIgnored()
        {
            SerializerSettings context = new SerializerSettings();
            ITypeData typeData = new TypeData<NamedOnlyExactConstructor>(context);
            IPropertyData stringPropA = typeData.FindProperty("StringPropA");
            IPropertyData stringPropB = typeData.FindProperty("StringPropB");
            Assert.IsFalse(stringPropA.Ignored, "stringPropA Constructor argument should not be ignored");
            Assert.IsFalse(stringPropB.Ignored, "stringPropA Constructor argument should not be ignored");
        }

        [Test]
        public void TestParameterAlias()
        {
            SerializerSettings context = new SerializerSettings();
            ITypeData typeData = new TypeData<AliasedConstructor>(context);
            List<IPropertyData> expected = new List<IPropertyData>();
            expected.Add(typeData.FindProperty("IntProp"));
            CollectionAssert.AreElementsEqual(expected, typeData.ConstructorParameters);
        }
    }
}
