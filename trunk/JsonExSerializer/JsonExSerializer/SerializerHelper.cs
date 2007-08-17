using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using JsonExSerializer.TypeConversion;
using JsonExSerializer.Collections;
using System.Reflection;

namespace JsonExSerializer
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

        public override IJsonWriter WriteObject(object o)
        {
            throw new Exception("Method not implemented");
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
            if (o != null && _context.OutputTypeInformation && o.GetType() != _serializedType)
            {
                WriteCast(o.GetType());
            }
            Serialize(o, "this", null);
            
        }

        /// <summary>
        /// Serialize the given object at the current indent level.  The path to the object is represented by
        /// currentPath such as "this.name", etc.  This is an internal method that can be called recursively.
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <param name="indent">indent level for formating</param>
        /// <param name="currentPath">the current path for reference writing</param>
        private void Serialize(object o, string currentPath, IJsonTypeConverter converter)
        {
            if (o == null)
            {
                this.SpecialValue("null");
            }
            else
            {
                // Get the typecode and call the approriate method
                switch (Type.GetTypeCode(o.GetType()))
                {
                    case TypeCode.Char:
                        this.QuotedValue(((char)o).ToString());
                        break;
                    case TypeCode.String:
                        this.QuotedValue((string)o);
                        break;
                    case TypeCode.Double:
                        this.Value((double) o);
                        break;
                    case TypeCode.Single:
                        this.Value((float)o);
                        break;
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

                                    this.SpecialValue(refPath);
                                    return;
                                case SerializationContext.ReferenceOption.IgnoreCircularReferences:
                                    if (currentPath.StartsWith(refPath))
                                    {
                                        this.SpecialValue("null");
                                        return;
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
                        // Check for a converter and convert
                        if (converter != null || _context.HasConverter(o.GetType()))
                        {
                            converter = (converter != null) ? converter : _context.GetConverter(o.GetType());
                            o = converter.ConvertFrom(o, _context);
                            // call serialize again in case the new type has a converter
                            Serialize(o, currentPath, null);
                            refInfo.CanReference = true;    // can't reference inside the object
                            return;
                        }
                        else if (o is IJsonTypeConverter)
                        {
                            o = ((IJsonTypeConverter)o).ConvertFrom(o, _context);
                            // call serialize again in case the new type has a converter
                            Serialize(o, currentPath, null);
                            refInfo.CanReference = true;    // can't reference inside the object
                            return;
                        }
                        TypeHandler handler = _context.GetTypeHandler(o.GetType());
                        if (handler.IsCollection())
                        {
                            SerializeCollection(o, currentPath);
                        }
                        else if (o is ICollection && !(o is IDictionary))
                        {
                            throw new CollectionException(o.GetType() + " is a collection but doesn't have a CollectionHandler");
                        }
                        else
                        {
                            refInfo.CanReference = true;    // regular object, can reference at any time
                            SerializeObject(o, currentPath);
                        }
                        break;
                    case TypeCode.DateTime:
                        WriteDateTime((DateTime)o);
                        break;
                    case TypeCode.Boolean:
                        this.Value((bool) o);
                        break;
                    case TypeCode.Byte:
                        WriteByte((byte)o);
                        break;
                    case TypeCode.DBNull:
                        WriteDBNull((DBNull)o);
                        break;
                    case TypeCode.Empty:
                        throw new ApplicationException("Unsupported value (Empty): " + o);
                    case TypeCode.Int16:
                        WriteInt16((short)o);
                        break;
                    case TypeCode.Int32:
                        this.Value((int)o);
                        break;
                    case TypeCode.Int64:
                        WriteInt64((long)o);
                        break;
                    case TypeCode.SByte:
                        WriteSByte((sbyte)o);
                        break;

                    case TypeCode.UInt16:
                        WriteUInt16((ushort)o);
                        break;
                    case TypeCode.UInt32:
                        WriteUInt32((uint)o);
                        break;
                    case TypeCode.UInt64:
                        WriteUInt64((ulong)o);
                        break;
                    case TypeCode.Decimal:
                        WriteDecimal((decimal)o);
                        break;
                    default:
                        this.SpecialValue(Convert.ToString(o));
                        break;
                }
            }
        }

        /// <summary>
        /// serialize an enum type
        /// </summary>
        /// <param name="o">the enum to serialize</param>
        private void SerializeEnum(Enum enm)
        {
            _writer.Write(Enum.Format(enm.GetType(), enm, "d"));
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
        private void SerializeObject(object obj, string currentPath)
        {
            if (obj is IDictionary)
            {
                SerializeDictionary((IDictionary) obj, currentPath);
                return;
            }

            TypeHandler handler = _context.GetTypeHandler(obj.GetType());
            
            this.ObjectStart();
            if (obj is ISerializationCallback)
            {
                ((ISerializationCallback)obj).OnBeforeSerialization();
            }

            try
            {
                foreach (PropertyHandler prop in handler.Properties)
                {
                    this.Key(prop.Name);
                    object value = prop.GetValue(obj);
                    if (value != null && _context.OutputTypeInformation && value.GetType() != prop.PropertyType)
                    {
                        this.Cast(value.GetType());
                    }
                    if (_context.HasConverter(prop.Property))
                    {
                        Serialize(value, currentPath + "." + prop.Name, _context.GetConverter(prop.Property));
                    }
                    else
                    {
                        Serialize(value, currentPath + "." + prop.Name, null);
                    }
                }
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
            this.ObjectEnd();
        }

        /// <summary>
        /// Serialize an object implementing IDictionary.  The serialized data is similar to a regular
        /// object, except that the keys of the dictionary are used instead of properties.
        /// </summary>
        /// <param name="dictionary">the dictionary object</param>
        /// <param name="currentPath">object's path</param>
        private void SerializeDictionary(IDictionary dictionary, string currentPath)
        {
            this.ObjectStart();

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
            try
            {
                foreach (DictionaryEntry pair in dictionary)
                {
                    //Serialize(pair.Key, subindent, "", null);
                    //may not work in all cases
                    this.Key(pair.Key.ToString());                    
                    object value = pair.Value;
                    if (value != null && _context.OutputTypeInformation && value.GetType() != itemType)
                    {
                        this.Cast(value.GetType());
                    }
                    Serialize(value, currentPath + "." + pair.Key.ToString(), null);
                }
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
            this.ObjectEnd();
        }

        /// <summary>
        /// Serialize an object that acts like a collection.
        /// The syntax will be: [item1, item2, item3]
        /// </summary>
        /// <param name="collection">collection</param>
        /// <param name="currentPath">the object's path</param>
        private void SerializeCollection(object collection, string currentPath)
        {
            TypeHandler handler = _context.GetTypeHandler(collection.GetType());

            bool outputTypeInfo = _context.OutputTypeInformation;
            ICollectionHandler collectionHandler = handler.GetCollectionHandler();
            Type elemType = collectionHandler.GetItemType(handler.ForType);

            this.ArrayStart();
            int index = 0;

            if (collection is ISerializationCallback)
            {
                ((ISerializationCallback)collection).OnBeforeSerialization();
            }
            try
            {
                foreach (object value in collectionHandler.GetEnumerable(collection))
                {
                    if (outputTypeInfo && value.GetType() != elemType)
                    {
                        this.Cast(value.GetType());
                    }
                    Serialize(value, currentPath + ".[" + index + "]", null);
                    index++;
                }
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

            this.ArrayEnd();
        }

        private void WriteDateTime(DateTime value)
        {
            this.SpecialValue(value.ToString());
        }

        private void WriteBoolean(bool value)
        {
            this.Value(value);
        }

        private void WriteByte(byte value)
        {
            this.Value(value);
        }
        private void WriteDBNull(DBNull value)
        {
            _writer.Write(value);
        }
        private void WriteInt16(short value)
        {
            this.Value(value);
        }
        private void WriteInt32(int value)
        {
            this.Value(value);
        }
        private void WriteInt64(long value)
        {
            this.Value(value);
        }
        private void WriteSByte(sbyte value)
        {
            this.Value(value);
        }
        private void WriteUInt16(ushort value)
        {
            this.Value(value);
        }
        private void WriteUInt32(uint value)
        {
            this.Value(value);
        }
        private void WriteUInt64(ulong value)
        {
            this.SpecialValue(string.Format("{0}", value));
        }
        private void WriteDecimal(decimal value)
        {
            this.SpecialValue(string.Format("{0}", value));
        }

        /// <summary>
        /// Writes a type cast for an object.  The cast will be one of two forms:
        /// (System.Type)
        /// or
        /// ("SomeNamespace.SomeType, SomeAssembly")
        /// </summary>
        /// <param name="t">the type to cast</param>
        private void WriteCast(Type t)
        {
            if (t != typeof(string)) {
                this.Cast(t);
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
            else if (t.IsArray)
            {
                WriteTypeInfo(t.GetElementType());
                _writer.Write("[]");
                return;
            }

            Assembly core = typeof(object).Assembly;

            if (t.IsGenericType && !t.IsGenericTypeDefinition)
            {
                WriteTypeInfo(t.GetGenericTypeDefinition()); 
                _writer.Write('<');
                bool writeComma = false;
                foreach (Type genArgType in t.GetGenericArguments())
                {
                    if (writeComma)
                        _writer.Write(',');
                    writeComma = true;
                    WriteTypeInfo(genArgType);
                }
                _writer.Write('>');
            }
            else
            {                
                if (t.Assembly == core)
                {
                    string typeName = t.FullName;
                    if (t.IsGenericTypeDefinition)
                    {
                        typeName = typeName.Substring(0, typeName.LastIndexOf('`'));
                    }
                    _writer.Write(typeName);
                }
                else
                {
                    AssemblyName asmblyName = t.Assembly.GetName();
                    _writer.Write('"');
                    _writer.Write(t.FullName);
                    _writer.Write(",");
                    if (t.Assembly.GlobalAssemblyCache)
                        _writer.Write(asmblyName.FullName);
                    else
                        _writer.Write(asmblyName.Name);

                    _writer.Write('"');
                }
            }
        }
        /*
        private static string EscapeString(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\t", "\\t").Replace("\"", "\\\"");
        }
        */

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
