using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JsonExSerializer.Expression
{
    /// <summary>
    /// Evaluator for Object expressions
    /// </summary>
    public class ObjectEvaluator : ComplexEvaluatorBase
    {

        public ObjectEvaluator(ObjectExpression expression)
            : base(expression)
        {
        }

        protected override object Construct()
        {
            // set the default type if none set
            Expression.SetResultTypeIfNotSet(typeof(Hashtable));
            if (Expression.ConstructorArguments.Count > 0)
            {
                TypeHandler handler = Context.GetTypeHandler(Expression.ResultType);
                if (handler.ConstructorParamaters.Count == Expression.ConstructorArguments.Count)
                {
                    for (int i = 0; i < handler.ConstructorParamaters.Count; i++)
                    {
                        Expression.ConstructorArguments[i].SetResultTypeIfNotSet(handler.ConstructorParamaters[i].PropertyType);
                    }
                }
                else
                {
                    throw new ParseException("Wrong number of constructor arguments for type: " + Expression.ResultType
                        + ", expected: " + handler.ConstructorParamaters.Count + ", actual: " + Expression.ConstructorArguments.Count);
                }
            }
            return base.Construct();
        }
        /// <summary>
        /// Populate the list with its values
        /// </summary>
        protected override void InitializeResult()
        {            
            foreach (KeyValueExpression Item in Expression.Properties)
            {
                // evaluate the item and let it assign itself?
                Item.Evaluate(Context, this._result);
            }
        }

        public new ObjectExpression Expression
        {
            get { return (ObjectExpression)_expression; }
            set { _expression = value; }
        }
    }
}
