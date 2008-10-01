using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework
{
    public interface IParsingStage
    {
        ExpressionBase Execute(ExpressionBase root);
    }
}
