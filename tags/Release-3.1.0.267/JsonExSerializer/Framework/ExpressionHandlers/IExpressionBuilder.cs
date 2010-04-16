using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.Expressions;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer.Framework.ExpressionHandlers
{
    /// <summary>
    /// Defines methods to serialize objects into json expressions
    /// </summary>
    public interface IExpressionBuilder
    {

        /// <summary>
        /// Serialize the object into an expression.
        /// </summary>
        /// <param name="value">the value to serialize</param>
        /// <param name="currentPath">the current path to the value</param>
        /// <returns>a json expression representing the value</returns>
        Expression Serialize(object value, JsonPath currentPath);

        /// <summary>
        /// Serialize an object into an expression using a specific type converter
        /// </summary>
        /// <param name="value">the value to serialize</param>
        /// <param name="currentPath">the current path to the value</param>
        /// <param name="converter">the type converter to use to convert the object</param>
        /// <returns>a json expression representing the value</returns>
        Expression Serialize(object value, JsonPath currentPath, IJsonTypeConverter converter);

        /// <summary>
        /// Indicates that the object can now be referenced.  Any attempts to build a reference to the current object before
        /// this method is called will result in an exception.
        /// </summary>
        /// <param name="value">the object being referenced</param>
        void SetCanReference(object value);
    }
}
