/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JsonExSerializer.Expression
{
    public class ListEvaluator : ComplexEvaluatorBase
    {

        public ListEvaluator(ListExpression expression)
            : base(expression)
        {
        }

        protected override object Construct()
        {
            // set the default type if none set
            Expression.SetResultTypeIfNotSet(typeof(ArrayList));

            return base.Construct();
        }

        /// <summary>
        /// Populate the list with its values
        /// </summary>
        protected override void InitializeResult()
        {
            foreach (ExpressionBase Item in Expression.Items)
            {
                object value = Item.Evaluate(Context);
                ((IList)_result).Add(value);
                // if its an IList...
            }
        }

        /// <summary>
        /// The list expression
        /// </summary>
        public new ListExpression Expression
        {
            get { return (ListExpression) _expression; }
            set { _expression = value; }
        }
    } 
}
