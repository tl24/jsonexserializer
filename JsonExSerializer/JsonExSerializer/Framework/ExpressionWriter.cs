using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.Expressions;
using System.Globalization;

namespace JsonExSerializer.Framework
{
    /// <summary>
    /// Writes expressions to the JsonWriter
    /// </summary>
    public class ExpressionWriter
    {    
        private Dictionary<Type, Action<Expression>> actions;
        private IJsonWriter jsonWriter;
        private bool outputTypeInfo;

        public ExpressionWriter(IJsonWriter jsonWriter, bool outputTypeInfo)
        {
            this.jsonWriter = jsonWriter;
            this.outputTypeInfo = outputTypeInfo;
            InitActions();
        }

        private void InitActions()
        {
            this.actions = new Dictionary<Type, Action<Expression>>();
            this.actions[typeof(BooleanExpression)] = WriteBoolean;
            this.actions[typeof(NumericExpression)] = WriteNumeric;
            this.actions[typeof(ValueExpression)] = WriteValue;
            this.actions[typeof(NullExpression)] = WriteNull;
            this.actions[typeof(ArrayExpression)] = WriteList;
            this.actions[typeof(ObjectExpression)] = WriteObject;
            this.actions[typeof(ReferenceExpression)] = WriteReference;
            this.actions[typeof(CastExpression)] = WriteCast;
        }

        public bool OutputTypeInfo
        {
            get { return this.outputTypeInfo; }
            set { this.outputTypeInfo = value; }
        }

        public static void Write(IJsonWriter writer, bool outputTypeInfo, Expression expression)
        {
            new ExpressionWriter(writer, outputTypeInfo).Write(expression);
        }

        public virtual void Write(Expression expression)
        {
            this.actions[expression.GetType()](expression);
        }

        protected virtual void WriteBoolean(Expression expression)
        {
            this.jsonWriter.WriteValue((bool)((BooleanExpression)expression).Value);
        }

        protected virtual void WriteNumeric(Expression expression)
        {
            NumericExpression numeric = (NumericExpression)expression;
            object value = numeric.Value;
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Double:
                    jsonWriter.WriteValue((double)value);
                    break;
                case TypeCode.Single:
                    jsonWriter.WriteValue((float)value);
                    break;
                case TypeCode.Int64:
                    jsonWriter.WriteValue((long)value);
                    break;
                case TypeCode.Decimal:
                    jsonWriter.WriteValue((decimal)value);
                    break;
                case TypeCode.UInt64:
                    jsonWriter.WriteSpecialValue(string.Format(CultureInfo.InvariantCulture, "{0}", value));
                    break;
                default:
                    jsonWriter.WriteValue((long)Convert.ChangeType(value, typeof(long), CultureInfo.InvariantCulture));
                    break;
            }
        }

        protected virtual void WriteValue(Expression expression)
        {
            ValueExpression value = (ValueExpression)expression;
            jsonWriter.WriteQuotedValue(value.StringValue);
        }

        protected virtual void WriteNull(Expression expression)
        {
            if (!(expression is NullExpression))
                throw new ArgumentException("Expression should be a NullExpression");
            jsonWriter.WriteSpecialValue("null");
        }

        protected virtual void WriteList(Expression Expression)
        {
            ArrayExpression list = (ArrayExpression)Expression;
            jsonWriter.WriteArrayStart();
            foreach (Expression item in list.Items)
                Write(item);
            jsonWriter.WriteArrayEnd();
        }

        protected virtual void WriteObject(Expression Expression)
        {
            ObjectExpression obj = (ObjectExpression)Expression;
            bool hasConstructor = false;
            if (obj.ConstructorArguments.Count > 0)
            {
                hasConstructor = true;
                jsonWriter.WriteConstructorStart(obj.ResultType);
                jsonWriter.WriteConstructorArgsStart();
                foreach (Expression ctorArg in obj.ConstructorArguments)
                {
                    Write(ctorArg);
                }
                jsonWriter.WriteConstructorArgsEnd();
            }
            if (!hasConstructor || obj.Properties.Count > 0)
            {
                jsonWriter.WriteObjectStart();
                foreach (KeyValueExpression keyValue in obj.Properties)
                {
                    jsonWriter.WriteKey(keyValue.Key);
                    Write(keyValue.ValueExpression);
                }
                jsonWriter.WriteObjectEnd();
            }
            if (hasConstructor)
                jsonWriter.WriteConstructorEnd();
        }

        protected virtual void WriteReference(Expression Expression)
        {
            ReferenceExpression refExpr = (ReferenceExpression)Expression;
            jsonWriter.WriteSpecialValue(refExpr.Path.ToString());
        }

        protected virtual void WriteCast(Expression Expression)
        {
            CastExpression cast = (CastExpression)Expression;
            if (outputTypeInfo && !(cast.Expression is ReferenceExpression))
                jsonWriter.WriteCast(cast.ResultType);
            Write(cast.Expression);
        }
    }
}
