using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.ExpressionHandlers;
using JsonExSerializer.Framework.Expressions;

namespace JsonExSerializer.Framework.Parsing
{
    /// <summary>
    /// Evaluates an expression tree to construct the object
    /// </summary>
    public class Evaluator : IDeserializerHandler, IConfigurationAware
    {
        private IConfiguration _config;

        public Evaluator(IConfiguration config)
        {
            this.Config = config;
        }

        /// <summary>
        /// Evaluate the expression and return an object
        /// </summary>
        /// <param name="Expression">expression to evaluate</param>
        /// <returns>the constructed object</returns>
        public object Evaluate(Expression Expression)
        {
            IExpressionHandler handler = Config.ExpressionHandlers.GetHandler(Expression);
            return handler.Evaluate(Expression, this);
        }

        /// <summary>
        /// Evaluates an expression and applies the results to an existing object
        /// </summary>
        /// <param name="Expression">the expression to evaluate</param>
        /// <param name="existingObject">the object to apply to</param>
        /// <returns>the evaluated object</returns>
        public object Evaluate(Expression Expression, object existingObject)
        {
            IExpressionHandler handler = Config.ExpressionHandlers.GetHandler(Expression);
            return handler.Evaluate(Expression, existingObject, this);
        }

        public IConfiguration Config
        {
            get
            {
                return this._config;
            }
            set
            {
                this._config = value;
            }
        }
    }
}
