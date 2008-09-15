/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
namespace JsonExSerializer.Expression
{
    public interface IEvaluator : IContextAware
    {

        void SetResult(object value);

        /// <summary>
        /// Fully evaluates the expression for this evaluator
        /// </summary>
        /// <returns>evaluated result</returns>
        object Evaluate();

        ExpressionBase Expression { get; set; }
    }
}
