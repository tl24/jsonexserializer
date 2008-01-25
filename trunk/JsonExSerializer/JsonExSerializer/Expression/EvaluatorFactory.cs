/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;
using JsonExSerializer.MetaData;

namespace JsonExSerializer.Expression
{

    [global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class DefaultEvaluatorAttribute : Attribute
    {
        private readonly Type _evaluatorType;

        /// <summary>
        /// The type of the evaluator for this expression
        /// </summary>
        /// <param name="evaluatorType"></param>
        public DefaultEvaluatorAttribute(Type evaluatorType)
        {
            this._evaluatorType = evaluatorType;
        }

        public System.Type EvaluatorType
        {
            get { return this._evaluatorType; }
        }

    }

    public class EvaluatorFactory
    {
        public static IEvaluator GetEvaluator(ExpressionBase expression, SerializationContext context)
        {
            Type evaluatorType = null;
            Type expType = expression.GetType();
            IEvaluator evaluator = null;
            ITypeHandler handler = context.GetTypeHandler(expression.ResultType);
            if (expType.IsDefined(typeof(DefaultEvaluatorAttribute), false))
            {
                DefaultEvaluatorAttribute attr = (DefaultEvaluatorAttribute)expType.GetCustomAttributes(typeof(DefaultEvaluatorAttribute), false)[0];
                evaluatorType = attr.EvaluatorType;
                evaluator = (IEvaluator) Activator.CreateInstance(evaluatorType, expression);                
            } else if (expression is ListExpression) {                
                if (handler.IsCollection())
                {
                    evaluator = new CollectionBuilderEvaluator(expression);
                }
            }
            if (evaluator != null)
            {
                evaluator.Context = context;
                if (typeof(IJsonTypeConverter).IsAssignableFrom(expression.ResultType)) {
                    IEvaluator defaultEvaluator = evaluator;
                    // pass null for converter because object itself is the converter
                    evaluator = new ConverterEvaluator(expression, defaultEvaluator, null);
                    evaluator.Context = context;
                }
                else if (handler.HasConverter)
                {
                    IEvaluator defaultEvaluator = evaluator;
                    evaluator = new ConverterEvaluator(expression, defaultEvaluator, handler.TypeConverter);
                    evaluator.Context = context;
                }
                return evaluator;
            }
            else
            {
                throw new Exception("No suitable evaluator found for expression type: " + expression.GetType().FullName);
            }
        }
    }
}
