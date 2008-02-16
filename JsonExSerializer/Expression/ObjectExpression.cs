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
    /// Represents a javascript object
    /// </summary>
    [DefaultEvaluator(typeof(ObjectEvaluator))]
    public sealed class ObjectExpression : ComplexExpressionBase {

        private IList<KeyValueExpression> _properties;

        public ObjectExpression()
        {
            _properties = new List<KeyValueExpression>();
        }
        /// <summary>
        /// The object's properties
        /// </summary>
        public IList<KeyValueExpression> Properties
        {
            get { return this._properties; }
            set { this._properties = value; }
        }

        /// <summary>
        /// Add a property to this object
        /// </summary>
        /// <param name="key">the key for the property</param>
        /// <param name="value">the value for the property</param>
        /// <returns>KeyValueExpression that was added</returns>
        public KeyValueExpression Add(ExpressionBase key, ExpressionBase value)
        {
            return Add(new KeyValueExpression(key, value));
        }

        /// <summary>
        /// Add a property to this object
        /// </summary>
        /// <param name="expression">the key value expression to add</param>
        /// <returns>KeyValueExpression that was added</returns>
        public KeyValueExpression Add(KeyValueExpression expression)
        {
            expression.Parent = this;
            expression.ValueExpression.Parent = this;
            Properties.Add(expression);
            return expression;
        }

        protected override ExpressionBase ResolveChildReference(ReferenceIdentifier refID)
        {
            string key = refID.Current;
            foreach (KeyValueExpression exp in Properties)
            {
                if (exp.Key == key)
                {
                    return exp.ResolveReference(refID);
                }
            }
            // if we get here we didn't find it
            throw new Exception("Unable to resolve reference: " + refID);
        }

    } 
}
