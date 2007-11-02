/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Expression
{
    /// <summary>
    /// Value types such as string, bool, or number
    /// </summary>
    [DefaultEvaluator(typeof(ValueEvaluator))]
    public class ValueExpression: ExpressionBase
    {
        private string _value;

        public ValueExpression(string value)
        {
            _value = value;
        }

        /// <summary>
        /// The value for the expression
        /// </summary>
        public virtual string Value
        {
            get { return this._value; }
            set { this._value = value; }
        }

        public override ExpressionBase ResolveReference(ReferenceIdentifier refID)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    /// <summary>
    /// distinguished types for evaluator purposes
    /// </summary>
    [DefaultEvaluator(typeof(NumericEvaluator))]
    public class NumericExpression : ValueExpression
    {
        public NumericExpression(string value)
            : base(value)
        {
        }           
    }

    /// <summary>
    /// distinguished types for evaluator purposes
    /// </summary>
    [DefaultEvaluator(typeof(BooleanEvaluator))]
    public class BooleanExpression : ValueExpression
    {
        public BooleanExpression(string value)
            : base(value)
        {
        }
    }
}
