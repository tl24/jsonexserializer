using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer.Framework.ExpressionHandlers;
using JsonExSerializer.Framework.Expressions;
using System.Globalization;
using JsonExSerializer;
using System.Data;
using JsonExSerializer.CustomHandlers;
using JsonExSerializer.TypeConversion;

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

        [Test]
        public void DataTableSerialize()
        {
            DataTable dt = new DataTable("MyTable");
            dt.Columns.Add("IntColumn", typeof(int));
            dt.Columns.Add("StringColumn", typeof(string));
            dt.Columns.Add("BoolColumn", typeof(bool));
            dt.Columns.Add("DateTimeColumn", typeof(DateTime));
            dt.Columns.Add("DoubleColumn", typeof(double));
            dt.Rows.Add((int)32, "row 1", true, new DateTime(2009, 9, 1), 213.45d);
            dt.Rows.Add((int)64, "row 2", false, new DateTime(2005, 5, 15), 124.95d);

            Serializer s = new Serializer(typeof(DataTable));
            s.Config.ExpressionHandlers.Insert(0, new DataTableExpressionHandler());
            s.Config.RegisterTypeConverter(typeof(Type), new TypeToStringConverter()); // for DataType property of Column
            string result = s.Serialize(dt);

            DataTable dtResult = (DataTable) s.Deserialize(result);
            DataSet expected = new DataSet();
            expected.Tables.Add(dt);

            DataSet actual = new DataSet();
            actual.Tables.Add(dtResult);
            DataAssert.AreDataEqual(expected, actual);
        }
        
    }
}
