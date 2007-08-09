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
    /// <summary>
    /// The Serializer class is the main entry point into the Serialization framework.  To
    /// get an instance of the Serializer, call the GetSerializer factory method with the type of
    /// the object that you want to Serialize or Deserialize.  
    /// </summary>
    /// <example>
    /// <c>
    ///     Serializer serializerObject = Serializer.GetSerializer(typeof(MyClass));
    ///     MyClass myClass = new MyClass();
    ///     /* set properties on myClass */
    ///     string data = serializerObject.Serialize(myClass);
    /// </c>
    /// </example>
    public sealed class Serializer
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

        /// <summary>
        /// Serialize the object and write the data to the writer parameter.
        /// The caller is expected to close the writer when done.
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <param name="writer">writer for the serialized data</param>
        public void Serialize(object o, TextWriter writer)
        {
            SerializerHelper helper = new SerializerHelper(_serializedType, _context, writer);
            helper.Serialize(o);

        }

        /// <summary>
        /// Serialize the object and return the serialized data as a string.
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <returns>serialized data string</returns>
        public string Serialize(object o)
        {
            using (TextWriter writer = new StringWriter())
            {
                Serialize(o, writer);
                string s = writer.ToString();
                writer.Close();
                return s;
            }
        }

        #endregion

        #region Deserialization

        /// <summary>
        /// Read the serialized data from the reader and return the
        /// deserialized object.
        /// </summary>
        /// <param name="reader">TextReader to read the data from</param>
        /// <returns>the deserialized object</returns>
        public object Deserialize(TextReader reader)
        {            
            Parser p = new Parser(_serializedType, reader, _context);
            return p.Parse();
        }

        /// <summary>
        /// Read the serialized data from the input string and return the
        /// deserialized object.
        /// </summary>
        /// <param name="input">the string containing the serialized data</param>
        /// <returns>the deserialized object</returns>
        public object Deserialize(string input)
        {
            StringReader rdr = new StringReader(input);
            object result = Deserialize(rdr);
            rdr.Close();
            return result;
        }

        #endregion

        /// <summary>
        /// The Serialization context for this serializer.  The SerializationContext contains
        /// options for serializing as well as serializer helper classes such as TypeConverters
        /// and CollectionHandlers.
        /// </summary>
        public SerializationContext Context
        {
            get { return this._context; }
        }

    }
}
