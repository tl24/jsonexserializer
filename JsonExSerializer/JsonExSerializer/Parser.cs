/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using JsonExSerializer.Collections;
using System.Reflection;
using JsonExSerializer.TypeConversion;
using System.IO;

namespace JsonExSerializer
{
    sealed class Parser
    {
        #region Member Variables

        private Type _deserializedType;
        private TokenStream _tokenStream;
        private SerializationContext _context;
        private Stack<string> _pathStack;
        private IDictionary<string, object> _values;
        #endregion

        #region Token Constants

        // Define some constants
        /// <summary> &gt; </summary>
        private readonly Token GenericArgsStart = new Token(TokenType.Symbol, "<");

        /// <summary> &lt; </summary>
        private readonly Token GenericArgsEnd = new Token(TokenType.Symbol, ">");

        /// <summary> new </summary>
        private readonly Token NewToken = new Token(TokenType.Identifier, "new");

        /// <summary> : </summary>
        private readonly Token ColonToken = new Token(TokenType.Symbol, ":");
        /// <summary> . </summary>
        private readonly Token PeriodToken = new Token(TokenType.Symbol, ".");

        /// <summary> , </summary>
        private readonly Token CommaToken = new Token(TokenType.Symbol, ",");

        /// <summary> ( </summary>
        private readonly Token LParenToken = new Token(TokenType.Symbol, "(");

        /// <summary> ) </summary>
        private readonly Token RParenToken = new Token(TokenType.Symbol, ")");

        /// <summary> ( </summary>
        private readonly Token LSquareToken = new Token(TokenType.Symbol, "[");

        /// <summary> ) </summary>
        private readonly Token RSquareToken = new Token(TokenType.Symbol, "]");

        /// <summary> ( </summary>
        private readonly Token LBraceToken = new Token(TokenType.Symbol, "{");

        /// <summary> ) </summary>
        private readonly Token RBraceToken = new Token(TokenType.Symbol, "}");

        /// <summary> null </summary>
        private readonly Token NullToken = new Token(TokenType.Identifier, "null");

        /// <summary> this </summary>
        private readonly Token ReferenceStartToken = new Token(TokenType.Identifier, "this");
        #endregion

        public Parser(Type t, TextReader reader, SerializationContext context)
            : this(t, new TokenStream(reader), context)
        {
        }


        public Parser(Type t, TokenStream tokenStream, SerializationContext context)
        {
            _deserializedType = t;
            _tokenStream = tokenStream;
            _context = context;
            _pathStack = new Stack<string>();
            _values = new Dictionary<string, object>();
        }

        /// <summary>
        /// Parses the stream and returns the object result
        /// </summary>
        /// <returns>the object constructed from the stream</returns>
        public object Parse()
        {
            _pathStack.Push("this");
            return ParseExpression(_deserializedType, null, false, true);
        }

        /// <summary>
        /// Peeks at the next token in the list
        /// </summary>
        /// <returns>the token</returns>
        private Token PeekToken()
        {
            return _tokenStream.PeekToken();
        }

        /// <summary>
        /// Reads the next token and removes it from the list
        /// </summary>
        /// <returns>the next toke</returns>
        private Token ReadToken()
        {
            return _tokenStream.ReadToken();
        }

        private object ParseExpression(Type desiredType, object parent, bool isConverted, bool useConverter)
        {
            object value = null;
            if (_tokenStream.IsEmpty())
            {
                value = null;
            }
            else
            {
                if (useConverter && _context.HasConverter(desiredType))
                {
                    Type originalType = desiredType;
                    IJsonTypeConverter converter = _context.GetConverter(desiredType);
                    desiredType = converter.GetSerializedType(desiredType);
                    value = ParseExpression(desiredType, parent, true, true);
                    value = converter.ConvertTo(value, originalType, _context);
                    _values[GetCurrentPath()] = value;
                }
                else if (useConverter && typeof(IJsonTypeConverter).IsAssignableFrom(desiredType))
                {
                    // object converts itself, create an instance of the object
                    IJsonTypeConverter converter = (IJsonTypeConverter)Activator.CreateInstance(desiredType);
                    Type originalType = desiredType;
                    desiredType = converter.GetSerializedType(desiredType);
                    value = ParseExpression(desiredType, parent, true, true);
                    converter.ConvertTo(value, originalType, _context);
                    value = converter;
                    _values[GetCurrentPath()] = value;
                }
                else
                {
                    Token tok = PeekToken();
                    if (tok == ReferenceStartToken)
                    {
                        value = ParseReference();
                        // return the value early to skip the IDeserializationCallback
                        // since this is a reference its already been called
                        return value;
                    }
                    else if (tok.type == TokenType.Number
                        || (IsIdentifier(tok) && !IsKeyword(tok)))
                    {
                        value = ParsePrimitive(desiredType);
                    }
                    else if (IsQuotedString(tok))
                    {
                        value = ParseString(desiredType);
                    }
                    else if (tok == LSquareToken)
                    {
                        value = ParseCollection(desiredType, isConverted);
                    }
                    else if (tok == LBraceToken)
                    {
                        value = ParseObject(desiredType, isConverted);
                    }
                    else if (tok == LParenToken)
                    {
                        value = ParseCast(desiredType, isConverted);
                    }
                    else if (tok == NewToken)
                    {
                        value = ParseConstructorSpec(desiredType, isConverted);
                    }
                    else
                    {
                        throw new ParseException("Unexpected token: " + tok);
                    }
                }
            }
            if (value is IDeserializationCallback)
            {
                ((IDeserializationCallback)value).OnAfterDeserialization();
            }
            return value;
        }

