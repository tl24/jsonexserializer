using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Expression
{
    /// <summary>
    /// Base class for evaluators
    /// </summary>
    public abstract class EvaluatorBase : IEvaluator {

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

        /// <summary>
        /// Gets a reference to the final object.  The reference does not
        /// need to be fully initialized
        /// </summary>
        /// <returns>object reference</returns>
        public virtual object GetReference()
        {
            if (_result == null) {
               _result = Construct();
            }
            return _result;
        }

        protected abstract object Construct();  // constructs the object
        protected abstract void InitializeResult();  // initializes the result..populates properties, etc

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
                }
                InitializeResult();
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


    }
}
