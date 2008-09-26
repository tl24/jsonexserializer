/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using JsonExSerializer.TypeConversion;
using JsonExSerializer.MetaData;

namespace JsonExSerializer.Expression
{
    
    /// <summary>
    /// Key Value pairs in Object Expression.
    /// This class will alter the normal evaluation of the value by loading the propertyinfo
    /// for this key, from the parent and checking for a property converter.
    /// </summary>
    public sealed class KeyValueExpression : ExpressionBase {
        private ExpressionBase _keyExpression;
        private ExpressionBase _valueExpression;

        public KeyValueExpression(ExpressionBase key, ExpressionBase value)
        {
            _keyExpression = key;
            _valueExpression = value;
        }

        public string Key
        {
            get {
                if (!(_keyExpression is ValueExpression))
                    throw new InvalidOperationException("Key property is not valid when key expression is not a ValueExpression");
                return ((ValueExpression)this._keyExpression).StringValue; 
            }
        }

        public ExpressionBase KeyExpression
        {
            get { return this._keyExpression; }
            set { this._keyExpression = value; }
        }

        public ExpressionBase ValueExpression
        {
            get { return this._valueExpression; }
            set { this._valueExpression = value; }
        }

        public override Type DefaultType
        {
            get { return typeof(object); }
        }

    } 
}
