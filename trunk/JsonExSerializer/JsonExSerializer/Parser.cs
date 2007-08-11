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
using JsonExSerializer.Expression;

namespace JsonExSerializer
{
    sealed class Parser
    {
        #region Member Variables

        private Type _deserializedType;
        private TokenStream _tokenStream;
        private SerializationContext _context;
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
        }

        /// <summary>
        /// Parses the stream and returns the object result
        /// </summary>
        /// <returns>the object constructed from the stream</returns>
        public object Parse()
        {
            ExpressionBase expr = ParseExpression();
            expr.SetResultTypeIfNotSet(_deserializedType);
            return expr.Evaluate(this._context);
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

        private ExpressionBase ParseExpression()
        {
            ExpressionBase value;
            if (_tokenStream.IsEmpty())
            {
                value = new NullExpression();
            }
            else
            {
                Token tok = PeekToken();
                if (tok == ReferenceStartToken)
                    value = ParseReference();
                else if (tok.type == TokenType.Number
                    || (IsIdentifier(tok) && !IsKeyword(tok)))
                {
                    value = ParsePrimitive();
                }
                else if (IsQuotedString(tok))
                {
                    value = ParseString();
                }
                else if (tok == LSquareToken)
                {
                    value = ParseCollection();
                }
                else if (tok == LBraceToken)
                {
                    value = ParseObject();
                }
                else if (tok == LParenToken)
                {
                    value = ParseCast();
                }
                else if (tok == NewToken)
                {
                    value = ParseConstructedObject();
                }
                else
                {
                    throw new ParseException("Unexpected token: " + tok);
                }
            }
            return value;
        }

        /// <summary>
        /// Parses a reference to an object
        /// </summary>
        /// <returns></returns>
        private ExpressionBase ParseReference()
        {
            ReferenceIdentifier refID = new ReferenceIdentifier();
            Token tok = ReadToken();
            RequireToken(ReferenceStartToken, tok, "Invalid starting token for ParseReference");

            refID.AddPart(tok.value);
            while ((tok = PeekToken()) == PeriodToken)
            {
                tok = ReadToken(); // separator "."
                tok = ReadToken(); // type part
                if (tok == LSquareToken)
                {
                    refID.AddPart(ReadToken().value); // index
                    ReadToken(); // ]
                }
                else if (tok.type == TokenType.Identifier)
                {
                    refID.AddPart(tok.value);
                }
                else
                {
                    throw new ParseException("Invalid Reference, must be an identifier or array value: " + tok);
                }                
            }
            return new ReferenceExpression(refID);
        }

        /// <summary>
        /// Parses a type cast.  Will also parse the expression that the cast applies to.
        /// The result will be the expression following the cast with the ResultType set to
        /// the type of the cast.
        /// </summary>
        /// <returns>casted expression</returns>
        private ExpressionBase ParseCast()
        {
            Token tok = ReadToken();
            Debug.Assert(tok == LParenToken, "Invalid starting token for ParseCast: " + tok);
            Type desiredType = ParseTypeSpecifier();
            tok = ReadToken();
            RequireToken(RParenToken, tok, "Invalid Type Cast Syntax");
            ExpressionBase result = ParseExpression();
            result.ResultType = desiredType;
            return result;
        }

        /// <summary>
        /// Parses a javascript array
        /// </summary>
        /// <returns></returns>
        private ListExpression ParseCollection()
        {
            Token tok = ReadToken();
            Debug.Assert(tok == LSquareToken);
            ListExpression value = new ListExpression();
            ExpressionBase item;
            while (ReadAhead(CommaToken, RSquareToken, new ExpressionMethod(ParseExpression), out item))
            {
                value.Add(item);
            }
            return value;
        }

        /// <summary>
        /// Parses a javascript object
        /// </summary>
        /// <returns></returns>
        private ObjectExpression ParseObject()
        {
            Token tok = ReadToken();
            Debug.Assert(tok == LBraceToken);
            ObjectExpression value = new ObjectExpression();
            ExpressionBase item;
            while (ReadAhead(CommaToken, RBraceToken, new ExpressionMethod(ParseKeyValue), out item))
            {
                value.Add((KeyValueExpression)item);
            }
            return value;
        }

