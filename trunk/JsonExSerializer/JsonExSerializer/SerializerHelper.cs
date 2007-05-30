using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace JsonExSerializer
{
    public class SerializerHelper
    {
        private Type _serializedType;
        private SerializerOptions _options;
        private TextWriter _writer;
        private const string indentLevel = "   ";

        internal SerializerHelper(Type t, SerializerOptions options, TextWriter writer)
        {
            _serializedType = t;
            _options = options;
            _writer = writer;
        }

        public void Serialize(object o)
        {
            string indent = "";
            if (o != null && _options.OutputTypeComment)
            {
                _writer.WriteLine("/*");
                _writer.WriteLine("  Created by JsonExSerializer");
                _writer.WriteLine("  Assembly: " + o.GetType().Assembly.ToString());
                _writer.WriteLine("  Type: " + o.GetType().FullName);
                _writer.WriteLine("*/");
            }
            Serialize(o, indent);
        }

        public void Serialize(object o, string indent)
        {
            if (o == null) {
                _writer.Write("null");
            }
            else if (o.GetType().IsPrimitive && !(o is char))
            {
                string val;
                if (o is double)
                    val = ((double)o).ToString("R");
                else if (o is float)
                    val = ((float)o).ToString("R");
                else
                    val = Convert.ToString(o);

                _writer.Write(val);
            }
            else if (o.GetType() == typeof(string) || o.GetType() == typeof(char))
            {
                string val = o.ToString();
                _writer.Write('"');
                _writer.Write(EscapeString(val));
                _writer.Write('"');
            }
            else if (o is ICollection || o.GetType().IsArray)
            {
                SerializeCollection(o, indent);
            } 
            else
            {
                SerializeObject(o, indent);
            }

        }

        private void SerializeObject(object o, string indent)
        {
            TypeHandler handler = TypeHandler.GetHandler(o.GetType());
            
            bool addComma = false;
            _writer.Write('{');
            // indent if not in compact mode
            string subindent = "";
            if (!_options.IsCompact)
            {
                _writer.Write('\n');
                subindent = indent + indentLevel;
            }
            foreach (TypeHandlerProperty prop in handler.Properties)
            {
                if (addComma)
                {
                    _writer.Write(", ");
                    if (!_options.IsCompact) _writer.Write(Environment.NewLine);
                }
                _writer.Write(subindent);
                Serialize(prop.Name, subindent);
                _writer.Write(":");
                object value = prop.GetValue(o);
                if (_options._outputTypeInformation && value.GetType() != prop.PropertyType)
                {
                    WriteCast(value.GetType());
                }
                Serialize(prop.GetValue(o), subindent);
                addComma = true;
            }
            if (!_options.IsCompact)
            {
                _writer.Write(Environment.NewLine);
                _writer.Write(indent);
            }
            _writer.Write('}');
        }

        private void SerializeCollection(object o, string indent)
        {
            TypeHandler handler = TypeHandler.GetHandler(o.GetType());

            bool outputTypeInfo = _options.OutputTypeInformation;
            Type elemType = handler.GetElementType();
            

            bool addComma = false;
            _writer.Write('[');
            // indent if not in compact mode
            string subindent = "";
            if (!_options.IsCompact)
            {
                _writer.Write('\n');
                subindent = indent + indentLevel;
            }
            foreach (object value in (IEnumerable) o)
            {
                if (addComma)
                {
                    _writer.Write(", ");
                    if (!_options.IsCompact) _writer.Write(Environment.NewLine);
                }
                _writer.Write(subindent);
                if (outputTypeInfo && value.GetType() != elemType)
                {
                    WriteCast(value.GetType());
                }
                Serialize(value, subindent);
                addComma = true;
            }
            if (!_options.IsCompact)
            {
                _writer.Write(Environment.NewLine);
                _writer.Write(indent);
            }
            _writer.Write(']');
        }

        private void WriteCast(Type t)
        {
            if (t != typeof(string)) {
                _writer.Write('(');
                //TODO: Write simple type name for primitive types 
                // such as "int" instead of "System.Int32"
                _writer.Write(t.FullName);
                _writer.Write(')');
            }
        }
        private string EscapeString(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\t", "\\t").Replace("\"", "\\\"");
        }

    }
}
