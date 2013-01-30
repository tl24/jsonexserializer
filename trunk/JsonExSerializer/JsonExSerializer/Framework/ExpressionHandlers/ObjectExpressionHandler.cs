using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.Expressions;
using JsonExSerializer.MetaData;
using System.Collections;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer.Framework.ExpressionHandlers
{
    /// <summary>
    /// ExpressionHandler for a json object or non-primitive object that is not a collection.  This is usually the
    /// default handler.
    /// </summary>
    public class ObjectExpressionHandler : ExpressionHandlerBase
    {
        /// <summary>
        /// Initializes a default instance with no Serialization Context
        /// </summary>
        public ObjectExpressionHandler()
        {
        }

        /// <summary>
        /// Initializes an instance with a Serialization Context
        /// </summary>
        public ObjectExpressionHandler(ISerializerSettings config)
            : base(config)
        {
        }

        /// <summary>
        /// Creates an json object expression from object data.
        /// </summary>
        /// <param name="data">the data to serialize</param>
        /// <param name="currentPath">current path to the object</param>
        /// <param name="serializer">serializer instance used to serialize key values</param>
        /// <returns>json object expression</returns>
        public override Expression GetExpression(object data, JsonPath currentPath, IExpressionBuilder serializer)
        {
            ITypeData handler = Settings.Types[data.GetType()];

            ObjectExpression expression = new ObjectExpression();

            foreach (IPropertyData prop in handler.Properties)
            {
                GenerateItemExpression(data, currentPath, serializer, expression, prop);
            }
            return expression;
        }

        /// <summary>
        /// Generates an expression for an item and adds it to the object
        /// </summary>
        /// <param name="data">the item being serialized</param>
        /// <param name="currentPath">the current path to the object</param>
        /// <param name="serializer">serializer instance</param>
        /// <param name="expression">the object expression</param>
        /// <param name="prop">the property being serialized</param>
        protected virtual void GenerateItemExpression(object data, JsonPath currentPath, IExpressionBuilder serializer, ObjectExpression expression, IPropertyData prop)
        {
            object value = prop.GetValue(data);
            if (!prop.ShouldWriteValue(this.Settings, value))
                return;
            Expression valueExpr;
            if (prop.HasConverter)
            {
                valueExpr = serializer.Serialize(value, currentPath.Append(prop.Alias), prop.TypeConverter);
            }
            else
            {
                valueExpr = serializer.Serialize(value, currentPath.Append(prop.Alias));
            }
            if (value != null && !ReflectionUtils.AreEquivalentTypes(value.GetType(), prop.PropertyType))
            {
                valueExpr = new CastExpression(value.GetType(), valueExpr);
            }
            expression.Add(prop.Alias, valueExpr);
        }

        /// <summary>
        /// Evaluates the expression and deserializes it.
        /// </summary>
        /// <param name="expression">json object expression</param>
        /// <param name="deserializer">deserializer for deserializing key values</param>
        /// <returns>deserialized object</returns>
        public override object Evaluate(Expression expression, IDeserializerHandler deserializer)
        {
            object value = ConstructObject(CastExpression<ObjectExpression>(expression), deserializer);
            value = Evaluate(expression, value, deserializer);
            if (value is IDeserializationCallback)
                ((IDeserializationCallback)value).OnAfterDeserialization();
            return value;
        }

        /// <summary>
        /// Evaluates the expression and populates an existing object with the expression's properties
        /// </summary>
        /// <param name="expression">json object expression</param>
        /// <param name="existingObject">the existing object to populate</param>
        /// <param name="deserializer">deserializer for deserializing key values</param>
        /// <returns>deserialized object</returns>
        public override object Evaluate(Expression expression, object existingObject, IDeserializerHandler deserializer)
        {
            ITypeData typeHandler = Settings.Types[existingObject.GetType()];
            ObjectExpression objectExpression = CastExpression<ObjectExpression>(expression);
            foreach (KeyValueExpression Item in objectExpression.Properties)
            {
                EvaluateItem(existingObject, deserializer, typeHandler, Item);
            }
            return existingObject;
        }

        protected virtual void EvaluateItem(object existingObject, IDeserializerHandler deserializer, ITypeData typeHandler, KeyValueExpression Item)
        {
            // evaluate the item and let it assign itself?
            IPropertyData hndlr = typeHandler.FindPropertyByAlias(Item.Key);
            if (hndlr == null)
            {
                switch (this.Settings.MissingPropertyAction)
                {
                    case MissingPropertyOptions.Ignore:
                        return;
                    case MissingPropertyOptions.ThrowException:
                        throw new Exception(string.Format("Could not find property {0} for type {1}", Item.Key, typeHandler.ForType));
                    default:
                        throw new InvalidOperationException("Unhandled MissingPropertyAction: " + this.Settings.MissingPropertyAction);
                }
            }
            if (hndlr.Ignored)
            {
                switch (Settings.IgnoredPropertyAction)
                {
                    case IgnoredPropertyOption.Ignore:
                        return;
                    case IgnoredPropertyOption.SetIfPossible:
                        if (!hndlr.CanWrite)
                            return;
                        break;
                    case IgnoredPropertyOption.ThrowException:
                        throw new Exception(string.Format("Can not set property {0} for type {1} because it is ignored and IgnorePropertyAction is set to ThrowException", Item.Key, typeHandler.ForType));
                }
            }
            Expression valueExpression = Item.ValueExpression;
            valueExpression.ResultType = hndlr.PropertyType;
            object result = null;
            TypeConverterExpressionHandler converterHandler = null;
            IJsonTypeConverter converter = null;
            if (hndlr.HasConverter)
            {
                converterHandler = (TypeConverterExpressionHandler)Settings.ExpressionHandlers.Find(typeof(TypeConverterExpressionHandler));
                converter = hndlr.TypeConverter;
            }

            if (!hndlr.CanWrite)
            {
                result = hndlr.GetValue(existingObject);
                if (converterHandler != null)
                {
                    converterHandler.Evaluate(valueExpression, result, deserializer, converter);

                }
                else
                {
                    deserializer.Evaluate(valueExpression, result);
                }
            }
            else
            {
                if (hndlr.HasConverter)
                    hndlr.SetValue(existingObject, converterHandler.Evaluate(valueExpression, deserializer, converter));
                else
                    hndlr.SetValue(existingObject, deserializer.Evaluate(valueExpression));
            }
        }

        /// <summary>
        /// Constructs a new instance of the object represented by the expression.
        /// </summary>
        /// <param name="expression">json object expression</param>
        /// <param name="deserializer">deserializer for deserializing constructor arguments if any</param>
        /// <returns>constructed, but unpopulated object</returns>
        protected virtual object ConstructObject(ObjectExpression expression, IDeserializerHandler deserializer)
        {
            ITypeData handler = Settings.Types[expression.ResultType];
            // set the default type if none set
            if (expression.ConstructorArguments.Count > 0)
            {
                // old way expects parameters in the constructor list
                ResolveConstructorTypes(Settings, expression);
            }
            else
            {
                foreach (IPropertyData parameter in handler.ConstructorParameters)
                {
                    int propLocation = expression.IndexOf(parameter.Alias);
                    if (propLocation >= 0)
                    {
                        Expression arg = expression.Properties[propLocation].ValueExpression;
                        arg.ResultType = parameter.PropertyType;
                        expression.ConstructorArguments.Add(arg);
                        expression.Properties.RemoveAt(propLocation);
                    }
                    else
                    {
                        expression.ConstructorArguments.Add(new NullExpression());
                    }
                }
            }

            object[] args = new object[expression.ConstructorArguments.Count];

            for (int i = 0; i < args.Length; i++)
            {
                Expression carg = expression.ConstructorArguments[i];
                if (i < handler.ConstructorParameters.Count && handler.ConstructorParameters[i].HasConverter)
                {
                    TypeConverterExpressionHandler converterHandler = (TypeConverterExpressionHandler)Settings.ExpressionHandlers.Find(typeof(TypeConverterExpressionHandler));
                    args[i] = converterHandler.Evaluate(carg, deserializer, handler.ConstructorParameters[i].TypeConverter);
                }
                else
                {
                    args[i] = deserializer.Evaluate(carg);
                }
            }
            object result = handler.CreateInstance(args);
            expression.OnObjectConstructed(result);
            return result;
        }

        /// <summary>
        /// Resolves and updates the types of any constructor arguments
        /// </summary>
        /// <param name="context">serialization context</param>
        /// <param name="expression">object expression</param>
        protected static void ResolveConstructorTypes(ISerializerSettings settings, ObjectExpression expression)
        {
            ITypeData handler = settings.Types[expression.ResultType];
            Type[] definedTypes = GetConstructorParameterTypes(handler.ConstructorParameters);

            CtorArgTypeResolver resolver = new CtorArgTypeResolver(expression, settings, definedTypes);
            Type[] resolvedTypes = resolver.ResolveTypes();
            for (int i = 0; i < resolvedTypes.Length; i++)
            {
                if (resolvedTypes[i] != null)
                    expression.ConstructorArguments[i].ResultType = resolvedTypes[i];
            }
        }

        /// <summary>
        /// Gets the default types of any constructor parameters from the type metadata
        /// </summary>
        /// <param name="constructorParameters">constructor parameter list</param>
        /// <returns>default types</returns>
        protected static Type[] GetConstructorParameterTypes(IList<IPropertyData> constructorParameters)
        {
            Type[] types = new Type[constructorParameters.Count];
            for (int i = 0; i < constructorParameters.Count; i++)
            {
                types[i] = constructorParameters[i].PropertyType;
            }
            return types;
        }

        /// <summary>
        /// Determines whether this handler can handle a specific object type
        /// </summary>
        /// <param name="objectType">the object type</param>
        /// <returns>true if this handler handles the type</returns>
        public override bool CanHandle(Type objectType)
        {
            return true;
        }
    }
}
