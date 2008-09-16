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
        private object _value;

        public ValueExpression(object value)
        {
            _value = value;
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// The value for the expression
        /// </summary>
        public virtual string StringValue
        {
            get { return (this._value ?? string.Empty).ToString(); }
        }
    }

    /// <summary>
    /// distinguished types for evaluator purposes
    /// </summary>
    [DefaultEvaluator(typeof(NumericEvaluator))]
    public sealed class NumericExpression : ValueExpression
    {
        public NumericExpression(object value)
            : base(value)
        {
        }

        public bool IsFloatingPoint()
        {
            switch (Type.GetTypeCode(this.ResultType))
            {
                case TypeCode.Double:
                case TypeCode.Single:
                case TypeCode.Decimal:
                    return true;
                default:
                    return StringValue.Contains(".");

            }
        }
    }

    /// <summary>
    /// distinguished types for evaluator purposes
    /// </summary>
    [DefaultEvaluator(typeof(BooleanEvaluator))]
    public sealed class BooleanExpression : ValueExpression
    {
        public BooleanExpression(object value)
            : base(value)
        {
        }
    }
}
