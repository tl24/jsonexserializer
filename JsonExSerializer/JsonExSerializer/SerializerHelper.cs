using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using JsonExSerializer.TypeConversion;
using JsonExSerializer.Collections;

namespace JsonExSerializer
{
    public class SerializerHelper
    {
        private Type _serializedType;
        private SerializationContext _context;
        private TextWriter _writer;
        private const int indentStep = 3;
        private IDictionary<object, string> _refs;

        internal SerializerHelper(Type t, SerializationContext context, TextWriter writer)
        {
            _serializedType = t;
            _context = context;
            _writer = writer;
            _refs = new Dictionary<object, string>(new ReferenceEqualityComparer<object>());
        }

        public void Serialize(object o)
        {
            if (o != null && _context.OutputTypeComment)
            {
                _writer.WriteLine("/*");
                _writer.WriteLine("  Created by JsonExSerializer");
                _writer.WriteLine("  Assembly: " + o.GetType().Assembly.ToString());
                _writer.WriteLine("  Type: " + o.GetType().FullName);
                _writer.WriteLine("*/");
            }
            if (o != null && _context.OutputTypeInformation && o.GetType() != _serializedType)
            {
                WriteCast(o.GetType());
            }
            Serialize(o, 0, "this");
            
        }

        public void Serialize(object o, int indent, string currentPath)
        {
            if (o == null)
            {
                _writer.Write("null");
            }
            else
            {
                switch (Type.GetTypeCode(o.GetType()))
                {
                    case TypeCode.Char:
                        WriteChar((char)o, indent);
                        break;
                    case TypeCode.String:
                        WriteString((string)o, indent);
                        break;
                    case TypeCode.Double:
                        WriteDouble((double) o, indent);
                        break;
                    case TypeCode.Single:
                        WriteFloat((float)o, indent);
                        break;
                    case TypeCode.Object:
                        if (_refs.ContainsKey(o))
                        {
                            string refPath = _refs[o];
                            switch (_context.ReferenceWritingType)
                            {
                                //case SerializationContext.ReferenceOption.WriteIdentifier:
                                //    _writer.Write(refPath);
                                //    return;
                                case SerializationContext.ReferenceOption.IgnoreCircularReferences:
                                    if (currentPath.StartsWith(refPath))
                                    {
                                        _writer.Write("null");
                                        return;
                                    }
                                    break;
                                case SerializationContext.ReferenceOption.ErrorCircularReferences:
                                    if (currentPath.StartsWith(refPath))
                                    {
                                        throw new ApplicationException("Circular reference detected.  Current path: " + currentPath + ", reference to: " + refPath);
                                    }
                                    break;
                            }
                        } else {
                            _refs[o] = currentPath;
                        }
                        if (_context.HasConverter(o.GetType()))
                        {
                            IJsonTypeConverter converter = _context.GetConverter(o.GetType());
                            o = converter.ConvertFrom(o);
                            Serialize(o, indent, currentPath);
                        }
                        TypeHandler handler = _context.GetTypeHandler(o.GetType());
                        if (handler.IsCollection(_context))
                        {
                            SerializeCollection(o, indent, currentPath);
                        }
                        else if (o is ICollection && !(o is IDictionary))
                        {
                            throw new ApplicationException(o.GetType() + " is a collection but doesn't have a CollectionHandler");
                        }
                        else
                        {
                            SerializeObject(o, indent, currentPath);
                        }
                        break;
                    case TypeCode.DateTime:
                        WriteDateTime((DateTime)o, indent);
                        break;
                    case TypeCode.Boolean:
                        WriteBoolean((bool)o, indent);
                        break;
                    case TypeCode.Byte:
                        WriteByte((byte)o, indent);
                        break;
                    case TypeCode.DBNull:
                        WriteDBNull((DBNull)o, indent);
                        break;
                    case TypeCode.Empty:
                        throw new Exception("Unsupported value (Empty): " + o);
                    case TypeCode.Int16:
                        WriteInt16((short)o, indent);
                        break;
                    case TypeCode.Int32:
                        WriteInt32((int)o, indent);
                        break;
                    case TypeCode.Int64:
                        WriteInt64((long)o, indent);
                        break;
                    case TypeCode.SByte:
                        WriteSByte((sbyte)o, indent);
                        break;

                    case TypeCode.UInt16:
                        WriteUInt16((ushort)o, indent);
                        break;
                    case TypeCode.UInt32:
                        WriteUInt32((uint)o, indent);
                        break;
                    case TypeCode.UInt64:
                        WriteUInt64((ulong)o, indent);
                        break;
                    case TypeCode.Decimal:
                        WriteDecimal((decimal)o, indent);
                        break;
                    default:
                        _writer.Write(Convert.ToString(o));
                        break;
                }
            }
        }

