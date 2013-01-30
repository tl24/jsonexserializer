using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Framework.Expressions
{
    public class CastExpression : Expression
    {
        public CastExpression(Type CastedType)
        {
            _resultType = CastedType;
        }

        public CastExpression(Type castedType, Expression expression)
            : this(castedType)
        {
            Expression = expression;

        }

        public override Type ResultType
        {
            get { return base.ResultType; }
            set
            {
                ; // ignore this 
            }
        }

        public override int LineNumber
        {
            get
            {
                if (Expression != null)
                    return Expression.LineNumber;
                else
                    return base.LineNumber;
            }
            set
            {
                base.LineNumber = value;
            }
        }

        public override int CharacterPosition
        {
            get
            {
                if (Expression != null)
                    return Expression.CharacterPosition;
                else
                    return base.CharacterPosition;
            }
            set
            {
                base.CharacterPosition = value;
            }
        }

        public override Type DefaultType
        {
            get { return typeof(object); }
        }

        public Expression Expression { get; set; }

        public override Expression Parent
        {
            get { return base.Parent; }
            set
            {
                base.Parent = value;
                if (Expression != null)
                    Expression.Parent = value;
            }
        }

        public override string ToString()
        {
            return "(" + ResultType.GetType().FullName + ") " + Expression.ToString();
        }
    }
}
