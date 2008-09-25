using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public class BooleanObjectHandler : ObjectHandlerBase
    {
        public BooleanObjectHandler()
        {
        }

        public BooleanObjectHandler(SerializationContext Context)
            : base(Context)
        {
        }

        public override ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler Serializer)
        {
            return new BooleanExpression(data);
        }

        public override bool CanHandle(Type ObjectType)
        {
            return (typeof(bool).IsAssignableFrom(ObjectType));
        }
    }
}
