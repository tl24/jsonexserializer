using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public class ValueExpressionHandler : ExpressionHandlerBase, IExpressionHandler
    {
        public ValueExpressionHandler()
        {
        }

        public ValueExpressionHandler(SerializationContext context) : base(context)
        {
        }

        public override ExpressionBase GetExpression(object data, JsonPath currentPath, ISerializerHandler serializer)
        {
            return new ValueExpression(data);
        }

        public override object Evaluate(ExpressionBase expression, IDeserializerHandler deserializer)
        {
            ValueExpression value = (ValueExpression)expression;
            if (value.ResultType.IsEnum)
                return Enum.Parse(value.ResultType, value.StringValue);
            else if (value.ResultType == typeof(object))
                return value.StringValue;
            else if (value.ResultType == typeof(string))
                return value.StringValue;
            else
                return Convert.ChangeType(value.StringValue, value.ResultType);
        }

        public override object Evaluate(ExpressionBase expression, object existingObject, IDeserializerHandler deserializer)
        {
            throw new InvalidOperationException("Value types can not be updated");
        }

        public override bool CanHandle(Type ObjectType)
        {
            return (typeof(string).IsAssignableFrom(ObjectType)
                || typeof(char).IsAssignableFrom(ObjectType)
                || typeof(DateTime).IsAssignableFrom(ObjectType));
        }
    }
}
