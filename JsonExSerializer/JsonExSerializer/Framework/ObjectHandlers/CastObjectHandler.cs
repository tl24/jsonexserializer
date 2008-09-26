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

        public override ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler Serializer)
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

        public override object Evaluate(ExpressionBase Expression, IDeserializerHandler Deserializer)
        {
            ExpressionBase innerExpression = ((CastExpression)Expression).Expression;
            innerExpression.ResultType = Expression.ResultType;
            return Deserializer.Evaluate(innerExpression);
        }

        public override object Evaluate(ExpressionBase Expression, object ExistingObject, IDeserializerHandler Deserializer)
        {
            ExpressionBase innerExpression = ((CastExpression)Expression).Expression;
            innerExpression.ResultType = Expression.ResultType;
            return Deserializer.Evaluate(innerExpression, ExistingObject);
        }
    }
}