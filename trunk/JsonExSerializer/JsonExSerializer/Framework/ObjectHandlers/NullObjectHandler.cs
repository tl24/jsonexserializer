using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public class NullObjectHandler : ObjectHandlerBase
    {
        public override ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler serializer)
        {
            return new NullExpression();
        }

        public override bool CanHandle(Type objectType)
        {
            return false;
        }

        public override bool CanHandle(ExpressionBase expression)
        {
            return (expression is NullExpression);
        }

        public override object Evaluate(ExpressionBase expression, IDeserializerHandler deserializer)
        {
            if (!(expression is NullExpression))
                throw new ArgumentException("expression should be NullExpression");
            return null;
        }

        public override object Evaluate(ExpressionBase expression, object existingObject, IDeserializerHandler deserializer)
        {
            return existingObject;
        }
    }
}
