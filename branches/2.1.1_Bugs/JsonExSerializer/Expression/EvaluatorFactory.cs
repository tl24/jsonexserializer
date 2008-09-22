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

    class EvaluatorFactory
    {
        private static Dictionary<Type, EvalCtor> _cache = new Dictionary<Type, EvalCtor>();

        private delegate IEvaluator EvalCtor(ExpressionBase expression);
        static EvaluatorFactory()
        {
            _cache[typeof(ObjectExpression)] = delegate (ExpressionBase e) { return new ObjectEvaluator((ObjectExpression) e); };
            _cache[typeof(ValueExpression)] = delegate(ExpressionBase e) { return new ValueEvaluator((ValueExpression)e); };
            _cache[typeof(NumericExpression)] = delegate(ExpressionBase e) { return new ValueEvaluator((NumericExpression)e); };
            _cache[typeof(BooleanExpression)] = delegate(ExpressionBase e) { return new ValueEvaluator((BooleanExpression)e); };
        }
        public static IEvaluator GetEvaluator(ExpressionBase expression, SerializationContext context)
        {
            Type evaluatorType = null;
            Type expType = expression.GetType();
            IEvaluator evaluator = null;
            TypeHandler handler = context.GetTypeHandler(expression.ResultType);
            evaluator = GetDefaultEvaluator(expression);          
            if (evaluator == null && expression is ListExpression && handler.IsCollection())
                evaluator = new CollectionBuilderEvaluator(expression);

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

        private static IEvaluator GetDefaultEvaluator(ExpressionBase expression)
        {
            Type expType = expression.GetType();
            EvalCtor evaluatorConstructor = null;
            if (_cache.TryGetValue(expType, out evaluatorConstructor))
                return evaluatorConstructor(expression);
            else
                return null;
        }
    }
}
