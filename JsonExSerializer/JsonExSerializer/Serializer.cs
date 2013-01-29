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
        /// <summary>
        /// Constructs a serializer to (de)serialize the given type using the
        /// default configuration section "JsonExSerializer" if it exists.
        /// </summary>
        /// <param name="type">the type to serialize/deserialize</param>
        public Serializer()
            : this("JsonExSerializer")
        {
        }


        /// <summary>
        /// Constructs a serializer to (de)serialize the given type using the
        /// specified configuration section.
        /// </summary>
        /// <param name="type">the type to serialize/deserialize</param>
        public Serializer(string configSection) 
        {
            Settings = new SerializerSettings();
            XmlConfigurator.Configure(Settings, configSection);
        }

        /// <summary>
        /// Constructs a serializer with an existing context  to (de)serialize the given type
        /// </summary>
        /// <param name="type">the type to serialize/deserialize</param>
        /// <param name="settings">the serializer settings to customize how the serializer operates</param>
        public Serializer(SerializerSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            Settings = settings;
        }

        /// <summary>
        /// The configuration options for this serializer.  The Config contains
        /// options for serializing as well as serializer helper classes such as TypeConverters
        /// and CollectionHandlers.
        /// </summary>
        public ISerializerSettings Settings { get; private set; }

        /// <summary>
        /// The expected type of object to be serailized or deserialized.  This may be a base
        /// class of the actual type, including System.Object.
        /// </summary>
        

        #region Serialization

        /// <summary>
        /// Serialize the object and write the data to the stream parameter.
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <param name="stream">stream for the serialized data</param>
        /// <typeparam name="T">the type to serialize as</typeparam>
        public void Serialize<T>(T o, Stream stream)
        {
            Serialize(o, stream, typeof(T));
        }

        /// <summary>
        /// Serialize the object and write the data to the stream parameter.
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <param name="stream">stream for the serialized data</param>
        /// <param name="serializedType">the type to serialize as</param>
        public void Serialize(object o, Stream stream, Type serializedType = null)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            using (StreamWriter sw = new StreamWriter(stream))
            {
                Serialize(o, sw, serializedType);
            }
        }

        /// <summary>
        /// Serialize the object and return the serialized data as a string.
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <returns>serialized data string</returns>
        public string Serialize<T>(T o)
        {
            return Serialize(o, typeof(T));
        }
        /// <summary>
        /// Serialize the object and return the serialized data as a string.
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <returns>serialized data string</returns>
        public string Serialize(object o, Type serializedType)
        {
            using (TextWriter writer = new StringWriter())
            {
                Serialize(o, writer, serializedType);
                string s = writer.ToString();
                writer.Close();
                return s;
            }
        }

        /// <summary>
        /// Serialize the object and write the data to the writer parameter.
        /// The caller is expected to close the writer when done.
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <param name="writer">writer for the serialized data</param>
        /// <typeparam name="T">the type to serialize as</typeparam>
        public void Serialize<T>(T o, TextWriter writer)
        {
            Serialize(o, writer, typeof(T));
        }

        /// <summary>
        /// Serialize the object and write the data to the writer parameter.
        /// The caller is expected to close the writer when done.
        /// </summary>
        /// <param name="o">the object to serialize</param>
        /// <param name="writer">writer for the serialized data</param>
        /// <param name="serializedType">the type to serialize as</param>
        public virtual void Serialize(object o, TextWriter writer, Type serializedType = null)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            Expression expr = CreateExpressionFromObject(o, serializedType);
            WriteExpression(o, writer, expr);
        }

        protected virtual void WriteExpression(object o, TextWriter writer, Expression expr)
        {
            // NOTE: we don't dispose of the JsonWriter because we didn't create the text writer
            JsonWriter jsonWriter = new JsonWriter(writer, !this.Settings.IsCompact, this.Settings.TypeAliases);
            ExpressionWriter.Write(jsonWriter, this.Settings.OutputTypeInformation, expr);
        }

        protected virtual Expression CreateExpressionFromObject(object o, Type serializedType)
        {
            serializedType = serializedType ?? (o != null ? o.GetType() : typeof(object));
            ExpressionBuilder builder = new ExpressionBuilder(serializedType, this.Settings);
            Expression expr = builder.Serialize(o);
            return expr;
        }

        #endregion

        #region Deserialization

        /// <summary>
        /// Read the serialized data from the stream and return the
        /// deserialized object.  The stream will be closed when the 
        /// method returns.  To control the stream, use the overload of Deserialize
        /// that that takes a TextReader.
        /// </summary>
        /// <param name="stream">stream to read the data from</param>
        /// <typeparam name="T">The type the object was serialized as</typeparam>
        /// <returns>the deserialized object</returns>
        public T Deserialize<T>(Stream stream)
        {
            return (T)Deserialize(stream, typeof(T));
        }

        /// <summary>
        /// Read the serialized data from the stream and return the
        /// deserialized object.  The stream will be closed when the 
        /// method returns.  To control the stream, use the overload of Deserialize
        /// that that takes a TextReader.
        /// </summary>
        /// <param name="stream">stream to read the data from</param>
        /// <param name="serializedType">The type the object was serialized as</param>
        /// <returns>the deserialized object</returns>
        public object Deserialize(Stream stream, Type serializedType)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            using (StreamReader sr = new StreamReader(stream))
            {
                return Deserialize(sr, serializedType);
            }
        }

        /// <summary>
        /// Read the serialized data from the stream and update
        /// the target object.  The stream will be closed when the 
        /// method returns.  To control the stream, use the overload of Deserialize
        /// that that takes a TextReader.
        /// </summary>
        /// <param name="stream">stream to read the data from</param>
        /// <param name="target">Target object that will be updated</param>
        /// <typeparam name="T">The type the object was serialized as</typeparam>
        public void Deserialize<T>(Stream stream, T target)
        {
            Deserialize(stream, (object)target, typeof(T));
        }

        /// <summary>
        /// Read the serialized data from the stream and update
        /// the target object.  The stream will be closed when the 
        /// method returns.  To control the stream, use the overload of Deserialize
        /// that that takes a TextReader.
        /// </summary>
        /// <param name="stream">stream to read the data from</param>
        /// <param name="target">Target object that will be updated</param>
        /// <param name="serializedType">The type the object was serialized as</param>
        public void Deserialize(Stream stream, object target, Type serializedType)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            using (StreamReader sr = new StreamReader(stream))
            {
                Deserialize(sr, target, serializedType);
            }
        }

        /// <summary>
        /// Read the serialized data from the reader and return the
        /// deserialized object.
        /// </summary>
        /// <param name="reader">TextReader to read the data from</param>
        /// <typeparam name="T">The type the object was serialized as</typeparam>
        /// <returns>the deserialized object</returns>
        public T Deserialize<T>(TextReader reader)
        {
            return (T)Deserialize(reader, typeof(T));
        }

        /// <summary>
        /// Read the serialized data from the reader and returns the
        /// deserialized object.
        /// </summary>
        /// <param name="reader">TextReader to read the data from</param>
        /// <returns>the deserialized object</returns>
        public virtual object Deserialize(TextReader reader, Type serializedType)
        {
            Expression expr = Parse(reader, serializedType);
            return Evaluate(expr);
        }

        /// <summary>
        /// Read the serialized data from the input string and returns the
        /// deserialized object.
        /// </summary>
        /// <param name="input">the string containing the serialized data</param>
        /// <typeparam name="T">The type the object was serialized as</typeparam>
        /// <returns>the deserialized object</returns>
        public T Deserialize<T>(string input)
        {
            return (T)Deserialize(input, typeof(T));
        }

        /// <summary>
        /// Read the serialized data from the input string and return the
        /// deserialized object.
        /// </summary>
        /// <param name="input">the string containing the serialized data</param>
        /// <returns>the deserialized object</returns>
        public object Deserialize(string input, Type serializedType)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            using (StringReader rdr = new StringReader(input))
            {
                object result = Deserialize(rdr, serializedType);
                return result;
            }
        }

        /// <summary>
        /// Read the serialized data from the input string and update
        /// the target from the serialized data.
        /// </summary>
        /// <param name="input">the string containing the serialized data</param>
        /// <param name="target">the target to write the information to</param>
        /// <typeparam name="T">The type the object was serialized as</typeparam>
        public void Deserialize<T>(string input, T target)
        {
            Deserialize(input, target, typeof(T));
        }

        /// <summary>
        /// Read the serialized data from the input string and update
        /// the target from the serialized data.
        /// </summary>
        /// <param name="input">the string containing the serialized data</param>
        /// <param name="target">the target to write the information to</param>
        public void Deserialize(string input, object target, Type serializedType)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            using (StringReader rdr = new StringReader(input))
            {
                Deserialize(rdr, target, serializedType);
            }
        }

        /// <summary>
        /// Read the serialized data from the reader update the target.
        /// </summary>
        /// <param name="reader">TextReader to read the data from</param>
        /// <param name="target">Target object that will be updated</param>
        /// <typeparam name="T">The type the object was serialized as</typeparam>
        public virtual void Deserialize<T>(TextReader reader, T target)
        {
            Deserialize(reader, target, typeof(T));
        }

        /// <summary>
        /// Read the serialized data from the reader update the target.
        /// </summary>
        /// <param name="reader">TextReader to read the data from</param>
        /// <param name="target">Target object that will be updated</param>
        public virtual void Deserialize(TextReader reader, object target, Type serializedType)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (target.GetType().IsValueType || target.GetType().IsPrimitive)
                throw new ArgumentException("Target can not be a value type or primitive", "target");
            Expression expr = Parse(reader, serializedType);
            Evaluate(expr, target);
        }

        protected virtual Expression Parse(TextReader reader, Type serializedType)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            Parser p = new Parser(reader, this.Settings.TypeAliases);
            Expression parsedExpression = p.Parse();
            parsedExpression.ResultType = serializedType;
            foreach (IParsingStage stage in this.Settings.ParsingStages)
            {
                parsedExpression = stage.Execute(parsedExpression);
            }
            return parsedExpression;
        }

        protected virtual object Evaluate(Expression expression)
        {
            Evaluator eval = new Evaluator(this.Settings);
            try
            {
                return eval.Evaluate(expression);
            }
            catch (Exception e)
            {
                if (eval.Current != null && eval.Current.LineNumber != 0)
                {
                    string path = eval.GetCurrentPath();
                    if (!string.IsNullOrEmpty(path))
                    {
                        throw ReflectionUtils.WrapException("Error deserializing expression Path: " + path + " near Line: " + eval.Current.LineNumber + ", Position: " + eval.Current.CharacterPosition + ": " + e.Message, e);
                    }
                    else
                    {
                        throw ReflectionUtils.WrapException("Error deserializing expression near Line: " + eval.Current.LineNumber + ", Position: " + eval.Current.CharacterPosition + ": " + e.Message, e);
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        protected virtual void Evaluate(Expression expression, object target)
        {
            Evaluator eval = new Evaluator(this.Settings);
            try
            {
                eval.Evaluate(expression, target);
            }
            catch (Exception e)
            {
                if (eval.Current != null && eval.Current.LineNumber != 0)
                {
                    string path = eval.GetCurrentPath();
                    if (!string.IsNullOrEmpty(path))
                    {
                        throw ReflectionUtils.WrapException("Error deserializing expression Path: " + path + " near Line: " + eval.Current.LineNumber + ", Position: " + eval.Current.CharacterPosition + ": " + e.Message, e);
                    }
                    else
                    {
                        throw ReflectionUtils.WrapException("Error deserializing expression near Line: " + eval.Current.LineNumber + ", Position: " + eval.Current.CharacterPosition + ": " + e.Message, e);
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        #endregion
    }
}
