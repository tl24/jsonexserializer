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
            SerializationContext context = new SerializationContext();
            TypeData typeData = new TypeData(typeof(NamedOnlyExactConstructor), context);
            IList<IPropertyData> propData = typeData.ConstructorParameters;
            List<IPropertyData> expected = new List<IPropertyData>();
            expected.Add(typeData.FindProperty("StringPropA"));
            expected.Add(typeData.FindProperty("StringPropB"));
            CollectionAssert.AreElementsEqual(expected, propData);
        }

        [Test]
        public void TestNamedOnlyIgnoreCase()
        {
            SerializationContext context = new SerializationContext();
            TypeData typeData = new TypeData(typeof(NamedOnlyIgnoreCaseConstructor), context);
            IList<IPropertyData> propData = typeData.ConstructorParameters;
            List<IPropertyData> expected = new List<IPropertyData>();
            expected.Add(typeData.FindProperty("StringPropA"));
            expected.Add(typeData.FindProperty("StringPropB"));
            CollectionAssert.AreElementsEqual(expected, propData);
        }

        [Test]
        public void TestMixed()
        {
            SerializationContext context = new SerializationContext();
            TypeData typeData = new TypeData(typeof(MixedExactConstructor), context);
            IList<IPropertyData> propData = typeData.ConstructorParameters;
            List<IPropertyData> expected = new List<IPropertyData>();
            expected.Add(typeData.FindProperty("StringPropA"));
            expected.Add(typeData.FindProperty("StringPropB"));
            expected.Add(typeData.FindProperty("IntProp"));
            CollectionAssert.AreElementsEqual(expected, propData);
        }

        [Test]
        public void TestPositionOnly()
        {
            SerializationContext context = new SerializationContext();
            TypeData typeData = new TypeData(typeof(MyPointConstructor), context);
            IList<IPropertyData> propData = typeData.ConstructorParameters;
            List<IPropertyData> expected = new List<IPropertyData>();
            expected.Add(typeData.FindProperty("X"));
            expected.Add(typeData.FindProperty("Y"));
            CollectionAssert.AreElementsEqual(expected, propData);
        }
    }
}
