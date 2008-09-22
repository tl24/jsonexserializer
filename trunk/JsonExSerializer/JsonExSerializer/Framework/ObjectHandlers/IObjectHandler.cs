using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public interface IObjectHandler
    {
        ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler Serializer);
        bool CanHandle(object Data);
    }
}
