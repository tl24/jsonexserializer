using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Expression
{
    public class NullExpression : ExpressionBase
    {
        public override object Evaluate(SerializationContext context)
        {
            return null;
        }

        public override object GetReference(SerializationContext context)
        {
            throw new Exception("Cannot reference null");
        }

        public override ExpressionBase ResolveReference(ReferenceIdentifier refID)
        {
            throw new Exception("Cannot reference null");
        }
    }
}
