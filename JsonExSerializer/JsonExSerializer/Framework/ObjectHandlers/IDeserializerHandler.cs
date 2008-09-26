using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public interface IDeserializerHandler
    {
        object Evaluate(ExpressionBase Expression);

        object Evaluate(ExpressionBase Expression, object ExistingObject);
    }
}
