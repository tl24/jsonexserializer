/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
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
        private SerializationContext _context;

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
            _context = new SerializationContext(this);
        }

        #region Serialization

        public void Serialize(object o, TextWriter writer)
        {
            SerializerHelper helper = new SerializerHelper(_serializedType, _context, writer);
            helper.Serialize(o);

        }

        public string Serialize(object o)
        {
            TextWriter writer = new StringWriter();
            Serialize(o, writer);
            string s = writer.ToString();
            writer.Close();
            return s;
        }

        #endregion

        #region Deserialization

        public object Deserialize(TextReader reader)
        {
            Deserializer d = new Deserializer(_serializedType, reader, _context);
            return d.Deserialize();
        }

        public object Deserialize(string input)
        {
            StringReader rdr = new StringReader(input);
            object result = Deserialize(rdr);
            rdr.Close();
            return result;
        }

        #endregion

        public SerializationContext Context
        {
            get { return this._context; }
        }

    }
}
