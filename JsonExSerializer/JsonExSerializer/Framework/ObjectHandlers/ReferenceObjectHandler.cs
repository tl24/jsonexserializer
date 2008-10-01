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

        public override ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler serializer)
        {
            return serializer.HandleReference(data, CurrentPath);
        }

        public override bool CanHandle(Type ObjectType)
        {
            return false;
        }

        public override bool CanHandle(ExpressionBase expression)
        {
            return (expression is ReferenceExpression);
        }

        public override object Evaluate(ExpressionBase expression, IDeserializerHandler deserializer)
        {
            return ((ReferenceExpression)expression).ReferencedValue;
        }
        public override object Evaluate(ExpressionBase expression, object existingObject, IDeserializerHandler deserializer)
        {
            throw new InvalidOperationException("Cannot update a reference");
        }
    }
}
