using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer.MetaData;
using JsonExSerializer;
using JsonExSerializerTests.Mocks;

namespace JsonExSerializerTests.MetaData
{
    [TestFixture]
    public class PropertyNamingTests
    {
        [Test]
        public void DefaultStrategy_ReturnsOrginal()
        {
            DefaultPropertyNamingStrategy strategy = new DefaultPropertyNamingStrategy();
            string original = "listOfCrazy_mixed_upNamesIOXML_HTTP";
            string modified = strategy.GetName(original);
            Assert.AreEqual(original, modified);
        }

        [RowTest]
        [Row("NoChange", "NoChange")]
        [Row("lowerFirst", "LowerFirst")]
        [Row("with_underscores", "WithUnderscores")]
        [Row("XMLDocument","XmlDocument")]
        [Row("RawHTML", "RawHtml")]
        [Row("alllower", "Alllower")]
        public void PascalNamingStrategyTest(string original, string expected)
        {
            PascalCaseNamingStrategy strategy = new PascalCaseNamingStrategy();
            string modified = strategy.GetName(original);
            Assert.AreEqual(expected, modified);
        }

        [RowTest]
        [Row("NoChange", "noChange")]
        [Row("lowerFirst", "lowerFirst")]
        [Row("with_underscores", "withUnderscores")]
        [Row("XMLDocument", "xmlDocument")]
        [Row("RawHTML", "rawHtml")]
        [Row("alllower", "alllower")]
        public void CamelNamingStrategyTest(string original, string expected)
        {
            CamelCaseNamingStrategy strategy = new CamelCaseNamingStrategy();
            string modified = strategy.GetName(original);
            Assert.AreEqual(expected, modified);
        }

        [RowTest]
        [Row("NoChange", "No_Change")]
        [Row("lowerFirst", "lower_First")]
        [Row("with_underscores", "with_underscores")]
        [Row("XMLDocument", "XML_Document")]
        [Row("RawHTML", "Raw_HTML")]
        [Row("alllower", "alllower")]
        public void UnderscoreStrategy_OriginalTest(string original, string expected)
        {
            UnderscoreNamingStrategy strategy = new UnderscoreNamingStrategy(UnderscoreNamingStrategy.UnderscoreCaseStyle.OriginalCase);
            string modified = strategy.GetName(original);
            Assert.AreEqual(expected, modified);
        }

        [RowTest]
        [Row("NoChange", "no_change")]
        [Row("lowerFirst", "lower_first")]
        [Row("with_underscores", "with_underscores")]
        [Row("XMLDocument", "xml_document")]
        [Row("RawHTML", "raw_html")]
        [Row("alllower", "alllower")]
        public void UnderscoreStrategy_LowerTest(string original, string expected)
        {
            UnderscoreNamingStrategy strategy = new UnderscoreNamingStrategy(UnderscoreNamingStrategy.UnderscoreCaseStyle.LowerCase);
            string modified = strategy.GetName(original);
            Assert.AreEqual(expected, modified);
        }

        [RowTest]
        [Row("NoChange", "NO_CHANGE")]
        [Row("lowerFirst", "LOWER_FIRST")]
        [Row("with_underscores", "WITH_UNDERSCORES")]
        [Row("XMLDocument", "XML_DOCUMENT")]
        [Row("RawHTML", "RAW_HTML")]
        [Row("alllower", "ALLLOWER")]
        public void UnderscoreStrategy_UpperTest(string original, string expected)
        {
            UnderscoreNamingStrategy strategy = new UnderscoreNamingStrategy(UnderscoreNamingStrategy.UnderscoreCaseStyle.UpperCase);
            string modified = strategy.GetName(original);
            Assert.AreEqual(expected, modified);
        }

        [RowTest]
        [Row("NoChange", "No_Change")]
        [Row("lowerFirst", "Lower_First")]
        [Row("with_underscores", "With_Underscores")]
        [Row("XMLDocument", "Xml_Document")]
        [Row("RawHTML", "Raw_Html")]
        [Row("alllower", "Alllower")]
        public void UnderscoreStrategy_MixedTest(string original, string expected)
        {
            UnderscoreNamingStrategy strategy = new UnderscoreNamingStrategy(UnderscoreNamingStrategy.UnderscoreCaseStyle.MixedCase);
            string modified = strategy.GetName(original);
            Assert.AreEqual(expected, modified);
        }

        [Test]
        public void TypeDataNamingStrategy_AppliedToProperties()
        {
            SerializationContext config = new SerializationContext();
            config.TypeHandlerFactory.SetPropertyNamingStrategy(new UnderscoreNamingStrategy());
            TypeData td = config.TypeHandlerFactory[typeof(SimpleObject)];
            IPropertyData pd = td.FindPropertyByName("ByteValue");
            Assert.AreEqual("Byte_Value", pd.Alias, "ByteValue Alias");
            IPropertyData bvByAlias = td.FindPropertyByAlias("Byte_Value");
            Assert.AreSame(pd, bvByAlias, "ByteValue By Alias");
        }

        [Test]
        public void SerializationUsesAliases()
        {
            Serializer s = new Serializer(typeof(MyLine));
            s.Config.TypeHandlerFactory.SetPropertyNamingStrategy(new DelegateNamingStrategy(delegate(string old) { return "__" + old.ToLower() + "__"; }), true);
            MyLine source = new MyLine();
            source.Start = new MyImmutablePoint(0,0);
            source.End = new MyImmutablePoint(5, -5);
            s.Config.OutputTypeComment = false;
            s.Config.IsCompact = true;
            string result = s.Serialize(source);
            string expected = "{\"__start__\":\"(0:0)\", \"__end__\":\"5,-5\"}";
            Assert.AreEqual(expected, result, "serialization");
            MyLine target = (MyLine)s.Deserialize(result);
            Assert.AreEqual(source, target, "Deserialization");
        }

        [Test]
        public void JsonExProperty_AppliesAlias()
        {
            Serializer s = new Serializer(typeof(JsonPropertyAlias));
            IPropertyData pd = s.Config.TypeHandlerFactory[typeof(JsonPropertyAlias)].FindProperty("MyProperty");
            Assert.AreEqual("serialize_this", pd.Alias);
        }

        [Test]
        public void JsonExProperty_AliasOverridesNamingStrategy()
        {
            Serializer s = new Serializer(typeof(JsonPropertyAlias));
            s.Config.TypeHandlerFactory.SetPropertyNamingStrategy(new DelegateNamingStrategy(delegate(string old) { return "__" + old + "__"; }));
            IPropertyData pd = s.Config.TypeHandlerFactory[typeof(JsonPropertyAlias)].FindProperty("MyProperty");
            Assert.AreEqual("serialize_this", pd.Alias);
        }

        public class JsonPropertyAlias
        {
            [JsonExProperty("serialize_this")]
            public int MyProperty
            {
                get { return 0; }
                set { }
            }
        }
    }
}
