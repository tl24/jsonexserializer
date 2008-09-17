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
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework
{
    /// <summary>
    /// Class to do the work of serializing an object
    /// </summary>
    sealed class SerializerHelper : JsonWriter
    {
        private Type _serializedType;
        private SerializationContext _context;
        //private TextWriter _writer;
        private const int indentStep = 3;
        private IDictionary<object, ReferenceInfo> _refs;

        internal SerializerHelper(Type t, SerializationContext context, TextWriter writer) : base(writer, !context.IsCompact)
        {
            _serializedType = t;
            _context = context;
            //_writer = writer;
            _refs = new Dictionary<object, ReferenceInfo>(new ReferenceEqualityComparer<object>());
        }

        /// <summary>
        /// Serialize the given object
        /// </summary>
        /// <param name="o">object to serialize</param>
        public void Serialize(object o)
        {
            if (o != null && _context.OutputTypeComment)
            {
                string comment = "";
                comment += "/*" + "\r\n";
                comment += "  Created by JsonExSerializer" + "\r\n";
                comment += "  Assembly: " + o.GetType().Assembly.ToString() + "\r\n";
                comment += "  Type: " + o.GetType().FullName + "\r\n";
                comment += "*/" + "\r\n";
                this.Comment(comment);
            }
            ExpressionBase expr = Serialize(o, JsonPath.Root, null);
            if (o != null && o.GetType() != _serializedType)
            {
                expr = new CastExpression(o.GetType(), expr);
            }
            ExpressionWriter.Write(this, _context, expr);
        }

        /// <summary>
        /// Serialize the given object at the current indent level.  The path to the object is represented by
        /// currentPath such as "this.name", etc.  This is an internal method that can be called recursively.
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <param name="currentPath">the current path for reference writing</param>
        private ExpressionBase Serialize(object o, string currentPath, IJsonTypeConverter converter)
        {
            if (o == null)
            {
                return new NullExpression();
            }
            else
            {
                // Get the typecode and call the approriate method
                switch (Type.GetTypeCode(o.GetType()))
                {
                    case TypeCode.Char:
                    case TypeCode.String:
                    case TypeCode.DateTime:
                        return new ValueExpression(o);
                        break;
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.Double:
                    case TypeCode.Single:
                    case TypeCode.UInt64:
                    case TypeCode.Decimal:
                        return new NumericExpression(o);
                        break;
                    case TypeCode.Boolean:
                        return new BooleanExpression((bool)o);
                        break;
                    case TypeCode.Empty:
                        throw new ApplicationException("Unsupported value (Empty): " + o);
                    case TypeCode.Object:
                        ReferenceInfo refInfo = null;

                        if (_refs.ContainsKey(o))
                        {
                            /*
                             * This object has already been seen by the serializer so
                             * determine what to do with it.  If its part of the current path
                             * then its a circular reference and an error needs to be thrown or it should
                             * be ignored depending on the option. Otherwise write a reference to it
                             */ 
                            refInfo = _refs[o];
                            string refPath = refInfo.Path;
                            switch (_context.ReferenceWritingType)
                            {
                                case SerializationContext.ReferenceOption.WriteIdentifier:
                                    if (!refInfo.CanReference)
                                        throw new JsonExSerializationException("Can't reference object: " + refPath + " from " + currentPath + ", either it is a collection, or it has not been converted yet");

                                    return new ReferenceExpression(refPath);
                                case SerializationContext.ReferenceOption.IgnoreCircularReferences:
                                    if (currentPath.StartsWith(refPath))
                                    {
                                        return new NullExpression();
                                    }
                                    break;
                                case SerializationContext.ReferenceOption.ErrorCircularReferences:
                                    if (currentPath.StartsWith(refPath))
                                    {
                                        throw new JsonExSerializationException("Circular reference detected.  Current path: " + currentPath + ", reference to: " + refPath);
                                    }
                                    break;
                            }
                        } else {
                            refInfo = new ReferenceInfo(currentPath);
                            _refs[o] = refInfo;
                        }
                        TypeHandler handler = _context.GetTypeHandler(o.GetType());

                        // Check for a converter and convert
                        if (converter != null || handler.HasConverter)
                        {
                            converter = (converter != null) ? converter : handler.TypeConverter;
                            o = converter.ConvertFrom(o, _context);
                            // call serialize again in case the new type has a converter
                            ExpressionBase expr = Serialize(o, currentPath, null);
                            refInfo.CanReference = true;    // can't reference inside the object
                            return expr;
                        }
                        else if (o is IJsonTypeConverter)
                        {
                            o = ((IJsonTypeConverter)o).ConvertFrom(o, _context);
                            // call serialize again in case the new type has a converter
                            ExpressionBase expr = Serialize(o, currentPath, null);
                            refInfo.CanReference = true;    // can't reference inside the object
                            return expr;
                        }
                        
                        if (handler.IsCollection())
                        {
                            return SerializeCollection(o, currentPath);
                        }
                        else if (o is ICollection && !(o is IDictionary))
                        {
                            throw new InvalidOperationException(o.GetType() + " is a collection but doesn't have a CollectionHandler");
                        }
                        else
                        {
                            refInfo.CanReference = true;    // regular object, can reference at any time
                            return SerializeObject(o, currentPath);
                        }
                        break;
                    default:
                        return new ValueExpression(Convert.ToString(o));
                        break;
                }
            }
        }

        /// <summary>
        /// Serialize a non-primitive non-scalar object.  Will use the
        /// following notation:
        /// <c>
        ///  { prop1: "value1", prop2: "value2" }
        /// </c>
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <param name="currentPath">object's path</param>
        private ExpressionBase SerializeObject(object obj, string currentPath)
        {
            if (obj is IDictionary)
            {
                return SerializeDictionary((IDictionary) obj, currentPath);
            }

            TypeHandler handler = _context.GetTypeHandler(obj.GetType());
            
            if (obj is ISerializationCallback)
            {
                ((ISerializationCallback)obj).OnBeforeSerialization();
            }

            ObjectExpression expression = new ObjectExpression();
            try
            {
                if (handler.ConstructorParameters.Count > 0)
                {
                    expression.ResultType = obj.GetType();
                    foreach (AbstractPropertyHandler ctorParm in handler.ConstructorParameters)
                    {
                        object value = ctorParm.GetValue(obj);
                        ExpressionBase argExpr;
                        if (ctorParm.HasConverter)
                        {
                            argExpr = Serialize(value, "", ctorParm.TypeConverter);
                        }
                        else
                        {
                            argExpr = Serialize(value, "", null);
                        }
                        if (value != null && value.GetType() != ctorParm.PropertyType)
                        {
                            argExpr = new CastExpression(value.GetType(), argExpr);
                        }
                        expression.ConstructorArguments.Add(argExpr);
                    }
                }


                foreach (AbstractPropertyHandler prop in handler.Properties)
                {
                    object value = prop.GetValue(obj);
                    ExpressionBase valueExpr;
                    if (prop.HasConverter)
                    {
                        valueExpr = Serialize(value, currentPath + "." + prop.Name, prop.TypeConverter);
                    }
                    else
                    {
                        valueExpr = Serialize(value, currentPath + "." + prop.Name, null);
                    }
                    if (value != null && value.GetType() != prop.PropertyType)
                    {
                        valueExpr = new CastExpression(value.GetType(), valueExpr);
                    }
                    expression.Add(prop.Name, valueExpr);
                }
                return expression;
            }
            finally
            {
                // make sure this is in a finally block in case the ISerializationCallback interface
                // is used to control thread locks
                if (obj is ISerializationCallback)
                {
                    ((ISerializationCallback)obj).OnAfterSerialization();
                }
            }
            
        }

        /// <summary>
        /// Serialize an object implementing IDictionary.  The serialized data is similar to a regular
        /// object, except that the keys of the dictionary are used instead of properties.
        /// </summary>
        /// <param name="dictionary">the dictionary object</param>
        /// <param name="currentPath">object's path</param>
        private ExpressionBase SerializeDictionary(IDictionary dictionary, string currentPath)
        {
            Type itemType = typeof(object);
            Type genericDictionary = null;

            if ((genericDictionary = dictionary.GetType().GetInterface(typeof(IDictionary<,>).Name)) != null)
            {
                itemType = genericDictionary.GetGenericArguments()[1];
            }

            if (dictionary is ISerializationCallback)
            {
                ((ISerializationCallback)dictionary).OnBeforeSerialization();
            }
            ObjectExpression expression = new ObjectExpression();
            try
            {
                foreach (DictionaryEntry pair in dictionary)
                {
                    //Serialize(pair.Key, subindent, "", null);
                    //may not work in all cases
                    object value = pair.Value;
                    ExpressionBase valueExpr = Serialize(value, currentPath + "." + pair.Key.ToString(), null);
                    if (value != null && value.GetType() != itemType)
                    {
                        valueExpr = new CastExpression(value.GetType(), valueExpr);
                    }
                    expression.Add(pair.Key.ToString(), valueExpr);
                }
                return expression;
            }
            finally
            {
                // make sure this is in a finally block in case the ISerializationCallback interface
                // is used to control thread locks
                if (dictionary is ISerializationCallback)
                {
                    ((ISerializationCallback)dictionary).OnAfterSerialization();
                }
            }
            
        }

        /// <summary>
        /// Serialize an object that acts like a collection.
        /// The syntax will be: [item1, item2, item3]
        /// </summary>
        /// <param name="collection">collection</param>
        /// <param name="currentPath">the object's path</param>
        private ExpressionBase SerializeCollection(object collection, string currentPath)
        {
            TypeHandler handler = _context.GetTypeHandler(collection.GetType());

            CollectionHandler collectionHandler = handler.GetCollectionHandler();
            Type elemType = collectionHandler.GetItemType(handler.ForType);

            int index = 0;

            if (collection is ISerializationCallback)
            {
                ((ISerializationCallback)collection).OnBeforeSerialization();
            }
            ListExpression expression = new ListExpression();
            try
            {
                foreach (object value in collectionHandler.GetEnumerable(collection))
                {
                    ExpressionBase itemExpr = Serialize(value, currentPath + ".[" + index + "]", null);
                    if (value != null && value.GetType() != elemType)
                    {
                        itemExpr = new CastExpression(value.GetType(), itemExpr);
                    }
                    expression.Add(itemExpr);
                    index++;
                }
                return expression;
            }
            finally
            {
                // make sure this is in a finally block in case the ISerializationCallback interface
                // is used to control thread locks
                if (collection is ISerializationCallback)
                {
                    ((ISerializationCallback)collection).OnAfterSerialization();
                }
            }
        }

        /// <summary>
        /// Writes out the type for an object in regular C# code syntax
        /// </summary>
        /// <param name="t">the type to write</param>
        protected override void WriteTypeInfo(Type t)
        {
            string alias = _context.GetTypeAlias(t);
            if (alias != null) {
                _writer.Write(alias);
                return;
            }
            base.WriteTypeInfo(t);
        }

        /// <summary>
        /// Helper class to store information about a reference
        /// </summary>
        private class ReferenceInfo
        {
            public string Path;
            public bool CanReference = false;

            public ReferenceInfo(string Path)
            {
                this.Path = Path;
            }
        }
    }
}
