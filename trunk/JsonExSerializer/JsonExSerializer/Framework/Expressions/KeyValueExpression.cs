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

namespace JsonExSerializer.Framework.Expressions
{
    
    /// <summary>
    /// Key Value pairs in Object Expression.
    /// This class will alter the normal evaluation of the value by loading the propertyinfo
    /// for this key, from the parent and checking for a property converter.
    /// </summary>
    public sealed class KeyValueExpression : Expression {

        public KeyValueExpression(Expression key, Expression value)
        {
            KeyExpression = key;
            ValueExpression = value;
            this.LineNumber = key.LineNumber;
            this.CharacterPosition = key.CharacterPosition;
        }

        public string Key
        {
            get {
                if (!(KeyExpression is ValueExpression))
                    throw new InvalidOperationException("Key property is not valid when key expression is not a ValueExpression");
                return ((ValueExpression)KeyExpression).StringValue; 
            }
        }

        public Expression KeyExpression { get; set; }

        public Expression ValueExpression { get; set; }

        public override Type DefaultType
        {
            get { return typeof(object); }
        }

        public override string ToString()
        {
            return KeyExpression.ToString() + " : " + ValueExpression.ToString();
        }
    } 
}
