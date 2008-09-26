using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;
using JsonExSerializer.MetaData;
using System.Collections;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    /// <summary>
    /// Serialize a non-primitive non-scalar object.  Will use the
    /// following notation:
    /// <c>
    ///  { prop1: "value1", prop2: "value2" }
    /// </c>
    /// </summary>
    /// <param name="obj">the object to serialize</param>
    /// <param name="currentPath">object's path</param>
    public class JsonObjectHandler : ObjectHandlerBase
    {
        public JsonObjectHandler()
        {
        }

        public JsonObjectHandler(SerializationContext Context)
            : base(Context)
        {
        }

        public override ExpressionBase GetExpression(object Data, JsonPath CurrentPath, ISerializerHandler Serializer)
        {
             TypeHandler handler = Context.GetTypeHandler(Data.GetType());

            ObjectExpression expression = new ObjectExpression();
            if (handler.ConstructorParameters.Count > 0)
            {
                expression.ResultType = Data.GetType();
                foreach (IPropertyHandler ctorParm in handler.ConstructorParameters)
                {
                    object value = ctorParm.GetValue(Data);
                    ExpressionBase argExpr;
                    // TODO: Improve reference support when constructor arguments are refactored
                    if (ctorParm.HasConverter)
                    {
                        argExpr = Serializer.Serialize(value, new JsonPath(""), ctorParm.TypeConverter);
                    }
                    else
                    {
                        argExpr = Serializer.Serialize(value, new JsonPath(""));
                    }
                    if (value != null && value.GetType() != ctorParm.PropertyType)
                    {
                        argExpr = new CastExpression(value.GetType(), argExpr);
                    }
                    expression.ConstructorArguments.Add(argExpr);
                }
            }


            foreach (IPropertyHandler prop in handler.Properties)
            {
                object value = prop.GetValue(Data);
                ExpressionBase valueExpr;
                if (prop.HasConverter)
                {
                    valueExpr = Serializer.Serialize(value, CurrentPath.Append(prop.Name), prop.TypeConverter);
                }
                else
                {
                    valueExpr = Serializer.Serialize(value, CurrentPath.Append(prop.Name));
                }
                if (value != null && value.GetType() != prop.PropertyType)
                {
                    valueExpr = new CastExpression(value.GetType(), valueExpr);
                }
                expression.Add(prop.Name, valueExpr);
            }
            return expression;
        }

        public override object Evaluate(ExpressionBase Expression, IDeserializerHandler Deserializer)
        {
            object value = ConstructObject((ObjectExpression)Expression, Deserializer);
            value = Evaluate(Expression, value, Deserializer);
            if (value is IDeserializationCallback)
                ((IDeserializationCallback)value).OnAfterDeserialization();
            return value;
        }

        public override object Evaluate(ExpressionBase Expression, object ExistingObject, IDeserializerHandler Deserializer)
        {
            TypeHandler typeHandler = Context.GetTypeHandler(ExistingObject.GetType());
            ObjectExpression objectExpression = (ObjectExpression)Expression;
            foreach (KeyValueExpression Item in objectExpression.Properties)
            {
                // evaluate the item and let it assign itself?
                IPropertyHandler hndlr = typeHandler.FindProperty(Item.Key);
                if (hndlr == null)
                {
                    throw new Exception(string.Format("Could not find property {0} for type {1}", Item.Key, typeHandler.ForType));
                }
                if (hndlr.Ignored)
                {
                    switch (Context.IgnoredPropertyAction)
                    {
                        case SerializationContext.IgnoredPropertyOption.Ignore:
                            continue;
                        case SerializationContext.IgnoredPropertyOption.SetIfPossible:
                            if (!hndlr.CanWrite)
                                continue;
                            break;
                        case SerializationContext.IgnoredPropertyOption.ThrowException:
                            throw new Exception(string.Format("Can not set property {0} for type {1} because it is ignored and IgnorePropertyAction is set to ThrowException", Item.Key, typeHandler.ForType));
                    }
                }
                ExpressionBase valueExpression = Item.ValueExpression;
                valueExpression.ResultType = hndlr.PropertyType;
                object result = null;
                TypeConverterObjectHandler converterHandler = null;
                IJsonTypeConverter converter = null;
                if (hndlr.HasConverter)
                {
                    converterHandler = (TypeConverterObjectHandler) Context.ObjectHandlers.Find(typeof(TypeConverterObjectHandler));
                    converter = hndlr.TypeConverter;
                    /*
                    IEvaluator defaultEvaluator = EvaluatorFactory.GetEvaluator(valueExpression, Context);
                    
                    if (defaultEvaluator is ConverterEvaluator)
                    {
                        // override the type converter with the property converter
                        defaultEvaluator = ((ConverterEvaluator)defaultEvaluator).DefaultEvaluator;
                    }
                    ConverterEvaluator evaluator = new ConverterEvaluator(valueExpression, defaultEvaluator, converter);
                    evaluator.Context = Context;
                    valueExpression.Evaluator = evaluator;
                    */
                }
                
                if (!hndlr.CanWrite)
                {
                    result = hndlr.GetValue(ExistingObject);
                    if (converterHandler != null)
                    {
                        converterHandler.Evaluate(valueExpression, result, Deserializer, converter);
                        //TODO: Use ObjectHandler here
                        //valueExpression.GetEvaluator(Context).SetResult(result);
                        //valueExpression.OnObjectConstructed(result);
                        //valueExpression.Evaluate(Context);
                    }
                    else
                    {
                        Deserializer.Evaluate(valueExpression, result);
                    }
                }
                else
                {
                    //TODO: Use ObjectHandler here
                    if (hndlr.HasConverter)
                        hndlr.SetValue(ExistingObject, converterHandler.Evaluate(valueExpression,Deserializer,converter));
                    else
                        hndlr.SetValue(ExistingObject, Deserializer.Evaluate(valueExpression));
                }
            }
            return ExistingObject;
        }

        protected virtual object ConstructObject(ObjectExpression Expression, IDeserializerHandler Deserializer)
        {
            // set the default type if none set
            if (Expression.ConstructorArguments.Count > 0)
            {
                ResolveConstructorTypes(Context, Expression);
            }
            object[] args = new object[Expression.ConstructorArguments.Count];

            for (int i = 0; i < args.Length; i++)
            {
                ExpressionBase carg = Expression.ConstructorArguments[i];
                args[i] = Deserializer.Evaluate(carg);
            }
            TypeHandler handler = Context.GetTypeHandler(Expression.ResultType);
            object result = handler.CreateInstance(args);
            Expression.OnObjectConstructed(result);
            return result;
        }

        protected static void ResolveConstructorTypes(SerializationContext Context, ObjectExpression Expression)
        {
            TypeHandler handler = Context.GetTypeHandler(Expression.ResultType);
            Type[] definedTypes = GetConstructorParameterTypes(handler.ConstructorParameters);

            CtorArgTypeResolver resolver = new CtorArgTypeResolver(Expression, Context, definedTypes);
            Type[] resolvedTypes = resolver.ResolveTypes();
            for (int i = 0; i < resolvedTypes.Length; i++)
            {
                if (resolvedTypes[i] != null)
                    Expression.ConstructorArguments[i].ResultType = resolvedTypes[i];
            }
        }

        protected static Type[] GetConstructorParameterTypes(IList<IPropertyHandler> ConstructorParameters)
        {
            Type[] types = new Type[ConstructorParameters.Count];
            for (int i = 0; i < ConstructorParameters.Count; i++)
            {
                types[i] = ConstructorParameters[i].PropertyType;
            }
            return types;
        }

        public override bool CanHandle(Type ObjectType)
        {
            return true;
        }
    }
}
