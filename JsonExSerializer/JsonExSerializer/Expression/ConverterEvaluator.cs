/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer.Expression
{
    /// <summary>
    /// Evaluator for types with converters
    /// </summary>
    class ConverterEvaluator : IEvaluator
    {
        protected ExpressionBase _expression;
        protected SerializationContext _context;
        private IEvaluator _defaultEvaluator;
        private IJsonTypeConverter _converter;
        private bool _isEvaluating = false;
        // the result (cached for repeated calls)
        protected object _result;  


        public ConverterEvaluator(ExpressionBase expression, IEvaluator defaultEvaluator, IJsonTypeConverter converter)
        {
            _expression = expression;
            _defaultEvaluator = defaultEvaluator;
            _converter = converter;
        }

        public virtual object Evaluate()
        {
            if (_result == null)
            {
                Type sourceType = Expression.ResultType;
                if (typeof(IJsonTypeConverter).IsAssignableFrom(sourceType))
                {
                    _converter = (IJsonTypeConverter) Activator.CreateInstance(sourceType);
                }
                Type destType = _converter.GetSerializedType(sourceType);
                Expression.ResultType = destType;
                _isEvaluating = true;
                object tempResult = _defaultEvaluator.Evaluate();
                if (typeof(IJsonTypeConverter).IsAssignableFrom(sourceType))
                {

                    _result = _converter.ConvertTo(tempResult, sourceType, Context);
                }
                else
                {
                    _result = _converter.ConvertTo(tempResult, sourceType, Context);
                }
                Expression.OnObjectConstructed(_result);
                if (_result is IDeserializationCallback)
                {
                    ((IDeserializationCallback)_result).OnAfterDeserialization();
                }
                _isEvaluating = false;
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

        public JsonExSerializer.Expression.IEvaluator DefaultEvaluator
        {
            get { return this._defaultEvaluator; }
            set { this._defaultEvaluator = value; }
        }

        public JsonExSerializer.TypeConversion.IJsonTypeConverter Converter
        {
            get { return this._converter; }
            set { this._converter = value; }
        }

        public void SetResult(object value)
        {
            throw new Exception("The method or operation is not supported.");
        }

    }
}
