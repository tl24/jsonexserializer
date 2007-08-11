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
