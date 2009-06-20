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
            s.Config.ExpressionHandlers.InsertBefore(typeof(DateTimeExpressionHandler), handler);
            s.Config.SetJsonStrictOptions();
            s.Config.IsCompact = true;
            DateTime source = new DateTime(2008, 10, 9, 13, 23, 45);
            string result = s.Serialize(source);
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, "\"{0:F}\"", source), result, "DateTime not serialized correctly using Full DateTime Format");

            DateTime deserialized = (DateTime) s.Deserialize(result);
            Assert.AreEqual(source, deserialized, "DateTime did not deserialize properly using Full DateTime Format");
        }

        [Test]
        public void TestISODateTime()
        {
            DateTimeExpressionHandler handler = new DateTimeExpressionHandler("O");
            Serializer s = new Serializer(typeof(DateTime));
            s.Config.ExpressionHandlers.InsertBefore(typeof(DateTimeExpressionHandler), handler);
            s.Config.SetJsonStrictOptions();
            s.Config.IsCompact = true;
            DateTime source = new DateTime(2008, 10, 9, 13, 23, 45);
            string result = s.Serialize(source);
            Assert.AreEqual(string.Format("\"{0:O}\"", source), result, "DateTime not serialized correctly using ISO DateTime Format");

            DateTime deserialized = (DateTime)s.Deserialize(result);
            Assert.AreEqual(source, deserialized, "DateTime did not deserialize properly using ISO DateTime Format");
        }

        [Test]
        public void VerifyConfigSet()
        {
            ValueExpressionHandler handler = new ValueExpressionHandler();
            Serializer s = new Serializer(typeof(object));
            s.Config.ExpressionHandlers.Insert(0, handler);
            Assert.AreSame(s.Config, handler.Config, "IConfigurationAware.Config not set on ExpressionHandler when inserted into Handlers collection");
        }

        
    }
}
