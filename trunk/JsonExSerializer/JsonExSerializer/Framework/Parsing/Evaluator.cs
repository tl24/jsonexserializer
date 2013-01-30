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
        private ISerializerSettings _config;
        private Expression _currentExpression;

        public Evaluator(ISerializerSettings config)
        {
            this.Settings = config;
        }

        /// <summary>
        /// Evaluate the expression and return an object
        /// </summary>
        /// <param name="Expression">expression to evaluate</param>
        /// <returns>the constructed object</returns>
        public object Evaluate(Expression expression)
        {
            Expression oldExpr = Current;
            _currentExpression = expression;
            IExpressionHandler handler = Settings.ExpressionHandlers.GetHandler(expression);
            object result = handler.Evaluate(expression, this);
            _currentExpression = oldExpr ?? _currentExpression;
            return result;

        }

        /// <summary>
        /// Evaluates an expression and applies the results to an existing object
        /// </summary>
        /// <param name="Expression">the expression to evaluate</param>
        /// <param name="existingObject">the object to apply to</param>
        /// <returns>the evaluated object</returns>
        public object Evaluate(Expression expression, object existingObject)
        {
            Expression oldExpr = Current;
            _currentExpression = expression;
            IExpressionHandler handler = Settings.ExpressionHandlers.GetHandler(expression);
            object result = handler.Evaluate(expression, existingObject, this);
            _currentExpression = oldExpr ?? _currentExpression;
            return result;
        }

        public ISerializerSettings Settings
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

        /// <summary>
        /// Gets the current expression being evaluated, mainly for error handling purposes.
        /// </summary>
        public Expression Current
        {
            get { return _currentExpression; }
        }

        public string GetCurrentPath()
        {
            if (Current == null)
                return "";

            try
            {
                return GetPath(Current).ToString();
            }
            catch
            {
                return "";
            }
        }

        private JsonPath GetPath(Expression expression)
        {
            if (expression.Parent == null)
                return new JsonPath();

            Expression parent = GetRealExpression(expression.Parent);

            if (parent is ComplexExpressionBase)
            {
                ComplexExpressionBase complexParent = (ComplexExpressionBase)parent;
                JsonPath path = GetPath(parent);
                if (parent is ArrayExpression)
                {
                    ArrayExpression parentArray = (ArrayExpression)parent;
                    for (int i = 0; i < parentArray.Items.Count; i++)
                    {
                        if (ExpressionEqual(parentArray.Items[i], expression))
                        {
                            return path.Append(i);
                        }
                    }
                }
                else if (parent is ObjectExpression)
                {
                    ObjectExpression parentObject = (ObjectExpression)parent;
                    foreach (KeyValueExpression kve in parentObject.Properties)
                    {
                        if (ExpressionEqual(kve.KeyExpression, expression)
                            || ExpressionEqual(kve.ValueExpression, expression))
                        {
                            return path.Append(kve.Key);
                        }
                    }
                }
                for (int i = 0; i < complexParent.ConstructorArguments.Count; i++)
                {
                    if (ExpressionEqual(complexParent.ConstructorArguments[i], expression))                        
                    {
                        return path.Append("carg" + i);
                    }
                }
            }
            return null;
        }

        private static Expression GetRealExpression(Expression expression)
        {
            while (expression is CastExpression)
            {
                expression = ((CastExpression)expression).Expression;
            }
            return expression;
        }

        private static bool ExpressionEqual(Expression source, Expression target)
        {
            return source == target
                   || source is CastExpression && ((CastExpression)source).Expression == target;
        }
    }
}
