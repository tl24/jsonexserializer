/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using JsonExSerializer.TypeConversion;
using JsonExSerializer.MetaData;

namespace JsonExSerializer.Expression
{
    
    /// <summary>
    /// Key Value pairs in Object Expression.
    /// This class will alter the normal evaluation of the value by loading the propertyinfo
    /// for this key, from the parent and checking for a property converter.
    /// </summary>
    public sealed class KeyValueExpression : ExpressionBase {
        private ExpressionBase _keyExpression;
        private ExpressionBase _valueExpression;
        private object parentResult;

        public KeyValueExpression(ExpressionBase key, ExpressionBase value)
        {
            _keyExpression = key;
            _valueExpression = value;
        }

        public string Key
        {
            get {
                if (!(_keyExpression is ValueExpression))
                    throw new InvalidOperationException("Key property is not valid when key expression is not a ValueExpression");
                return ((ValueExpression)this._keyExpression).StringValue; 
            }
        }

        public ExpressionBase KeyExpression
        {
            get { return this._keyExpression; }
            set { this._keyExpression = value; }
        }

        public ExpressionBase ValueExpression
        {
            get { return this._valueExpression; }
            set { this._valueExpression = value; }
        }

        public override ExpressionBase Parent
        {
            get
            {
                return base.Parent;
            }
            set
            {
                base.Parent = value;
                value.ObjectConstructed += new EventHandler<ObjectConstructedEventArgs>(parent_ObjectConstructed);
            }
        }

        void parent_ObjectConstructed(object sender, ObjectConstructedEventArgs e)
        {
            parentResult = e.Result;
        }

        public override object Evaluate(SerializationContext context)
        {
            if (parentResult == null)
                throw new InvalidOperationException("Unabled to evaluate expression, parent object has not been evaluated yet");
            if (((ObjectExpression)Parent).IsDictionary)
            {
                return EvaluateDictionaryItem(context);
            }
            else
            {
                return EvaluateObjectProperty(context);
            }
        }

        public object EvaluateDictionaryItem(SerializationContext context)
        {
            // if no type set, set one
            KeyExpression.ResultType = ((ObjectExpression)Parent).DictionaryKeyType;
            ValueExpression.ResultType = ((ObjectExpression)Parent).DictionaryValueType;
            object keyObject = KeyExpression.Evaluate(context);
            object result = ValueExpression.Evaluate(context);
            ((IDictionary)parentResult)[keyObject] = result;
            return result;
        }

        public object EvaluateObjectProperty(SerializationContext context)
        {
            // lookup info for the type
            IPropertyHandler hndlr = context.GetTypeHandler(parentResult.GetType()).FindProperty(Key);
            if (hndlr == null)
            {
                throw new Exception(string.Format("Could not find property {0} for type {1}", Key, parentResult.GetType()));
            }
            if (hndlr.Ignored)
            {
                switch (context.IgnoredPropertyAction)
                {
                    case SerializationContext.IgnoredPropertyOption.Ignore:
                        return null;
                    case SerializationContext.IgnoredPropertyOption.SetIfPossible:
                        if (!hndlr.CanWrite)
                            return null;
                        break;
                    case SerializationContext.IgnoredPropertyOption.ThrowException:
                        throw new Exception(string.Format("Can not set property {0} for type {1} because it is ignored and IgnorePropertyAction is set to ThrowException", Key, parentResult.GetType()));
                }
            }
            ValueExpression.ResultType = hndlr.PropertyType;
            if (hndlr.HasConverter)
            {
                // find the converter and set it for the property
                IJsonTypeConverter converter = hndlr.TypeConverter;
                IEvaluator defaultEvaluator = EvaluatorFactory.GetEvaluator(ValueExpression, context);
                if (defaultEvaluator is ConverterEvaluator)
                {
                    // override the type converter with the property converter
                    defaultEvaluator = ((ConverterEvaluator)defaultEvaluator).DefaultEvaluator;
                }
                ConverterEvaluator evaluator = new ConverterEvaluator(ValueExpression, defaultEvaluator, converter);
                evaluator.Context = context;
                ValueExpression.Evaluator = evaluator;
            }
            object result = null;
            if (!hndlr.CanWrite)
            {
                result = hndlr.GetValue(parentResult);
                ValueExpression.GetEvaluator(context).SetResult(result);
                ValueExpression.OnObjectConstructed(result);
                ValueExpression.Evaluate(context);
            }
            else
            {
                result = ValueExpression.Evaluate(context);
                hndlr.SetValue(parentResult, result);
            }
            return result;
        }

    } 
}