        /// <summary>
        /// Parses a reference to an object
        /// </summary>
        /// <returns></returns>
        private object ParseReference()
        {
            StringBuilder refSpec = new StringBuilder();
            Token tok = ReadToken();
            RequireToken(ReferenceStartToken, tok, "Invalid starting token for ParseReference");

            refSpec.Append(tok.value);
            while ((tok = PeekToken()) == PeriodToken)
            {
                tok = ReadToken(); // separator "."
                refSpec.Append(tok.value);
                tok = ReadToken(); // type part
                if (tok.type != TokenType.Identifier)
                {
                    throw new ParseException("Invalid Reference, must be an identifier: " + tok);
                }
                refSpec.Append(tok.value);
            }
            return _values[refSpec.ToString()];
        }

        /// <summary>
        /// Turns the current path stack into a dot-seperated list
        /// </summary>
        /// <example>this.prop1.1.value</example>
        /// <returns>dot-seperated string</returns>
        private string GetCurrentPath()
        {
            string[] pathParts = _pathStack.ToArray();
            // reverse the string so its in the right order, fifo
            Array.Reverse(pathParts);
            return string.Join(".", pathParts);
        }

        private object ParseCast(Type desiredType, bool isConverted)
        {
            Token tok = ReadToken();
            Debug.Assert(tok == LParenToken, "Invalid starting token for ParseCast: " + tok);
            desiredType = ParseTypeSpecifier();
            tok = ReadToken();
            RequireToken(RParenToken, tok, "Invalid Type Cast Syntax");
            return ParseExpression(desiredType, null, isConverted, true);
        }

        private object ParseCollection(Type desiredType, bool isConverted)
        {
            ICollectionBuilder collBuilder = null;
            Token tok = ReadToken();
            Debug.Assert(tok == LSquareToken);

            Type elemType = typeof(object);
            TypeHandler handler = _context.GetTypeHandler(desiredType);            
            if (desiredType != typeof(object))
            {                
                elemType = handler.GetCollectionItemType();
            }
            if (collBuilder == null)
            {
                if (desiredType == typeof(object))
                {
                    collBuilder = new ListCollectionBuilder(typeof(ArrayList));
                }
                else
                {
                    collBuilder = handler.GetCollectionBuilder();
                }
            }
            object item;
            int i = 0;
            _pathStack.Push(i.ToString());
            while (ReadAhead(CommaToken, RSquareToken, delegate (Type t, object o) { return ParseExpression(t, o, false, true); }, elemType, collBuilder, out item))
            {
                collBuilder.Add(item);
                _pathStack.Pop();
                i++;
                _pathStack.Push(i.ToString());
            }
            _pathStack.Pop();
            object value = collBuilder.GetResult();
            if (!isConverted)
                _values[GetCurrentPath()] = value;
            return value;
        }

        private object ParseObject(Type desiredType, bool isConverted)
        {

            if (desiredType.IsArray)
            {
                throw new ParseException("Invalid collection type for object " + desiredType);
            }
            Token tok = ReadToken();
            Debug.Assert(tok == LBraceToken);
            object value = null;
            if (desiredType == typeof(object))
            {
                value = new Hashtable();
            }
            else
            {
                value = Activator.CreateInstance(desiredType);
            }
            if (!isConverted)
                _values[GetCurrentPath()] = value;

            object item;
            while (ReadAhead(CommaToken, RBraceToken, new ParserMethod(ParseKeyValue), desiredType, value, out item))
            {
            }
            return value;
        }

