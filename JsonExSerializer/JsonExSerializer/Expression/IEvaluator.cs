/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
namespace JsonExSerializer.Expression
{
    public interface IEvaluator
    {
        /// <summary>
        /// Fully evaluates the expression for this evaluator
        /// </summary>
        /// <returns>evaluated result</returns>
        object Evaluate();

        /// <summary>
        /// Gets a reference to the expression result if the expression
        /// supports references at the time this is called
        /// </summary>
        /// <returns>object to reference</returns>
        object GetReference();

        ExpressionBase Expression { get; set; }

        SerializationContext Context { get; set; }
    }
}
