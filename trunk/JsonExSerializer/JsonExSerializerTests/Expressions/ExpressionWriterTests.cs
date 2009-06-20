using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MbUnit.Framework;
using JsonExSerializer;
using JsonExSerializer.Framework.Expressions;
using JsonExSerializer.Framework;
using System.Collections;

namespace JsonExSerializerTests.Expressions
{
    
    [TestFixture]
    public class ExpressionWriterTests
    {
        private StringWriter _stringWriter;
        private JsonWriter _jsonWriter;
        private SerializationContext _context;
        private ExpressionWriter _exprWriter;

        [SetUp]
        public void TestSetup()
        {
            _stringWriter = new StringWriter();
            _jsonWriter = new JsonWriter(_stringWriter, false);
            _context = new SerializationContext();
            _exprWriter = new ExpressionWriter(_jsonWriter, _context);
        }

        [RowTest]
        [Row(true)]
        [Row(false)]
        public void WriteBoolean(bool value)
        {
            BooleanExpression expr = new BooleanExpression(value);
            _exprWriter.Write(expr);
            Assert.AreEqual(value.ToString().ToLower(), _stringWriter.ToString());
        }

        [RowTest]
        [Row(1)]
        [Row(0)]
        [Row(-1)]
        [Row(10L)]
        [Row(0L)]
        [Row(-17L)]
        [Row(2.3)]
        [Row(-2.3)]
        [Row(4.9f)]
        [Row(-4.9f)]
        [Row(5.1e10)]
        [Row(-5.1e10)]
        [Row(5.1e-10)]
        [Row(-5.1e-10)]
        public void WriteNumber(object value)
        {
            NumericExpression expr = new NumericExpression(value);
            _exprWriter.Write(expr);
            Assert.AreEqual(value.ToString(), _stringWriter.ToString());
        }

        [RowTest]
        [Row("test", "test")]
        [Row("Single 'Quote'", "Single 'Quote'")]
        [Row("Double \"Quote\"", "Double \\\"Quote\\\"")]
        [Row("", "")]
        public void WriteString(string value, string expected)
        {
            ValueExpression expr = new ValueExpression(value);
            _exprWriter.Write(expr);
            Assert.AreEqual("\"" + expected + "\"", _stringWriter.ToString());
        }

        [Test]
        public void WriteNull()
        {
            _exprWriter.Write(new NullExpression());
            Assert.AreEqual("null", _stringWriter.ToString());
        }

        [Test]
        public void WriteEmptyList()
        {
            ArrayExpression list = new ArrayExpression();
            _exprWriter.Write(list);
            Assert.AreEqual("[]", _stringWriter.ToString());
        }

        [Test]
        public void WriteSimpleList()
        {
            ArrayExpression list = new ArrayExpression();
            list.Items.Add(new BooleanExpression(true));
            list.Items.Add(new NumericExpression(12));
            list.Items.Add(new ValueExpression("test"));
            list.Items.Add(new NullExpression());
            _exprWriter.Write(list);
            Assert.AreEqual("[true, 12, \"test\", null]", _stringWriter.ToString());
        }

        [Test]
        public void WriteEmptyObject()
        {
            ObjectExpression obj = new ObjectExpression();
            _exprWriter.Write(obj);
            Assert.AreEqual("{}", _stringWriter.ToString());
        }

        [Test]
        public void WriteSimpleObject()
        {
            ObjectExpression obj = new ObjectExpression();
            obj.Add("bool", new BooleanExpression(true));
            obj.Add("number", new NumericExpression(12));
            obj.Add("string", new ValueExpression("test"));
            obj.Add("nullprop", new NullExpression());
            _exprWriter.Write(obj);
            Assert.AreEqual("{\"bool\":true, \"number\":12, \"string\":\"test\", \"nullprop\":null}", _stringWriter.ToString());
        }

        [Test]
        public void WriteObjectWithConstructor()
        {
            ObjectExpression obj = new ObjectExpression();
            obj.ResultType = typeof(string);
            obj.ConstructorArguments.Add(new NumericExpression(12));
            obj.Add("keyname", new ValueExpression("valuename"));
            _exprWriter.Write(obj);
            Assert.AreEqual("new string(12){\"keyname\":\"valuename\"}",_stringWriter.ToString());
        }

        [Test]
        public void WriteReference()
        {
            ReferenceExpression refExpr = new ReferenceExpression("this.a.b");
            _exprWriter.Write(refExpr);
            Assert.AreEqual("this.a.b",_stringWriter.ToString());
        }

        [Test]
        public void SimpleCast_OutputTypeInfo()
        {
            CastExpression cast = new CastExpression(typeof(byte));
            cast.Expression = new NumericExpression(255);
            _exprWriter.Write(cast);
            Assert.AreEqual("(byte)255", _stringWriter.ToString());
        }

        [Test]
        public void SimpleCast_NoOutputTypeInfo()
        {
            CastExpression cast = new CastExpression(typeof(byte));
            cast.Expression = new NumericExpression(255);
            _context.OutputTypeInformation = false;
            _exprWriter.Write(cast);
            Assert.AreEqual("255", _stringWriter.ToString());
        }

        [Test]
        public void CastList()
        {
            CastExpression cast = new CastExpression(typeof(ArrayList));
            cast.Expression = new ArrayExpression();
            _exprWriter.Write(cast);
            Assert.AreEqual("(System.Collections.ArrayList)[]", _stringWriter.ToString());
        }

        [Test]
        public void CastObject()
        {
            CastExpression cast = new CastExpression(typeof(Hashtable));
            cast.Expression = new ObjectExpression();
            _exprWriter.Write(cast);
            Assert.AreEqual("(System.Collections.Hashtable){}", _stringWriter.ToString());
        }

    }
}
