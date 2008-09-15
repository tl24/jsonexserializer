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
    /// Base class for evaluators
    /// </summary>
    abstract class EvaluatorBase : IEvaluator {

        // the expression to evaluate;
        protected ExpressionBase _expression;
        protected SerializationContext _context;

        // the result (cached for repeated calls)
        protected object _result;  

        // true if the expression is fully evaluated, or false for only partially created
        protected bool _isFullyEvaluated; 
   
        public EvaluatorBase(ExpressionBase expression) {
            _expression = expression;
        }

        protected abstract object Construct();  // constructs the object
        protected abstract void UpdateResult();  // initializes the result..populates properties, etc

        /// <summary>
        /// Evaluate the expression and return the result
        /// </summary>
        /// <returns>the result of the evaluation</returns>
        public virtual object Evaluate()
        {
            if (!_isFullyEvaluated)
            {
                if (_result == null)
                {
                    _result = Construct();
                    this.Expression.OnObjectConstructed(_result);
                }
                UpdateResult();
                IDeserializationCallback callback = _result as IDeserializationCallback;
                if (callback != null)
                    callback.OnAfterDeserialization();

                _isFullyEvaluated = true;
            }
            return _result;
        }


        /// <summary>
        /// The expression being evaluated
        /// </summary>
        public ExpressionBase Expression
        {
            get { return this._expression; }
            set { this._expression = value; }
        }

        public SerializationContext Context
        {
            get { return this._context; }
            set { this._context = value; }
        }

        public void SetResult(object value)
        {
            if (_result != null && value != null && !object.ReferenceEquals(_result, value))
            {
                throw new InvalidOperationException("Can't update result, the result has already been set or the object has been evaluated already.");
            }
            _result = value;
        }


    }
}
