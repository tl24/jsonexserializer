using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Framework.Expressions
{
    public class CastExpression : ExpressionBase
    {
        private ExpressionBase _expression;

        public CastExpression(Type CastedType)
        {
            _resultType = CastedType;
        }

        public CastExpression(Type CastedType, ExpressionBase Expression)
            : this(CastedType)
        {
            _expression = Expression;
        }

        public override Type ResultType
        {
            get { return base.ResultType; }
            set
            {
                ; // ignore this 
            }
        }

        public override Type DefaultType
        {
            get { return typeof(object); }
        }
        public ExpressionBase Expression
        {
            get { return this._expression; }
            set { this._expression = value; }
        }

        public override ExpressionBase Parent
        {
            get { return base.Parent; }
            set
            {
                base.Parent = value;
                if (Expression != null)
                    Expression.Parent = value;
            }
        }
    }
}
