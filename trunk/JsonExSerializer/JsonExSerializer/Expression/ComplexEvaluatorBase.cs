using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Expression
{
    /// <summary>
    /// Base class evaluator for complex objects: javascript object and collection
    /// </summary>
    public abstract class ComplexEvaluatorBase : EvaluatorBase
    {
        protected bool _isConstructing;

        public ComplexEvaluatorBase(ExpressionBase expression)
            : base(expression)
        {
        }

        protected override object Construct()
        {
            // in case GetReference is called from one of the constructor arguments
            if (_isConstructing)
                throw new InvalidOperationException("A constructor argument cannot reference the object that it is an argument for");

            _isConstructing = true;
            object[] args = new object[Expression.ConstructorArguments.Count];

            for (int i = 0; i < args.Length; i++)
            {
                ExpressionBase carg = Expression.ConstructorArguments[i];
                args[i] = carg.Evaluate(Context);
            }
            _isConstructing = false;
            return Activator.CreateInstance(Expression.ResultType, args);
        }


        /// <summary>
        /// Complex expression
        /// </summary>
        public new ComplexExpressionBase Expression
        {
            get { return (ComplexExpressionBase)_expression; }
            set { _expression = value; }
        }
    }
}
