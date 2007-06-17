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

namespace JsonExSerializer
{
    public class Parser
    {
        #region Member Variables

        private Type _deserializedType;
        private TokenStream _tokenStream;
        private Stack _values;
        private SerializationContext _context;
        private Stack<string> _pathStack;
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

        #endregion

        public Parser(Type t, TokenStream tokenStream, SerializationContext context)
        {
            _deserializedType = t;
            _tokenStream = tokenStream;
            _values = new Stack();
            _context = context;
            _pathStack = new Stack<string>();
        }

        public object Parse()
        {         
            return ParseExpression(_deserializedType, null);
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

        private object ParseExpression(Type desiredType, object parent)
        {
            if (_tokenStream.IsEmpty())
            {
                return null;
            }
            else
            {
                Token tok = PeekToken();
                if (tok.type == TokenType.Number
                    || (IsIdentifier(tok) && !IsKeyword(tok)))
                {
                    return ParsePrimitive(desiredType);
                }
                else if (IsQuotedString(tok))
                {
                    return ParseString(desiredType);
                }
                else if (tok == LSquareToken)
                {
                    return ParseCollection(desiredType);
                }
                else if (tok == LBraceToken)
                {
                    return ParseObject(desiredType);
                }
                else if (tok == LParenToken)
                {
                    return ParseCast(desiredType);
                }
                else if (tok == NewToken)
                {
                    return ParseConstructorSpec(desiredType);
                }
                else
                {
                    throw new ParseException("Unexpected token: " + tok);
                }
            }
        }

        private object ParseCast(Type desiredType)
        {
            Token tok = ReadToken();
            Debug.Assert(tok == LParenToken, "Invalid starting token for ParseCast: " + tok);
            desiredType = ParseTypeSpecifier();
            tok = ReadToken();
            RequireToken(RParenToken, tok, "Invalid Type Cast Syntax");
            return ParseExpression(desiredType, null);
        }

        private object ParseCollection(Type desiredType)
        {
            ICollectionBuilder collBuilder = null;
            Token tok = ReadToken();
            Debug.Assert(tok == LSquareToken);

            IJsonTypeConverter converter = null;
            Type originalType = desiredType;
            if (_context.HasConverter(desiredType))
            {
                converter = _context.GetConverter(desiredType);
                desiredType = converter.GetSerializedType(desiredType);
            }
            Type elemType = typeof(object);
            TypeHandler handler = _context.GetTypeHandler(desiredType);            
            if (desiredType != typeof(object))
            {                
                elemType = handler.GetCollectionItemType(_context);
            }
            if (collBuilder == null)
            {
                if (desiredType == typeof(object))
                {
                    collBuilder = new ListCollectionBuilder(typeof(ArrayList));
                }
                else
                {
                    collBuilder = handler.GetCollectionBuilder(_context);
                }
            }
            object item;
            while (ReadAhead(CommaToken, RSquareToken, new ParserMethod(ParseExpression), elemType, collBuilder, out item))
            {
                collBuilder.Add(item);
            }

            object value = collBuilder.GetResult();
            if (converter != null)
            {
                value = converter.ConvertTo(value, originalType);
            }
            return value;
        }

        private object ParseObject(Type desiredType)
        {
            IJsonTypeConverter converter = null;
            Type originalType = desiredType;
            if (_context.HasConverter(desiredType))
            {
                converter = _context.GetConverter(desiredType);
                desiredType = converter.GetSerializedType(desiredType);
            }

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
            object item;
            while (ReadAhead(CommaToken, RBraceToken, new ParserMethod(ParseKeyValue), desiredType, value, out item))
            {
            }
            if (converter != null)
            {
                value = converter.ConvertTo(value, originalType);
            }
            return value;
        }

        private object ParseKeyValue(Type desiredType, object instance)
        {
            Token tok = ReadToken();
            string name = tok.value;
            tok = ReadToken();
            RequireToken(ColonToken, tok, "Syntax error, key should be followed by :.");
            object value;
            if (instance is IDictionary)
            {
                value = ParseExpression(typeof(object), null);
                ((IDictionary)instance)[name] = value;
            } 
            else 
            {
                TypeHandler handler = _context.GetTypeHandler(desiredType);
                TypeHandlerProperty prop = handler.FindProperty(name);
                if (prop == null)
                    throw new ParseException("Could not find property: " + name + " on type: " + handler.ForType.FullName);
                value = ParseExpression(prop.PropertyType, null);
                prop.SetValue(instance, value);
            }
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

        private object ParseConstructorSpec(Type desiredType)
        {
            throw new ParseException("Constructors not supported");
            Token tok = ReadToken();    // should be the new keyword
            Debug.Assert(tok == NewToken);
            Type t = ParseTypeSpecifier();

            Type type = (Type)_values.Pop();
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
            if (tok.type != TokenType.Identifier)
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
                    genericTypes.Add(ParseTypeSpecifier());
                }
                tok = ReadToken();
                RequireToken(GenericArgsEnd, tok, "Unterminated generic type arguments");                
            }

            Type builtType = null;
            if (genericTypes.Count > 0)
            {
                typeSpec.Append('`');
                typeSpec.Append(genericTypes.Count);
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

            Assembly current = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
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
            return Type.GetType(typeName, true);
        }

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
                else if (_context.HasConverter(desiredType))
                {
                    return _context.GetConverter(desiredType).ConvertTo(tok.value, desiredType);
                }
                else
                {
                    return Convert.ChangeType(tok.value, desiredType);
                }
            }
        }

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
            else if (_context.HasConverter(desiredType))
            {
                return _context.GetConverter(desiredType).ConvertTo(val, desiredType);
            }
            else
            {
                return val;
            }
        }

        private void RequireToken(Token expected, Token actual, string message)
        {
            if (actual != expected)
            {
                throw new ParseException(message + " Expected: " + expected + " got: " + actual);
            }
        }

        private bool IsKey(Token tok)
        {
            return (IsQuotedString(tok) && IsIdentifier(tok));
        }

        private bool IsQuotedString(Token tok)
        {
            return tok.type == TokenType.DoubleQuotedString || tok.type == TokenType.SingleQuotedString;
        }

        private bool IsIdentifier(Token tok)
        {
            return tok.type == TokenType.Identifier;
        }

        private bool IsKeyword(Token tok)
        {
            // include null?
            return tok.type == TokenType.Identifier && tok.value == "new";
        }
    }
}
