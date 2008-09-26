using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public class ReferenceObjectHandler : ObjectHandlerBase
    {

        public ReferenceObjectHandler()
        {
        }

        public ReferenceObjectHandler(SerializationContext Context)
            : base(Context)
        {
        }

        public override ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler Serializer)
        {
            return Serializer.HandleReference(data, CurrentPath);
        }

        public override bool CanHandle(Type ObjectType)
        {
            return false;
        }

        public override bool CanHandle(ExpressionBase Expression)
        {
            return (Expression is ReferenceExpression);
        }

        public override object Evaluate(ExpressionBase Expression, IDeserializerHandler Deserializer)
        {
            return ((ReferenceExpression)Expression).ReferencedValue;
        }
        public override object Evaluate(ExpressionBase Expression, object ExistingObject, IDeserializerHandler Deserializer)
        {
            throw new InvalidOperationException("Cannot update a reference");
        }
    }
}
