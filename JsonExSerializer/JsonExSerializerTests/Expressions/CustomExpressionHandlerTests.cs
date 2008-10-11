using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer.Framework.ExpressionHandlers;
using JsonExSerializer.Framework.Expressions;
using System.Globalization;
using JsonExSerializer;

namespace JsonExSerializerTests.Expressions
{
    [TestFixture]
    public class CustomExpressionHandlerTests
    {
        [Test]
        public void TestFullDateTime()
        {
            DateTimeExpressionHandler handler = new DateTimeExpressionHandler("F");
            Serializer s = new Serializer(typeof(DateTime));
            s.Context.ExpressionHandlers.InsertBefore(typeof(ValueExpressionHandler), handler);
            s.Context.SetJsonStrictOptions();
            s.Context.IsCompact = true;
            DateTime source = new DateTime(2008, 10, 9, 13, 23, 45);
            string result = s.Serialize(source);
            Assert.AreEqual(string.Format("\"{0:F}\"", source), result, "DateTime not serialized correctly using Full DateTime Format");

            DateTime deserialized = (DateTime) s.Deserialize(result);
            Assert.AreEqual(source, deserialized, "DateTime did not deserialize properly using Full DateTime Format");
        }

        [Test]
        public void TestISODateTime()
        {
            DateTimeExpressionHandler handler = new DateTimeExpressionHandler("O");
            Serializer s = new Serializer(typeof(DateTime));
            s.Context.ExpressionHandlers.InsertBefore(typeof(ValueExpressionHandler), handler);
            s.Context.SetJsonStrictOptions();
            s.Context.IsCompact = true;
            DateTime source = new DateTime(2008, 10, 9, 13, 23, 45);
            string result = s.Serialize(source);
            Assert.AreEqual(string.Format("\"{0:O}\"", source), result, "DateTime not serialized correctly using ISO DateTime Format");

            DateTime deserialized = (DateTime)s.Deserialize(result);
            Assert.AreEqual(source, deserialized, "DateTime did not deserialize properly using ISO DateTime Format");
        }

        [Test]
        public void VerifyContextSet()
        {
            ValueExpressionHandler handler = new ValueExpressionHandler();
            Serializer s = new Serializer(typeof(object));
            s.Context.ExpressionHandlers.Insert(0, handler);
            Assert.AreSame(s.Context, handler.Context, "IContextAware.Context not set on ExpressionHandler when inserted into Handlers collection");
        }

        public class DateTimeExpressionHandler : ValueExpressionHandler
        {
            private string dateFormat = "";
            public DateTimeExpressionHandler(string dateFormat)
            {
                this.dateFormat = dateFormat;
            }

            public override bool CanHandle(Type ObjectType)
            {
                return typeof(DateTime).IsAssignableFrom(ObjectType);
            }

            public override Expression GetExpression(object data, JsonPath currentPath, ISerializerHandler serializer)
            {
                string value = ((DateTime)data).ToString(dateFormat);
                return new ValueExpression(value);
            }

            public override object Evaluate(Expression expression, IDeserializerHandler deserializer)
            {
                ValueExpression valueExpr = (ValueExpression)expression;
                return DateTime.ParseExact(valueExpr.StringValue, dateFormat, CultureInfo.CurrentCulture.DateTimeFormat);
            }
        }
    }
}
