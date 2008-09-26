using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public class ValueObjectHandler : ObjectHandlerBase, IObjectHandler
    {
        private SerializationContext _context;
        public ValueObjectHandler()
        {
        }

        public ValueObjectHandler(SerializationContext Context)
        {
            this.Context = Context;
        }

        public override ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler Serializer)
        {
            return new ValueExpression(data);
        }

        public override object Evaluate(ExpressionBase Expression, IDeserializerHandler Deserializer)
        {
            ValueExpression value = (ValueExpression)Expression;
            if (value.ResultType.IsEnum)
                return Enum.Parse(value.ResultType, value.StringValue);
            else if (value.ResultType == typeof(object))
                return value.StringValue;
            else if (value.ResultType == typeof(string))
                return value.StringValue;
            else
                return Convert.ChangeType(value.StringValue, value.ResultType);
        }

        public override object Evaluate(ExpressionBase Expression, object ExistingObject, IDeserializerHandler Deserializer)
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
