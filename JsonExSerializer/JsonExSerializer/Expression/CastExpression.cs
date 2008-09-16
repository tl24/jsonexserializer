using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Expression
{
    public class CastExpression : ExpressionBase
    {
        private ExpressionBase _expression;

        public CastExpression(Type CastedType)
        {
            _resultType = CastedType;
        }

        public override Type ResultType
        {
            get {  return base.ResultType;  }
            set {  ; // ignore this 
            }
        }

        public ExpressionBase Expression
        {
            get { return this._expression; }
            set { this._expression = value; }
        }

        public override ExpressionBase Parent
        {
            get {  return base.Parent; }
            set
            {
                base.Parent = value;
                if (Expression != null)
                    Expression.Parent = value;
            }
        }

        public override object Evaluate(SerializationContext Context)
        {
            Expression.ResultType = this.ResultType;
            return Expression.Evaluate(Context);
        }

        public override IEvaluator GetEvaluator(SerializationContext Context)
        {
            //TODO: Some places call GetEvaluator directly, need to clean that up
            Expression.ResultType = this.ResultType;
            return Expression.GetEvaluator(Context);
        }
    }
}
