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
    /// Expression to represent a javascript List/Array
    /// </summary>
    public sealed class ArrayExpression : ComplexExpressionBase {
        private IList<ExpressionBase> _items;

        public ArrayExpression()
        {
            _items = new List<ExpressionBase>();
            _resultType = typeof(ArrayList);
        }

        public override Type DefaultType
        {
            get { return typeof(ArrayList); }
        }
        public IList<ExpressionBase> Items
        {
            get { return this._items; }
            set { this._items = value; }
        }

        public void Add(ExpressionBase item)
        {
            _items.Add(item);
            item.Parent = this;
        }
    }
}
