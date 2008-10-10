using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.Expressions;

namespace JsonExSerializer.Framework.Parsing
{
    public interface IParsingStage
    {
        ExpressionBase Execute(ExpressionBase root);
    }
}
