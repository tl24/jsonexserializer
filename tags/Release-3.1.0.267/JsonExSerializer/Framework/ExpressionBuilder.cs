/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using JsonExSerializer.TypeConversion;
using JsonExSerializer.Collections;
using System.Reflection;
using JsonExSerializer.MetaData;
using JsonExSerializer.Framework.Expressions;
using JsonExSerializer.Framework.ExpressionHandlers;

namespace JsonExSerializer.Framework
{
    /// <summary>
    /// Class to do the work of serializing an object
    /// </summary>
    public class ExpressionBuilder : IExpressionBuilder
    {
        private Type _serializedType;
        private IConfiguration _config;
        private const int indentStep = 3;
        private IDictionary<object, ReferenceInfo> _refs;

        internal ExpressionBuilder(Type t, IConfiguration config)
        {
            _serializedType = t;
            _config = config;
            _refs = new Dictionary<object, ReferenceInfo>(new ReferenceEqualityComparer<object>());
        }

        /// <summary>
        /// Serialize the given object
        /// </summary>
        /// <param name="value">object to serialize</param>
        public Expression Serialize(object value)
        {
            Expression expr = Serialize(value, new JsonPath(), null);
            if (value != null && !ReflectionUtils.AreEquivalentTypes(value.GetType(), _serializedType))
            {
                expr = new CastExpression(value.GetType(), expr);
            }
            return expr;
        }

        public Expression Serialize(object value, JsonPath currentPath)
        {
            return Serialize(value, currentPath, null);
        }
        /// <summary>
        /// Serialize the given object at the current indent level.  The path to the object is represented by
        /// currentPath such as "this.name", etc.  This is an internal method that can be called recursively.
        /// </summary>
        /// <param name="value">the object to serialize</param>
        /// <param name="currentPath">the current path for reference writing</param>
        public Expression Serialize(object value, JsonPath currentPath, IJsonTypeConverter converter)
        {
            if (value == null)
            {
                return new NullExpression();
            }
            else
            {
                IExpressionHandler objHandler;
                bool isReferencable = _config.IsReferenceableType(value.GetType());
                if (converter != null)
                {
                    TypeConverterExpressionHandler converterHandler = (TypeConverterExpressionHandler)_config.ExpressionHandlers.Find(typeof(TypeConverterExpressionHandler));
                    isReferencable = converterHandler.IsReferenceable(value, converter);
                    objHandler = converterHandler;
                }
                else
                {
                    objHandler = _config.ExpressionHandlers.GetHandler(value);
                    isReferencable = objHandler.IsReferenceable(value);
                }

                if (isReferencable)
                {
                    Expression expr = HandleReference(value, currentPath);
                    if (expr != null)
                        return expr;
                }

                ISerializationCallback callback = value as ISerializationCallback;
                if (callback != null)
                    callback.OnBeforeSerialization();

                try
                {
                    if (converter != null)
                    {
                        return ((TypeConverterExpressionHandler)objHandler).GetExpression(value, converter, currentPath, this);
                    }
                    else
                    {
                        SetCanReference(value);
                        return objHandler.GetExpression(value, currentPath, this);
                    }
                }
                finally
                {
                    if (callback != null)
                        callback.OnAfterSerialization();
                }
            }
        }

        protected virtual Expression HandleReference(object value, JsonPath CurrentPath)
        {
            ReferenceInfo refInfo = null;
            if (_refs.ContainsKey(value))
            {
                /*
                 * This object has already been seen by the serializer so
                 * determine what to do with it.  If its part of the current path
                 * then its a circular reference and an error needs to be thrown or it should
                 * be ignored depending on the option. Otherwise write a reference to it
                 */
                refInfo = _refs[value];
                JsonPath refPath = refInfo.Path;
                switch (_config.ReferenceWritingType)
                {
                    case SerializationContext.ReferenceOption.WriteIdentifier:
                        if (!refInfo.CanReference)
                            throw new InvalidOperationException("Can't reference object: " + refPath + " from " + CurrentPath + ", either it is a collection, or it has not been converted yet");

                        return new ReferenceExpression(refPath);
                    case SerializationContext.ReferenceOption.IgnoreCircularReferences:
                        if (CurrentPath.StartsWith(refPath))
                        {
                            return new NullExpression();
                        }
                        break;
                    case SerializationContext.ReferenceOption.ErrorCircularReferences:
                        if (CurrentPath.StartsWith(refPath))
                        {
                            throw new InvalidOperationException("Circular reference detected.  Current path: " + CurrentPath + ", reference to: " + refPath);
                        }
                        break;
                }
            }
            else
            {
                refInfo = new ReferenceInfo(CurrentPath);
                _refs[value] = refInfo;
            }
            return null;
        }


        /// <summary>
        /// Indicates that the object can now be referenced.  Any attempts to build a reference to the current object before
        /// this method is called will result in an exception.
        /// </summary>
        /// <param name="value">the object being referenced</param>
        public void SetCanReference(object value) {
            ReferenceInfo refInfo;
            if (_refs.TryGetValue(value, out refInfo))
            {
                refInfo.CanReference = true;
            }
        }

        /// <summary>
        /// Helper class to store information about a reference
        /// </summary>
        private class ReferenceInfo
        {
            public JsonPath Path;
            public bool CanReference;

            public ReferenceInfo(JsonPath Path)
            {
                this.Path = Path;
            }
        }

    }
}
