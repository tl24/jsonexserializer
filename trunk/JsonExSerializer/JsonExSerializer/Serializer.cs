/*
Copyright (c) 2007, Ted Elliott
Code licensed under the New BSD License
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace JsonExSerializer
{
    public class Serializer
    {
        private Type _serializedType;

        /// <summary>
        /// Gets a serializer for the given type
        /// </summary>
        /// <param name="t">type</param>
        /// <returns>a serializer</returns>
        public static Serializer GetSerializer(Type t)
        {
            return new Serializer(t);
        }


        private Serializer(Type t)
        {
            _serializedType = t;
        }

        public void Serialize(object o, TextWriter writer)
        {
            if (o == null) {
                writer.Write("null");
            }
            else if (o.GetType().IsPrimitive && !(o is char))
            {
                string val;
                if (o is double)
                {
                    val = ((double)o).ToString("R");
                }
                else if (o is float)
                {
                    val = ((float)o).ToString("R");
                }
                else
                {
                    val = Convert.ToString(o);
                }
                writer.Write(val);
            }
            else if (o.GetType() == typeof(string) || o.GetType() == typeof(char))
            {
                string val = o.ToString();
                writer.Write('"');
                writer.Write(val.Replace("\"", "\\\""));
                writer.Write('"');
            }
            else
            {
                SerializeObject(o, writer);
            }

        }

        private void SerializeObject(object o, TextWriter writer)
        {
            TypeHandler handler = TypeHandler.GetHandler(_serializedType);
            writer.Write("{\n");
            bool addComma = false;
            string indent = "   ";
            foreach (TypeHandlerProperty prop in handler.Properties)
            {
                if (addComma)
                {
                    writer.Write(',');
                    writer.Write(Environment.NewLine);
                }
                writer.Write(indent);
                Serialize(prop.Name, writer);
                writer.Write(":");
                Serialize(prop.GetValue(o), writer);
                addComma = true;
            }
            writer.Write(Environment.NewLine);
            writer.Write('}');
        }

        public string Serialize(object o)
        {
            TextWriter writer = new StringWriter();
            Serialize(o, writer);
            string s = writer.ToString();
            writer.Close();
            return s;
        }

        public object Deserialize(TextReader reader)
        {
            Deserializer d = new Deserializer(_serializedType, reader);
            return d.Deserialize();
        }

        public object Deserialize(string input)
        {
            StringReader rdr = new StringReader(input);
            object result = Deserialize(rdr);
            rdr.Close();
            return result;
        }
    }
}