        private object ParseKeyValue(Type desiredType, object instance)
        {
            Type keyType = typeof(string);
            Type valueType = typeof(object);
            if (instance is IDictionary)
            {
                if (desiredType.GetInterface(typeof(IDictionary<,>).Name) != null)
                {
                    Type genDict = desiredType.GetInterface(typeof(IDictionary<,>).Name);
                    Type[] genArgs = genDict.GetGenericArguments();
                    keyType = genArgs[0];
                    valueType = genArgs[1];
                }
            }
            object key = ParseExpression(keyType, instance, false, true);
            Token tok = ReadToken();
            RequireToken(ColonToken, tok, "Syntax error, key should be followed by :.");
            object value;
            _pathStack.Push(key.ToString());
            if (instance is IDictionary)
            {
                value = ParseExpression(valueType, null, false, true);
                ((IDictionary)instance)[key] = value;
            } 
            else 
            {
                TypeHandler handler = _context.GetTypeHandler(desiredType);
                PropertyHandler prop = handler.FindProperty(key.ToString());
                if (prop == null)
                    throw new ParseException("Could not find property: " + key + " on type: " + handler.ForType.FullName);
                if (_context.HasConverter(prop.Property))
                {
                    IJsonTypeConverter converter = _context.GetConverter(prop.Property);
                    Type propDesiredType = converter.GetSerializedType(prop.PropertyType);
                    value = ParseExpression(propDesiredType, null, true, false);
                    value = converter.ConvertTo(value, prop.PropertyType, _context);
                }
                else
                {

                    value = ParseExpression(prop.PropertyType, null, false, true);
                }
                prop.SetValue(instance, value);
            }
            _pathStack.Pop();
            return value;
            
        }

        private delegate object ParserMethod(Type desiredType, object parent);

        /// <summary>
        /// Handler for 1 or more construct
        /// </summary>
        /// <param name="separator">the separator token between items</param>
        /// <param name="terminal">the ending token</param>
        /// <param name="meth">the method to call to parse an item</param>
        /// <param name="desiredType">the desired type for the called method</param>
        /// <returns>true if match parsed, false otherwise</returns>
        private bool ReadAhead(Token separator, Token terminal, ParserMethod meth, Type desiredType, object parent, out object result)
        {
            Token tok = PeekToken();
            result = null;
            if (tok == terminal)
            {
                ReadToken();
                return false;
            }
            else if (tok == separator)
            {
                ReadToken();
            }
            result = meth(desiredType, parent);
            return true;
        }

        private object ParseConstructorSpec(Type desiredType, bool isConverted)
        {
            throw new ParseException("Constructors not supported");
            Token tok = ReadToken();    // should be the new keyword
            Debug.Assert(tok == NewToken);
            Type t = ParseTypeSpecifier();

            tok = ReadToken();
            RequireToken(LParenToken, tok, "Missing constructor arguments");
            return null;
        }

        /// <summary>
        /// Parses a type specifier, used by cast and constructor types.  The final
        /// result is "Type" which is then pushed on the values stack. 
        /// Examples:
        /// 
        /// <para>  System.Int32    -- int</para>
        /// <para>  System.Object[]   -- obect array</para>
        /// <para>  System.Collections.Generic.List&lt;System.String&gt; -- list of strings</para>
        /// <para>  System.Collections.Generic.List&lt;System.String&gt;[]  -- array of list of strings</para>
        /// <para>  System.Collections.Generic.List&lt;System.String[]&gt;  -- list of string arrays</para>
        /// </summary>
        private Type ParseTypeSpecifier()
        {
            StringBuilder typeSpec = new StringBuilder();
            Token tok = ReadToken();
            if (tok.type != TokenType.Identifier && !IsQuotedString(tok))
            {
                throw new ParseException("Type expected");
            }
            typeSpec.Append(tok.value);
            while ((tok = PeekToken()) == PeriodToken)
            {
                tok = ReadToken(); // separator "."
                typeSpec.Append(tok.value);
                tok = ReadToken(); // type part
                if (tok.type != TokenType.Identifier)
                {
                    throw new ParseException("Invalid Type specifier, must be an identifier: " + tok);
                }
                typeSpec.Append(tok.value);
            }
            // should we parse these into a type?
            // look for generic type args

            

            List<Type> genericTypes = new List<Type>();
            if (PeekToken() == GenericArgsStart)
            {
                
                tok = ReadToken();                
                genericTypes.Add(ParseTypeSpecifier());
                
                while (PeekToken() == CommaToken)
                {
                    ReadToken();    // eat the comma
                    genericTypes.Add(ParseTypeSpecifier());
                }
                tok = ReadToken();
                RequireToken(GenericArgsEnd, tok, "Unterminated generic type arguments");                
            }

            Type builtType = null;
            if (genericTypes.Count > 0)
            {
                // if its specified as a string it might already have this
                if (typeSpec.ToString().IndexOf('`') < 0)
                {
                    typeSpec.Append('`');
                    typeSpec.Append(genericTypes.Count);
                }
                //builtType = Type.GetType(typeSpec.ToString(), true);
                builtType = bindType(typeSpec.ToString());
                builtType = builtType.MakeGenericType(genericTypes.ToArray());
            }
            else
            {
                //builtType = Type.GetType(typeSpec.ToString(), true);
                builtType = bindType(typeSpec.ToString());
            }
            // array spec
            if (PeekToken() == LParenToken)
            {                
                tok = ReadToken();
                RequireToken(RParenToken, ReadToken(), "Expected array type specifier");
                builtType = builtType.MakeArrayType();
            }            

            return builtType;
        }

