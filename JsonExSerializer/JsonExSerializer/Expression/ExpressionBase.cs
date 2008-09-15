/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.Visitors;

namespace JsonExSerializer.Expression
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
    public abstract class ExpressionBase
    {

        private ExpressionBase _parent;
        protected IEvaluator _evaluator;  //EvaluatorBase
        protected Type _resultType; // the desired type of the result        

        public ExpressionBase()
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

        // the result of this operation should be cached in case references are created
        // i.e. subsequent calls to Evaluate should return the exact same object as before
        // (when a reference type is involved)
        public virtual object Evaluate(SerializationContext Context)
        {
            // allow classes to set their own evaluator if necessary, but
            // generally the factory should be used
            return GetEvaluator(Context).Evaluate();            
        }

        public virtual IEvaluator GetEvaluator(SerializationContext Context) {
            if (_evaluator == null)
            {
                this._evaluator = EvaluatorFactory.GetEvaluator(this, Context);
            }
            return this._evaluator;
        }

        public virtual ExpressionBase Parent
        {
            get { return this._parent; }
            set { this._parent = value; }
        }

        /// <summary>
        /// The type for the evaluated result
        /// </summary>
        public System.Type ResultType
        {
            get { return this._resultType; }
            set { this._resultType = value; }
        }

        public void SetResultTypeIfNotSet(Type newType)
        {
            if (_resultType == null || _resultType == typeof(object))
                _resultType = newType;
        }

        /// <summary>
        /// The evaluator for this expression, if this is null when Evaluate is called
        /// then a default evaluator will be looked up using the EvaluatorFactory.
        /// </summary>
        public IEvaluator Evaluator
        {
            get { return this._evaluator; }
            set { this._evaluator = value; }
        }

        /// <summary>
        /// Accept a visitor to this node
        /// </summary>
        public void Accept(VisitorBase visitor)
        {
            visitor.Visit(this);
        }
    }
}
