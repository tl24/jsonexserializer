using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public interface ISerializerHandler
    {
        ExpressionBase Serialize(object o, JsonPath currentPath);

        ExpressionBase Serialize(object o, JsonPath currentPath, IJsonTypeConverter converter);
    }
}
