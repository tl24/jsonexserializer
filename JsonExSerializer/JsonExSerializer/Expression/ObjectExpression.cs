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
    /// <summary>
    /// Represents a javascript object
    /// </summary>
    public sealed class ObjectExpression : ComplexExpressionBase {

        private IList<KeyValueExpression> _properties;
        private bool _isDictionary = false;
        private Type _dictionaryKeyType = typeof(string);
        private Type _dictionaryValueType = typeof(object);

        public ObjectExpression()
        {
            _properties = new List<KeyValueExpression>();
            this.ObjectConstructed += new EventHandler<ObjectConstructedEventArgs>(ObjectExpression_ObjectConstructed);
            _resultType = typeof(Hashtable);
        }

        public override Type DefaultType
        {
            get { return typeof(Hashtable); }
        }

        void ObjectExpression_ObjectConstructed(object sender, ObjectConstructedEventArgs e)
        {
            if (ResultType.GetInterface(typeof(IDictionary).FullName) != null)
            {
                _isDictionary = true;
                Type genDict = ResultType.GetInterface(typeof(IDictionary<,>).Name);
                // attempt to figure out what the types of the values are, if no type is set already
                if (genDict != null)
                {
                    Type[] genArgs = genDict.GetGenericArguments();
                    _dictionaryKeyType = genArgs[0];
                    _dictionaryValueType = genArgs[1];
                }
            }
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

        /// <summary>
        /// Add a property to this object
        /// </summary>
        /// <param name="key">the key for the property</param>
        /// <param name="value">the value for the property</param>
        /// <returns>KeyValueExpression that was added</returns>
        public KeyValueExpression Add(string key, ExpressionBase value)
        {
            return Add(new ValueExpression(key), value);
        }

        public bool IsDictionary
        {
            get { return this._isDictionary; }
        }

        public System.Type DictionaryKeyType
        {
            get { return this._dictionaryKeyType; }
        }

        public System.Type DictionaryValueType
        {
            get { return this._dictionaryValueType; }
        }
    } 
}
