using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework
{
    /// <summary>
    /// Writes expressions to the JsonWriter
    /// </summary>
    public class ExpressionWriter
    {
        private Dictionary<Type, WriteMethod> _actions;
        private delegate void WriteMethod(ExpressionBase Expression);
        private IJsonWriter _jsonWriter;
        private SerializationContext _context;

        public ExpressionWriter(IJsonWriter jsonWriter, SerializationContext context)
        {
            _jsonWriter = jsonWriter;
            _context = context;
            InitActions();
        }

        private void InitActions()
        {
            _actions = new Dictionary<Type, WriteMethod>();
            _actions[typeof(BooleanExpression)] = WriteBoolean;
            _actions[typeof(NumericExpression)] = WriteNumeric;
            _actions[typeof(ValueExpression)] = WriteValue;
            _actions[typeof(NullExpression)] = WriteNull;
            _actions[typeof(ListExpression)] = WriteList;
            _actions[typeof(ObjectExpression)] = WriteObject;
            _actions[typeof(ReferenceExpression)] = WriteReference;
            _actions[typeof(CastExpression)] = WriteCast;
        }

        public static void Write(IJsonWriter Writer, SerializationContext context, ExpressionBase Expression)
        {
            new ExpressionWriter(Writer, context).Write(Expression);
        }

        public void Write(ExpressionBase Expression)
        {
            _actions[Expression.GetType()](Expression);
        }

        private void WriteBoolean(ExpressionBase Expression)
        {
            _jsonWriter.Value((bool)((BooleanExpression)Expression).Value);
        }

        private void WriteNumeric(ExpressionBase Expression)
        {
            NumericExpression numeric = (NumericExpression)Expression;
            object value = numeric.Value;
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Double:
                    _jsonWriter.Value((double)value);
                    break;
                case TypeCode.Single:
                    _jsonWriter.Value((float)value);
                    break;
                case TypeCode.Int64:
                    _jsonWriter.Value((long)value);
                    break;
                case TypeCode.Decimal:
                case TypeCode.UInt64:
                    _jsonWriter.SpecialValue(string.Format("{0}", value));
                    break;
                default:
                    _jsonWriter.Value((long)Convert.ChangeType(value, typeof(long)));
                    break;
            }
        }

        private void WriteValue(ExpressionBase Expression)
        {
            ValueExpression value = (ValueExpression)Expression;
            _jsonWriter.QuotedValue(value.StringValue);
        }

        private void WriteNull(ExpressionBase Expression)
        {
            NullExpression n = (NullExpression)Expression;
            _jsonWriter.SpecialValue("null");
        }

        private void WriteList(ExpressionBase Expression)
        {
            ListExpression list = (ListExpression)Expression;
            _jsonWriter.ArrayStart();
            foreach (ExpressionBase item in list.Items)
                Write(item);
            _jsonWriter.ArrayEnd();
        }

        private void WriteObject(ExpressionBase Expression)
        {
            ObjectExpression obj = (ObjectExpression)Expression;
            bool hasConstructor = false;
            if (obj.ConstructorArguments.Count > 0)
            {
                hasConstructor = true;
                _jsonWriter.ConstructorStart(obj.ResultType);
                _jsonWriter.ConstructorArgsStart();
                foreach (ExpressionBase ctorArg in obj.ConstructorArguments)
                {
                    Write(ctorArg);
                }
                _jsonWriter.ConstructorArgsEnd();
            }
            if (!hasConstructor || obj.Properties.Count > 0)
            {
                _jsonWriter.ObjectStart();
                foreach (KeyValueExpression keyValue in obj.Properties)
                {
                    _jsonWriter.Key(keyValue.Key);
                    Write(keyValue.ValueExpression);
                }
                _jsonWriter.ObjectEnd();
            }
            if (hasConstructor)
                _jsonWriter.ConstructorEnd();
        }

        private void WriteReference(ExpressionBase Expression)
        {
            ReferenceExpression refExpr = (ReferenceExpression)Expression;
            _jsonWriter.SpecialValue(refExpr.ReferenceIdentifier.ToString());
        }

        private void WriteCast(ExpressionBase Expression)
        {
            CastExpression cast = (CastExpression)Expression;
            if (_context.OutputTypeInformation)
                _jsonWriter.Cast(cast.ResultType);
            Write(cast.Expression);
        }
    }
}