        private void SerializeEnum(object o, int indent)
        {
            _writer.Write(Enum.Format(o.GetType(), o, "d"));
        }

        private void SerializeObject(object o, int indent, string currentPath)
        {
            if (o is IDictionary)
            {
                SerializeDictionary(o, indent, currentPath);
                return;
            }

            TypeHandler handler = _context.GetTypeHandler(o.GetType());
            
            bool addComma = false;
            _writer.Write('{');
            // indent if not in compact mode
            int subindent = 0;
            if (!_context.IsCompact)
            {
                _writer.Write('\n');
                subindent = indent + indentStep;
            }
            foreach (TypeHandlerProperty prop in handler.Properties)
            {
                if (addComma)
                {
                    _writer.Write(", ");
                    if (!_context.IsCompact) _writer.Write(Environment.NewLine);
                }
                _writer.Write("".PadLeft(subindent));
                Serialize(prop.Name, subindent, "");
                _writer.Write(":");
                object value = prop.GetValue(o);
                if (value != null && _context.OutputTypeInformation && value.GetType() != prop.PropertyType)
                {
                    WriteCast(value.GetType());
                }
                Serialize(value, subindent, currentPath + "." + prop.Name);
                addComma = true;
            }
            if (!_context.IsCompact)
            {
                _writer.Write(Environment.NewLine);
                _writer.Write("".PadLeft(indent));
            }
            _writer.Write('}');
        }

        private void SerializeDictionary(object o, int indent, string currentPath)
        {            
            bool addComma = false;
            _writer.Write('{');
            // indent if not in compact mode
            int subindent = 0;
            if (!_context.IsCompact)
            {
                _writer.Write('\n');
                subindent = indent + indentStep;
            }

            Type itemType = typeof(object);
            Type genericDictionary = null;

            if ((genericDictionary = o.GetType().GetInterface(typeof(IDictionary<,>).Name)) != null)
            {
                itemType = genericDictionary.GetGenericArguments()[1];
            }

            
            foreach (DictionaryEntry pair in ((IDictionary)o))
            {
                if (addComma)
                {
                    _writer.Write(", ");
                    if (!_context.IsCompact) _writer.Write(Environment.NewLine);
                }
                _writer.Write("".PadLeft(subindent));

                Serialize(pair.Key, subindent, "");
                _writer.Write(":");
                object value = pair.Value;
                if (value != null && _context.OutputTypeInformation && value.GetType() != itemType)
                {
                    WriteCast(value.GetType());
                }
                Serialize(value, subindent, currentPath + "." + pair.Key.ToString());
                addComma = true;
            }
            if (!_context.IsCompact)
            {
                _writer.Write(Environment.NewLine);
                _writer.Write("".PadLeft(indent));
            }
            _writer.Write('}');
        }


