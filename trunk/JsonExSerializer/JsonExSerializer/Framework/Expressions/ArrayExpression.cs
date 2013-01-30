/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JsonExSerializer.Framework.Expressions
{
    /// <summary>
    /// Expression to represent a javascript List/Array
    /// </summary>
    public sealed class ArrayExpression : ComplexExpressionBase {

        public ArrayExpression()
        {
            Items = new List<Expression>();
            _resultType = typeof(ArrayList);
        }

        public override Type DefaultType
        {
            get { return typeof(ArrayList); }
        }

        public IList<Expression> Items { get; set; }

        public void Add(Expression item)
        {
            Items.Add(item);
            item.Parent = this;
        }

        public override string ToString()
        {
            return "[Array(" + Items.Count + ")]";
        }
    }
}
