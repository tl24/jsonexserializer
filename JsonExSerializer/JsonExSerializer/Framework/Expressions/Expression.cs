/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.Visitors;

namespace JsonExSerializer.Framework.Expressions
{

    public class ObjectConstructedEventArgs : EventArgs
    {
        private object _result;

        public ObjectConstructedEventArgs(object Result)
        {
            _result = Result;
        }

        public object Result
        {
            get { return this._result; }
        }
    }

    /// <summary>
    /// The base class for all expressions
    /// </summary>
    public abstract class Expression : IVisitable
    {

        protected Type _resultType; // the desired type of the result        

        protected Expression()
        {
            _resultType = typeof(object);
        }

        public event EventHandler<ObjectConstructedEventArgs> ObjectConstructed;

        public void OnObjectConstructed(object Result)
        {
            if (ObjectConstructed != null)
            {
                ObjectConstructed(this, new ObjectConstructedEventArgs(Result));
            }
        }

        public virtual Expression Parent { get; set; }

        /// <summary>
        /// Gets or sets the line number where the expression started in the
        /// file or stream it was read from.  Only valid during deserialization.
        /// </summary>
        /// <value>
        /// The line number.
        /// </value>
        public virtual int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the character position on the line where
        /// the expression started in the file or stream it was 
        /// read from.  Only valid during deserialization.
        /// </summary>
        /// <value>
        /// The character position.
        /// </value>
        public virtual int CharacterPosition { get; set; }

        /// <summary>
        /// The type for the evaluated result
        /// </summary>
        public virtual Type ResultType
        {
            get {
                if (this._resultType == null || _resultType == typeof(object))
                    return DefaultType;
                else
                    return this._resultType; 
            }
            set { this._resultType = value; }
        }

        public abstract Type DefaultType { get; }

        public void SetResultTypeIfNotSet(Type newType)
        {
            if (_resultType == null || _resultType == typeof(object))
                _resultType = newType;
        }

        /// <summary>
        /// Accept a visitor to this node
        /// </summary>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
