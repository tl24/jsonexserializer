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
using JsonExSerializer.Framework;
using JsonExSerializer.Framework.Parsing;
using JsonExSerializer.Framework.Expressions;

namespace JsonExSerializer
{
    /// <summary>
    /// The Serializer class is the main entry point into the Serialization framework.  To
    /// get an instance of the Serializer, call the GetSerializer factory method with the type of
    /// the object that you want to Serialize or Deserialize.  
    /// </summary>
    /// <example>
    /// <c>
    ///     Serializer serializerObject = new Serializer(typeof(MyClass));
    ///     MyClass myClass = new MyClass();
    ///     /* set properties on myClass */
    ///     string data = serializerObject.Serialize(myClass);
    /// </c>
    /// </example>
    public class Serializer
    {
        private Type _serializedType;
        private SerializationContext _context;

        /// <summary>
        /// Gets a serializer for the given type
        /// </summary>
        /// <param name="t">type</param>
        /// <returns>a serializer</returns>
        [Obsolete("Use the Serializer(Type) constructor")]
        public static Serializer GetSerializer(Type t)
        {
            return GetSerializer(t, "JsonExSerializer");
        }

        /// <summary>
        /// Gets a serializer for the given type
        /// </summary>
        /// <param name="t">type</param>
        /// <returns>a serializer</returns>
        [Obsolete("Use the Serializer(Type, string) constructor")]
        public static Serializer GetSerializer(Type type, string configSection)
        {
            return new Serializer(type, configSection);
        }

        [Obsolete("Use the Serializer(Type, SerializationContext) constructor")]
        public static Serializer GetSerializer(Type type, SerializationContext context)
        {
            return new Serializer(type, context);
        }

        /// <summary>
        /// Constructs a serializer to (de)serialize the given type using the
        /// default configuration section "JsonExSerializer" if it exists.
        /// </summary>
        /// <param name="t">the type to serialize/deserialize</param>
        public Serializer(Type type)
            : this(type, "JsonExSerializer")
        {
        }


        /// <summary>
        /// Constructs a serializer to (de)serialize the given type using the
        /// specified configuration section.
        /// </summary>
        /// <param name="t">the type to serialize/deserialize</param>
        public Serializer(Type type, string configSection) 
        {
            if (type == null)
                throw new ArgumentNullException("type");
            _serializedType = type;
            _context = new SerializationContext();
            _context.SerializerInstance = this;
            XmlConfigurator.Configure(_context, configSection);
        }

        /// <summary>
        /// Constructs a serializer with an existing context  to (de)serialize the given type
        /// </summary>
        /// <param name="t">the type to serialize/deserialize</param>
        /// <param name="context"></param>
        public Serializer(Type type, SerializationContext context)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (context == null)
                throw new ArgumentNullException("context");

            _serializedType = type;
            _context = context;
            _context.SerializerInstance = this;
        }
        #region Serialization

        /// <summary>
        /// Serialize the object and write the data to the stream parameter.
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <param name="stream">stream for the serialized data</param>
        public virtual void Serialize(object o, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            using (StreamWriter sw = new StreamWriter(stream))
            {                
                Serialize(o, sw);
            }
        }
        /// <summary>
        /// Serialize the object and write the data to the writer parameter.
        /// The caller is expected to close the writer when done.
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <param name="writer">writer for the serialized data</param>
        public virtual void Serialize(object o, TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            Expression expr = CreateExpressionFromObject(o);
            WriteExpression(o, writer, expr);
        }

        protected virtual void WriteExpression(object o, TextWriter writer, Expression expr)
        {
            // NOTE: we don't dispose of the JsonWriter because we didn't create the text writer
            JsonWriter jsonWriter = new JsonWriter(writer, !this.Context.IsCompact, this.Context.TypeAliases);
            WriteComment(jsonWriter, o);
            ExpressionWriter.Write(jsonWriter, this.Context, expr);
        }

        protected virtual Expression CreateExpressionFromObject(object o)
        {
            ExpressionBuilder builder = new ExpressionBuilder(this.SerializedType, this.Context);
            Expression expr = builder.Serialize(o);
            return expr;
        }

        protected virtual void WriteComment(IJsonWriter writer, object value)
        {
            if (value != null && this.Context.OutputTypeComment)
            {
                string comment = "";
                comment += "/*" + "\r\n";
                comment += "  Created by JsonExSerializer" + "\r\n";
                comment += "  Assembly: " + value.GetType().Assembly.ToString() + "\r\n";
                comment += "  Type: " + value.GetType().FullName + "\r\n";
                comment += "*/" + "\r\n";
                writer.WriteComment(comment);
            }
        }
        /// <summary>
        /// Serialize the object and return the serialized data as a string.
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <returns>serialized data string</returns>
        public virtual string Serialize(object o)
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
        /// Read the serialized data from the stream and return the
        /// deserialized object.
        /// </summary>
        /// <param name="stream">stream to read the data from</param>
        /// <returns>the deserialized object</returns>
        public virtual object Deserialize(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            using (StreamReader sr = new StreamReader(stream))
            {
                return Deserialize(sr);
            }
        }
        /// <summary>
        /// Read the serialized data from the reader and return the
        /// deserialized object.
        /// </summary>
        /// <param name="reader">TextReader to read the data from</param>
        /// <returns>the deserialized object</returns>
        public virtual object Deserialize(TextReader reader)
        {
            Expression expr = Parse(reader);
            return Evaluate(expr);
        }

        protected virtual Expression Parse(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            Parser p = new Parser(this.SerializedType, reader, this.Context);
            return p.Parse();
        }

        protected virtual object Evaluate(Expression expression)
        {
            Evaluator eval = new Evaluator(this.Context);
            return eval.Evaluate(expression);
        }

        /// <summary>
        /// Read the serialized data from the input string and return the
        /// deserialized object.
        /// </summary>
        /// <param name="input">the string containing the serialized data</param>
        /// <returns>the deserialized object</returns>
        public object Deserialize(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
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

        /// <summary>
        /// The expected type of object to be serailized or deserialized.  This may be a base
        /// class of the actual type, including System.Object.
        /// </summary>
        public Type SerializedType
        {
            get { return this._serializedType; }
        }
    }
}
