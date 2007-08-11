using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Expression
{
// The base for all expressions

    public abstract class ExpressionBase
    {

        private ExpressionBase _parent;
        protected IEvaluator _evaluator;  //EvaluatorBase
        protected Type _resultType; // the desired type of the result        

        public ExpressionBase()
        {
            _resultType = typeof(object);
        }

        ///<summary>
        /// Returns an object that can be used as a reference.
        /// This could be a partially constructed object, or fully-constructed.
        /// The expression must keep track that this method has been called, and
        /// not create a new object when evaluate is called.
        /// If a reference cannot be created at this point an exception should be thrown
        /// </summary>
        /// <returns>A constructed object</returns>
        public virtual object GetReference(SerializationContext context)
        {
            // allow classes to set their own evaluator if necessary, but
            // generally the factory should be used
            if (_evaluator == null)
            {
                this._evaluator = EvaluatorFactory.GetEvaluator(this, context);
            }
            return _evaluator.GetReference();
        }

        // the result of this operation should be cached in case references are created
        // i.e. subsequent calls to Evaluate should return the exact same object as before
        // (when a reference type is involved)
        public virtual object Evaluate(SerializationContext context)
        {
            // allow classes to set their own evaluator if necessary, but
            // generally the factory should be used
            if (_evaluator == null)
            {
                this._evaluator = EvaluatorFactory.GetEvaluator(this, context);
            }
            return _evaluator.Evaluate();            
        }

        public abstract ExpressionBase ResolveReference(ReferenceIdentifier refID);

        public JsonExSerializer.Expression.ExpressionBase Parent
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


    }
}