        /// <summary>
        /// Parses a key value pair of an javascript object
        /// </summary>
        /// <returns></returns>
        private KeyValueExpression ParseKeyValue()
        {
            ExpressionBase key = ParseExpression(); // should be a value
            Token tok = ReadToken();
            RequireToken(ColonToken, tok, "Syntax error, key should be followed by :.");
            ExpressionBase value = ParseExpression();
            return new KeyValueExpression(key, value);
        }

        /// <summary>
        /// Delegate method to call for the ReadAhead method.  The method will be called for
        /// each item found by the ReadAhead method.
        /// </summary>
        /// <returns></returns>
        private delegate ExpressionBase ExpressionMethod();

        /// <summary>
        /// Handler for 1 or more construct
        /// </summary>
        /// <param name="separator">the separator token between items</param>
        /// <param name="terminal">the ending token</param>
        /// <param name="meth">the method to call to parse an item</param>
        /// <param name="result">the parsed expression</param>
        /// <returns>true if match parsed, false otherwise</returns>
        private bool ReadAhead(Token separator, Token terminal, ExpressionMethod meth, out ExpressionBase result)
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
            result = meth();
            return true;
        }

        /// <summary>
        /// Parses a constructor expression
        /// </summary>
        /// <returns>complex expression</returns>
        /// <example>
        ///    new MyType("arg1", "arg2")
        ///    new MyType("argA", "argB") { "argC": "C", "argD": "D" }
        /// </example>
        private ComplexExpressionBase ParseConstructedObject()
        {
            Token tok = ReadToken();    // should be the new keyword
            Debug.Assert(tok == NewToken);
            Type t = ParseTypeSpecifier();

            tok = ReadToken();
            RequireToken(LParenToken, tok, "Missing constructor arguments");
            ExpressionBase arg;
            List<ExpressionBase> ConstructorArgs = new List<ExpressionBase>();
            while (ReadAhead(CommaToken, RParenToken, new ExpressionMethod(ParseExpression), out arg)) {
                ConstructorArgs.Add(arg);
            }
            ComplexExpressionBase value = null;
            if (PeekToken() == LSquareToken || PeekToken() == LBraceToken) {
                value = (ComplexExpressionBase) ParseExpression();
            } else {
                value = new ObjectExpression();
            }
            value.ResultType = t;
            value.ConstructorArguments = ConstructorArgs;
            return value;
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
            if (PeekToken() == LSquareToken)
            {                
                tok = ReadToken();
                RequireToken(RSquareToken, ReadToken(), "Expected array type specifier");
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

        private ExpressionBase ParsePrimitive() {
            Token tok = ReadToken();
                // no type info, try to figure out the closest type
            switch (tok.type)
            {
                case TokenType.Number:
                    return new NumericExpression(tok.value);
                    break;
                case TokenType.Identifier:
                    if (tok.value.Equals("true", StringComparison.CurrentCultureIgnoreCase)
                    || tok.value.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return new BooleanExpression(tok.value);
                    }
                    else if (tok.value.Equals("null"))
                    {
                        return new NullExpression();
                    }
                    else
                    {
                        return new ValueExpression(tok.value);
                    }
                    break;
                default:
                    return new ValueExpression(tok.value);
                    break;
            }
        }

        /// <summary>
        /// Parses a single or double quoted string, or a character
        /// </summary>
        /// <param name="desiredType">the desired return type, should be string or char</param>
        /// <returns>the parsed string or char</returns>
        private ExpressionBase ParseString()
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
            return new ValueExpression(val);
        }

        /// <summary>
        /// Asserts that the token read is the one expected
        /// </summary>
        /// <param name="expected">the expected token</param>
        /// <param name="actual">the actual token</param>
        /// <param name="message">message to use in the exception if expected != actual</param>
        private static void RequireToken(Token expected, Token actual, string message)
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
        private static bool IsQuotedString(Token tok)
        {
            return tok.type == TokenType.DoubleQuotedString || tok.type == TokenType.SingleQuotedString;
        }

        /// <summary>
        /// Test the token to see if its an identifier
        /// </summary>
        /// <param name="tok">the token to test</param>
        /// <returns>true if its an identifier</returns>
        private static bool IsIdentifier(Token tok)
        {
            return tok.type == TokenType.Identifier;
        }

        /// <summary>
        /// Test the token to see if its a keyword
        /// </summary>
        /// <param name="tok">the token to test</param>
        /// <returns>true if its a keyword</returns>
        private static bool IsKeyword(Token tok)
        {
            // include null?
            return tok.type == TokenType.Identifier && tok.value == "new";
        }
    }
}
