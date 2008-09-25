using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public class NullObjectHandler : ObjectHandlerBase
    {
        public override ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler Serializer)
        {
            return new NullExpression();
        }

        public override bool CanHandle(Type ObjectType)
        {
            return true;
        }
    }
}
