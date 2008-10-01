using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public class CastObjectHandler : ObjectHandlerBase
    {
        public CastObjectHandler()
        {
        }

        public CastObjectHandler(SerializationContext Context)
            : base(Context)
        {
        }

        public override ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler serializer)
        {
            throw new Exception("CastObjectHandler should not be called during Serialization");
        }

        public override bool CanHandle(Type ObjectType)
        {
            return false;
        }

        public override bool CanHandle(ExpressionBase Expression)
        {
            return (Expression is CastExpression);
        }

        public override object Evaluate(ExpressionBase Expression, IDeserializerHandler deserializer)
        {
            ExpressionBase innerExpression = ((CastExpression)Expression).Expression;
            innerExpression.ResultType = Expression.ResultType;
            return deserializer.Evaluate(innerExpression);
        }

        public override object Evaluate(ExpressionBase Expression, object existingObject, IDeserializerHandler deserializer)
        {
            ExpressionBase innerExpression = ((CastExpression)Expression).Expression;
            innerExpression.ResultType = Expression.ResultType;
            return deserializer.Evaluate(innerExpression, existingObject);
        }
    }
}