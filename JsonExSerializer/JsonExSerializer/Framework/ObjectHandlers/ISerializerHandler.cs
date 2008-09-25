using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public interface ISerializerHandler
    {
        ExpressionBase Serialize(object Value, JsonPath CurrentPath);

        ExpressionBase Serialize(object Value, JsonPath CurrentPath, IJsonTypeConverter Converter);

        /// <summary>
        /// Checks for previous references to this object and acts accordingly based on the Reference options.
        /// If an expression is returned, then further evaluation of the object should stop.  If no expression
        /// is returned then the object should be serialized normally.
        /// </summary>
        /// <param name="referenceValue"></param>
        /// <param name="CurrentPath"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Throws InvalidOperationException if there is a circular reference or an object
        /// is not in a referencable state</exception>
        ExpressionBase HandleReference(object ReferenceValue, JsonPath CurrentPath);

        /// <summary>
        /// Indicates that the object can now be referenced.  Any attempts to build a reference to the current object before
        /// this method is called will result in an exception.
        /// </summary>
        /// <param name="value">the object being referenced</param>
        void SetCanReference(object Value);
    }
}