        private Type bindType(string typeName)
        {
            if (_context.GetTypeBinding(typeName) != null)
            {
                return _context.GetTypeBinding(typeName);
            }

            /*
            Assembly current = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            // check the calling assembly to see if the type exists in the calling assembly
            Type ct = current.GetType(typeName);
            if (ct != null)
                return ct;
            
            foreach (AssemblyName a in current.GetReferencedAssemblies())
            {
                if (typeName.StartsWith(a.Name.Replace(".dll", "")))
                {
                    Assembly assmbly = Assembly.Load(a);
                    Type t = assmbly.GetType(typeName);
                    if (t != null)
                        return t;
                }
            }
            */
            return Type.GetType(typeName, true);
        }

        /// <summary>
        /// Parses a primitive value, numeric or string
        /// </summary>
        /// <param name="desiredType">the desired type of the returned value</param>
        /// <returns>parsed value</returns>
        private object ParsePrimitive(Type desiredType)
        {
            Token tok = ReadToken();
            if (desiredType == typeof(object))
            {
                // no type info, try to figure out the closest type
                switch (tok.type)
                {
                    case TokenType.Number:
                        if (tok.value.IndexOf('.') != -1)
                        {
                            return Convert.ToDouble(tok.value);
                        }
                        else
                        {
                            return Convert.ToInt32(tok.value);
                        }
                        break;
                    case TokenType.Identifier:
                        if (tok.value.Equals("true", StringComparison.CurrentCultureIgnoreCase)
                        || tok.value.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                        {
                            return Convert.ToBoolean(tok.value);
                        }
                        else if (tok.value.Equals("null"))
                        {
                            return null;
                        }
                        else
                        {
                            return tok.value;
                        }
                        break;
                    default:
                        return tok.value;
                        break;
                }
            }
            else
            {
                if (tok == NullToken)
                {
                    return null;
                }
                else if (desiredType.IsEnum)
                {
                    return Enum.Parse(desiredType, tok.value);
                }
                else
                {
                    return Convert.ChangeType(tok.value, desiredType);
                }
            }
        }

        /// <summary>
        /// Parses a single or double quoted string, or a character
        /// </summary>
        /// <param name="desiredType">the desired return type, should be string or char</param>
        /// <returns>the parsed string or char</returns>
        private object ParseString(Type desiredType)
        {
            Token tok = ReadToken();
            string val = null;
            if (tok.type == TokenType.DoubleQuotedString)
            {
                val = tok.value.Replace("\\\"", "\"");
                val = tok.value.Replace("\\t", "\t");
                val = tok.value.Replace("\\n", "\n");
            }
            else if (tok.type == TokenType.SingleQuotedString)
            {
                val = tok.value.Replace("\\'", "'");
                val = tok.value.Replace("\\t", "\t");
                val = tok.value.Replace("\\n", "\n");
            }
            if (desiredType == typeof(char))
            {
                return Convert.ChangeType(val, typeof(char));
            }
            else
            {
                return val;
            }
        }

        /// <summary>
        /// Asserts that the token read is the one expected
        /// </summary>
        /// <param name="expected">the expected token</param>
        /// <param name="actual">the actual token</param>
        /// <param name="message">message to use in the exception if expected != actual</param>
        private void RequireToken(Token expected, Token actual, string message)
        {
            if (actual != expected)
            {
                throw new ParseException(message + " Expected: " + expected + " got: " + actual);
            }
        }

        /// <summary>
        /// Test the token to see if its a quoted string
        /// </summary>
        /// <param name="tok">the token to test</param>
        /// <returns>true if its a quoted string</returns>
        private bool IsQuotedString(Token tok)
        {
            return tok.type == TokenType.DoubleQuotedString || tok.type == TokenType.SingleQuotedString;
        }

        /// <summary>
        /// Test the token to see if its an identifier
        /// </summary>
        /// <param name="tok">the token to test</param>
        /// <returns>true if its an identifier</returns>
        private bool IsIdentifier(Token tok)
        {
            return tok.type == TokenType.Identifier;
        }

        /// <summary>
        /// Test the token to see if its a keyword
        /// </summary>
        /// <param name="tok">the token to test</param>
        /// <returns>true if its a keyword</returns>
        private bool IsKeyword(Token tok)
        {
            // include null?
            return tok.type == TokenType.Identifier && tok.value == "new";
        }
    }
}
