using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.Expressions;

namespace JsonExSerializer.Framework.ExpressionHandlers
{

    /// <summary>
    /// Defines methods that are used during deserialization to deserialize expressions.
    /// </summary>
    public interface IDeserializerHandler
    {
        /// <summary>
        /// Evaluates an expression converting it into an object
        /// </summary>
        /// <param name="expression">the expression to evaluate</param>
        /// <returns>the object</returns>
        object Evaluate(ExpressionBase expression);

        /// <summary>
        /// Evaluates an expression and populates an existing object
        /// </summary>
        /// <param name="expression">the expression to evaluate</param>
        /// <param name="existingObject">the existing object to populate</param>
        /// <returns>the populated object</returns>
        object Evaluate(ExpressionBase expression, object existingObject);
    }
}
