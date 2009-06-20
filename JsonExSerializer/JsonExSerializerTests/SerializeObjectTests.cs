using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer;
using JsonExSerializerTests.Mocks;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class SerializeObjectTests
    {

        [Test]
        public void SimpleObjectTest()
        {
            SimpleObject src = new SimpleObject();
            src.BoolValue = true;
            src.ByteValue = 0x89;
            src.CharValue = 'h';
            src.DoubleValue = double.MaxValue;
            src.FloatValue = float.MaxValue;
            src.IntValue = int.MaxValue;
            src.LongValue = int.MinValue;
            src.ShortValue = short.MaxValue;
            src.StringValue = "The simple string";

            Serializer s = new Serializer(typeof(SimpleObject));
            string result = s.Serialize(src);
            SimpleObject dst = (SimpleObject) s.Deserialize(result);
            ValidateSimpleObjects(src, dst);
        }

        [Test]
        public void SemiComplexObjectTest()
        {
            SimpleObject src = new SimpleObject();
            src.BoolValue = true;
            src.ByteValue = 0x99;
            src.CharValue = '\n';
            src.DoubleValue = 1234.4567;
            src.FloatValue = float.MinValue;
            src.IntValue = int.MinValue;
            src.LongValue = int.MaxValue;
            src.ShortValue = short.MinValue;
            src.StringValue = "The\nmultiline\nstring";
            SemiComplexObject complex = new SemiComplexObject();
            complex.SimpleObject = src;
            complex.Name = "MyComplexObject";

            Serializer s = new Serializer(typeof(SemiComplexObject));
            string result = s.Serialize(complex);
            SemiComplexObject complexDest = (SemiComplexObject) s.Deserialize(result);
            Assert.AreEqual(complex.Name, complexDest.Name, "SemiComplex Name not deserialized correctly");
            ValidateSimpleObjects(complex.SimpleObject, complexDest.SimpleObject);            
        }

        [Test]
        public void NullValueTest()
        {
            SpecializedMock expected = new SpecializedMock();
            expected.Name = null;
            Serializer s = new Serializer(typeof(SpecializedMock));
            string result = s.Serialize(expected);
            SpecializedMock actual = (SpecializedMock)s.Deserialize(result);
            Assert.IsNull(actual.Name, "Null value not serialized, deserialized correctly");
        }

        [Test]
        public void GetOnlyPropertyTest()
        {
            SpecializedMock co = new SpecializedMock(33);
            co.Name = "ThirtyThree";
            Serializer s = new Serializer(typeof(SpecializedMock));
            string result = s.Serialize(co);
            SpecializedMock actual = (SpecializedMock)s.Deserialize(result);
            Assert.AreEqual(co.Name, actual.Name, "Name properties don't match");
            Assert.AreNotEqual(co.Count, actual.Count, "Read only property not deserialized correctly");
        }

        [Test]
        public void IgnoreAttributeTest()
        {
            SpecializedMock co = new SpecializedMock(33);
            co.Name = "ThirtyThree";
            co.IgnoredProp = "IgnoreMe";
            Serializer s = new Serializer(typeof(SpecializedMock));
            string result = s.Serialize(co);
            SpecializedMock actual = (SpecializedMock)s.Deserialize(result);
            Assert.AreNotEqual(co.IgnoredProp, actual.IgnoredProp, "Ignored property not ignored");
        }

        [Test]
        public void SubClassTest()
        {
            SubClassMock co = new SubClassMock();
            co.Name = "ThirtyThree";
            co.SubClassProp = "Some Subclass";
            Serializer s = new Serializer(typeof(SubClassMock));
            string result = s.Serialize(co);
            SubClassMock actual = (SubClassMock)s.Deserialize(result);
            Assert.AreEqual(co.Name, actual.Name, "Base class properties don't match");
            Assert.AreEqual(co.SubClassProp, actual.SubClassProp, "Sub Class properties don't match");
        }

        /// <summary>
        /// Tests a cast containing generic parameters
        /// </summary>
        [Test]
        public void CastWithGenericsTest()
        {
            Serializer s = new Serializer(typeof(IDictionary<string, int>));
            IDictionary<string, int> dict = new Dictionary<string, int>();
            dict.Add("one", 1);
            dict.Add("two", 2);
            dict.Add("ten", 10);
            string result = s.Serialize(dict);
            // make sure concrete type is correct
            Dictionary<string, int> actual = (Dictionary<string, int>)s.Deserialize(result);
            AssertDictionariesEqual<string, int>(dict, actual, "dictionaries not equal");
        }

        /// <summary>
        /// Tests an enum as a dictionary key
        /// </summary>
        [Test]
        public void EnumDictionaryKeyTest()
        {
            Serializer s = new Serializer(typeof(IDictionary<string, int>));
            IDictionary<SimpleEnum, string> dict = new Dictionary<SimpleEnum, string>();
            dict.Add(SimpleEnum.EnumValue1, "value1");
            dict.Add(SimpleEnum.EnumValue2, "value2");
            string result = s.Serialize(dict);
            // make sure concrete type is correct
            Dictionary<SimpleEnum, string> actual = (Dictionary<SimpleEnum, string>)s.Deserialize(result);
            AssertDictionariesEqual<SimpleEnum, string>(actual, dict, "Enum keyed dictionaries not equal");
        }

        [Test]
        public void MissingCommaBetweenKeyValuesThrowsException()
        {
            Serializer s = new Serializer(typeof(Dictionary<string, string>));
            string result = @"{""a"":""1"" ""b"":""2""}";
            try
            {
                object obj = s.Deserialize(result);
                Assert.Fail("No exception thrown for object with missing comma");
            }
            catch (ParseException e)
            {
            }
            catch (Exception e)
            {
                Assert.Fail("Wrong exception type thrown: " + e);
            }
        }



        [Test]
        public void EmptyObjectTest()
        {
            Dictionary<string, int> source = new Dictionary<string, int>();
            Serializer s = new Serializer(typeof(Dictionary<string, int>));
            s.Context.IsCompact = true;
            s.Context.OutputTypeComment = false;
            string result = s.Serialize(source);
            StringAssert.FullMatch(result, @"\s*\{\s*\}\s*");
            Dictionary<string, int> target = (Dictionary<string, int>)s.Deserialize(result);
            Assert.AreEqual(0, target.Count, "Dictionary count");
        }

        [Test]
        public void SingleItemObjectTest()
        {
            Dictionary<string, int> source = new Dictionary<string, int>();
            Serializer s = new Serializer(typeof(Dictionary<string, int>));
            s.Context.IsCompact = true;
            s.Context.OutputTypeComment = false;
            source["first"] = 1;
            string result = s.Serialize(source);
            StringAssert.FullMatch(result, @"\s*\{\s*""first""\s*:\s*1\s*\}\s*");
            Dictionary<string, int> target = (Dictionary<string, int>)s.Deserialize(result);
            Assert.AreEqual(1, target.Count, "Dictionary count");
            Assert.AreEqual(1, target["first"], "Dictionary item");
        }

        [Test]
        public void StructTypeTest()
        {
            Serializer s = new Serializer(typeof(MockValueType));
            MockValueType value = new MockValueType(5, 10);
            string result = s.Serialize(value);

            MockValueType actual = (MockValueType)s.Deserialize(result);
            Assert.AreEqual(value, actual, "Structs not equal");
        }

        public object TestStructGetter(object o)
        {
            return ((MockValueType)o).X;
        }

        public object TestClassGetter(object o)
        {
            return ((SimpleObject)o).BoolValue;
        }

        public object TestObjectConstructor()
        {
            return new SimpleObject();
        }

        public object TestStructConstructor()
        {
            return new MockValueType();
        }

        public void ValidateSimpleObjects(SimpleObject src, SimpleObject dst)
        {
            Assert.AreEqual(src.BoolValue, dst.BoolValue, "SimpleObject.BoolValue not equal");
            Assert.AreEqual(src.ByteValue, dst.ByteValue, "SimpleObject.ByteValue not equal");
            Assert.AreEqual(src.CharValue, dst.CharValue, "SimpleObject.CharValue not equal");
            Assert.AreEqual(src.DoubleValue, dst.DoubleValue, "SimpleObject.DoubleValue not equal");
            Assert.AreEqual(src.FloatValue, dst.FloatValue, "SimpleObject.FloatValue not equal");
            Assert.AreEqual(src.IntValue, dst.IntValue, "SimpleObject.IntValue not equal");
            Assert.AreEqual(src.LongValue, dst.LongValue, "SimpleObject.LongValue not equal");
            Assert.AreEqual(src.ShortValue, dst.ShortValue, "SimpleObject.ShortValue not equal");
            Assert.AreEqual(src.StringValue, dst.StringValue, "SimpleObject.StringValue not equal");
        }

        public void AssertDictionariesEqual<K, V>(IDictionary<K, V> expected, IDictionary<K, V> actual, string message)
        {
            Assert.AreEqual(expected.Count, actual.Count, message + " Counts not equal");
            foreach (K key in expected.Keys)
            {
                Assert.AreEqual(expected[key], actual[key], message + " Values not equal for key: " + key);
            }
        }
    }
}
