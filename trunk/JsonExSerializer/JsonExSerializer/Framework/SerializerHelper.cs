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
using JsonExSerializer.Framework.ObjectHandlers;

namespace JsonExSerializer.Framework
{
    /// <summary>
    /// Class to do the work of serializing an object
    /// </summary>
    sealed class SerializerHelper : JsonWriter, ISerializerHandler
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
            ExpressionBase expr = Serialize(o, new JsonPath(), null);
            if (o != null && o.GetType() != _serializedType)
            {
                expr = new CastExpression(o.GetType(), expr);
            }
            ExpressionWriter.Write(this, _context, expr);
        }
        public ExpressionBase Serialize(object o, JsonPath currentPath)
        {
            return Serialize(o, currentPath, null);
        }
        /// <summary>
        /// Serialize the given object at the current indent level.  The path to the object is represented by
        /// currentPath such as "this.name", etc.  This is an internal method that can be called recursively.
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <param name="currentPath">the current path for reference writing</param>
        public ExpressionBase Serialize(object o, JsonPath currentPath, IJsonTypeConverter converter)
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
                        return SerializeValue(o, currentPath);
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
                        return SerializeNumber(o, currentPath);
                    case TypeCode.Boolean:
                        return SerializeBoolean(o, currentPath);
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
                            JsonPath refPath = refInfo.Path;
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

                        if (o is ISerializationCallback)
                            ((ISerializationCallback)o).OnBeforeSerialization();

                        try
                        {
                            refInfo.CanReference = true;    // regular object, can reference at any time
                            IObjectHandler objHandler = _context.ObjectHandlers.GetHandler(o);
                            return objHandler.GetExpression(o, currentPath, this);
                        }
                        finally
                        {
                            if (o is ISerializationCallback)
                                ((ISerializationCallback)o).OnAfterSerialization();
                        }
                    default:
                        return new ValueExpression(Convert.ToString(o));
                }
            }
        }

        private ExpressionBase SerializeBoolean(object o, JsonPath CurrentPath)
        {
            return _context.ObjectHandlers.BooleanHandler.GetExpression(o, CurrentPath, this);
        }

        private ExpressionBase SerializeValue(object o, JsonPath CurrentPath)
        {
            return _context.ObjectHandlers.ValueHandler.GetExpression(o, CurrentPath, this);
        }

        private ExpressionBase SerializeNumber(object o, JsonPath CurrentPath)
        {
            return _context.ObjectHandlers.NumericHandler.GetExpression(o, CurrentPath, this);
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
            public JsonPath Path;
            public bool CanReference = false;

            public ReferenceInfo(JsonPath Path)
            {
                this.Path = Path;
            }
        }

    }
}
