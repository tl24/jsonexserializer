using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public class BooleanObjectHandler : ValueObjectHandler
    {
        public BooleanObjectHandler()
        {
        }

        public BooleanObjectHandler(SerializationContext Context)
            : base(Context)
        {
        }

        public override ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler serializer)
        {
            return new BooleanExpression(data);
        }

        public override bool CanHandle(Type ObjectType)
        {
            return (typeof(bool).IsAssignableFrom(ObjectType));
        }
    }
}
