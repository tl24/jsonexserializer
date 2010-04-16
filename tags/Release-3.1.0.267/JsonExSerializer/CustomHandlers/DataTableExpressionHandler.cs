using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.ExpressionHandlers;
using System.Data;
using JsonExSerializer.Framework.Expressions;

namespace JsonExSerializer.CustomHandlers
{
    public class DataTableExpressionHandler : ObjectExpressionHandler
    {
        public DataTableExpressionHandler(IConfiguration config) : base() {

        }

        public DataTableExpressionHandler()
            : base()
        {

        }

        public override bool CanHandle(Type objectType)
        {
            return typeof(DataTable).IsAssignableFrom(objectType);
        }

        public override Expression GetExpression(object data, JsonPath currentPath, IExpressionBuilder serializer)
        {
            DataTable table = (DataTable) data;
            ObjectExpression tableExpr = new ObjectExpression();
            tableExpr.Add("TableName", serializer.Serialize(table.TableName, currentPath.Append("TableName")));
            tableExpr.Add("Columns", GetColumnsExpression(table, currentPath.Append("Columns"), serializer));
            tableExpr.Add("Rows", GetRowsExpression(table, currentPath, serializer));
            return tableExpr;
        }

        protected virtual Expression GetColumnsExpression(DataTable table, JsonPath currentPath, IExpressionBuilder serializer)
        {
            ArrayExpression columns = new ArrayExpression();
            int colCount = 0;
            foreach (DataColumn dc in table.Columns)
            {
                columns.Add(GetColumnExpression(dc, currentPath.Append(colCount), serializer));
                colCount++;
            }
            return columns;
        }

        protected virtual Expression GetColumnExpression(DataColumn dc, JsonPath jsonPath, IExpressionBuilder serializer)
        {
            ObjectExpression column = new ObjectExpression();
            // just DataType and column for now
            column.Add("DataType", serializer.Serialize(dc.DataType, jsonPath.Append("DataType")));
            column.Add("ColumnName", serializer.Serialize(dc.ColumnName, jsonPath.Append("ColumnName")));
            return column;
        }

        protected virtual Expression GetRowsExpression(DataTable table, JsonPath currentPath, IExpressionBuilder serializer)
        {
            ArrayExpression rowsExpr = new ArrayExpression();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow row = table.Rows[i];
                object[] values = row.ItemArray;
                JsonPath rowPath = currentPath.Append(i);
                ArrayExpression rowExpr = new ArrayExpression();
                for (int j = 0; j < values.Length; j++)
                {
                    rowExpr.Add(serializer.Serialize(values[j], rowPath.Append(j)));
                }
                rowsExpr.Add(rowExpr);
            }
            return rowsExpr;
        }

        public override object Evaluate(Expression expression, IDeserializerHandler deserializer)
        {
            DataTable table = new DataTable();
            ObjectExpression tableExpr = (ObjectExpression)expression;
            EvaluateColumns(table, (ArrayExpression)tableExpr["Columns"], deserializer);
            // remove the columns expression, it's been evaluated already
            tableExpr.Properties.RemoveAt(tableExpr.IndexOf("Columns"));

            // now the rows
            EvaluateRows(table, (ArrayExpression)tableExpr["Rows"], deserializer);
            // remove the rows expression, it's been evaluated already
            tableExpr.Properties.RemoveAt(tableExpr.IndexOf("Rows"));

            // fill in any remaining properties
            return base.Evaluate(expression, table, deserializer);
        }

        protected virtual void EvaluateColumns(DataTable table, ArrayExpression columnsExpression, IDeserializerHandler deserializer)
        {
            foreach (Expression colExpr in columnsExpression.Items)
            {
                table.Columns.Add(EvaluateColumn(colExpr, deserializer));
            }
        }

        protected virtual DataColumn EvaluateColumn(Expression colExpr, IDeserializerHandler deserializer)
        {
            colExpr.ResultType = typeof(DataColumn);
            return (DataColumn) deserializer.Evaluate(colExpr);
        }

        protected virtual void EvaluateRows(DataTable table, ArrayExpression rowsExpression, IDeserializerHandler deserializer)
        {
            for (int i = 0; i < rowsExpression.Items.Count; i++)
            {
                ArrayExpression rowExpr = (ArrayExpression) rowsExpression.Items[i];
                for (int j = 0; j < rowExpr.Items.Count; j++)
                {
                    rowExpr.Items[j] = new CastExpression(table.Columns[j].DataType, rowExpr.Items[j]);
                }
                rowExpr.ResultType = typeof(object[]);
                DataRow dr = table.NewRow();
                dr.ItemArray = (object[]) deserializer.Evaluate(rowExpr);
                table.Rows.Add(dr);
            }
        }

    }
}