        private void SerializeCollection(object o, int indent, string currentPath)
        {
            TypeHandler handler = _context.GetTypeHandler(o.GetType());

            bool outputTypeInfo = _context.OutputTypeInformation;
            ICollectionHandler collectionHandler = handler.GetCollectionHandler(_context);
            Type elemType = collectionHandler.GetItemType(handler.ForType);
            

            bool addComma = false;
            _writer.Write('[');
            // indent if not in compact mode
            int subindent = 0;
            if (!_context.IsCompact)
            {
                _writer.Write('\n');
                subindent = indent + indentStep;
            }
            int index = 0;
            foreach (object value in collectionHandler.GetEnumerable(o))
            {
                if (addComma)
                {
                    _writer.Write(", ");
                    if (!_context.IsCompact) _writer.Write(Environment.NewLine);
                }
                _writer.Write("".PadLeft(subindent));
                if (outputTypeInfo && value.GetType() != elemType)
                {
                    WriteCast(value.GetType());
                }
                Serialize(value, subindent, currentPath + "." + index);
                addComma = true;
                index++;
            }
            if (!_context.IsCompact)
            {
                _writer.Write(Environment.NewLine);
                _writer.Write("".PadLeft(indent));
            }
            _writer.Write(']');
        }

        protected void WriteDateTime(DateTime value, int indent)
        {
            _writer.Write(value.ToString());
        }
        protected void WriteBoolean(bool value, int indent)
        {
            _writer.Write(value);
        }
        protected void WriteByte(byte value, int indent)
        {
            _writer.Write(value);
        }
        protected void WriteDBNull(DBNull value, int indent)
        {
            _writer.Write(value);
        }
        protected void WriteInt16(short value, int indent)
        {
            _writer.Write(value);
        }
        protected void WriteInt32(int value, int indent)
        {
            _writer.Write(value);
        }
        protected void WriteInt64(long value, int indent)
        {
            _writer.Write(value);
        }
        protected void WriteSByte(sbyte value, int indent)
        {
            _writer.Write(value);
        }
        protected void WriteUInt16(ushort value, int indent)
        {
            _writer.Write(value);
        }
        protected void WriteUInt32(uint value, int indent)
        {
            _writer.Write(value);
        }
        protected void WriteUInt64(ulong value, int indent)
        {
            _writer.Write(value);
        }
        protected void WriteDecimal(decimal value, int indent)
        {
            _writer.Write(value);
        }

        protected void WriteDouble(double value, int indent)
        {
            _writer.Write(value.ToString("R"));
        }

        protected void WriteFloat(float value, int indent)
        {
            _writer.Write(value.ToString("R"));
        }

        protected void WriteChar(char value, int indent)
        {
            _writer.Write('"');
            _writer.Write(EscapeString(value.ToString()));
            _writer.Write('"');
        }

        protected void WriteString(string value, int indent)
        {
            _writer.Write('"');
            _writer.Write(EscapeString(value));
            _writer.Write('"');
        }

        private void WriteCast(Type t)
        {
            if (t != typeof(string)) {
                _writer.Write('(');
                //TODO: Write simple type name for primitive types 
                // such as "int" instead of "System.Int32"
                WriteTypeInfo(t);
                _writer.Write(')');
            }
        }

        /// <summary>
        /// Writes out the type for an object in regular C# code syntax
        /// </summary>
        /// <param name="t">the type to write</param>
        private void WriteTypeInfo(Type t)
        {
            string alias = _context.GetTypeAlias(t);
            if (alias != null) {
                _writer.Write(alias);
            }
            else if (t.IsArray)
            {
                WriteTypeInfo(t.GetElementType());
                _writer.Write("[]");
            }
            else if (t.IsGenericType)
            {
                string genericName = _context.GetTypeAlias(t.GetGenericTypeDefinition());
                if (genericName == null)
                {
                    genericName = t.GetGenericTypeDefinition().FullName;
                    genericName = genericName.Substring(0, genericName.LastIndexOf('`'));
                }
                _writer.Write(genericName);
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
                _writer.Write(t.FullName);
            }
        }

        private string EscapeString(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\t", "\\t").Replace("\"", "\\\"");
        }

    }
}
