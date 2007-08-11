using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer.Expression
{
    
    /// <summary>
    /// Key Value pairs in Object Expression.
    /// This class will alter the normal evaluation of the value by loading the propertyinfo
    /// for this key, from the parent and checking for a property converter.
    /// </summary>
    public class KeyValueExpression : ExpressionBase {
        private ExpressionBase _keyExpression;
        private ExpressionBase _valueExpression;

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
                return ((ValueExpression)this._keyExpression).Value; 
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

        public override object Evaluate(SerializationContext context)
        {
            return Evaluate(context, this.Parent.GetReference(context));
        }

        public object Evaluate(SerializationContext context, object parentResult)
        {
            if (Parent.ResultType.GetInterface(typeof(IDictionary).FullName) != null)
            {
                return EvaluateDictionaryItem(context, parentResult);
            }
            else
            {
                return EvaluateObjectProperty(context, parentResult);
            }
        }

        public object EvaluateDictionaryItem(SerializationContext context, object parentObject)
        {
            if (ValueExpression.ResultType == typeof(object) || ValueExpression.ResultType == null) {
                Type keyType = typeof(string);
                Type valueType = typeof(object);
                // attempt to figure out what the types of the values are, if no type is set already
                if (Parent.ResultType.GetInterface(typeof(IDictionary<,>).Name) != null)
                {
                    Type genDict = Parent.ResultType.GetInterface(typeof(IDictionary<,>).Name);
                    Type[] genArgs = genDict.GetGenericArguments();
                    keyType = genArgs[0];
                    valueType = genArgs[1];
                }
                // if no type set, set one
                KeyExpression.SetResultTypeIfNotSet(keyType);
                ValueExpression.SetResultTypeIfNotSet(valueType);
            }
            object keyObject = KeyExpression.Evaluate(context);
            object result = ValueExpression.Evaluate(context);
            ((IDictionary)parentObject)[keyObject] = result;
            return result;
        }

        public object EvaluateObjectProperty(SerializationContext context, object parentObject)
        {
            // lookup info for the type
            PropertyHandler hndlr = context.GetTypeHandler(parentObject.GetType()).FindProperty(Key);
            ValueExpression.SetResultTypeIfNotSet(hndlr.PropertyType);
            if (context.HasConverter(hndlr.Property))
            {
                // find the converter and set it for the property
                IJsonTypeConverter converter = context.GetConverter(hndlr.Property);
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
            object result = ValueExpression.Evaluate(context);
            hndlr.SetValue(parentObject, result);
            return result;
        }

        public override object GetReference(SerializationContext context)
        {
            // any references should always point to the value object
            // so this should never get called
            throw new InvalidOperationException("KeyValueExpression should not be referenced, only the ValueExpression property");
        }

        public override ExpressionBase ResolveReference(ReferenceIdentifier refID)
        {
            if (refID.Current == Key)
            {
                return ValueExpression.ResolveReference(refID);
            }
            else
            {
                throw new Exception("Unable to resolve reference: " + refID);
            }
        }


    } 
}
