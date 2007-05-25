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

namespace JsonExSerializer
{
    public class Parser
    {
        private Type _deserializedType;
        private LinkedList<Token> _tokens;
        private Stack _values;

        // Define some constants
        /// <summary> &gt; </summary>
        private Token GenericArgsStart = new Token(TokenType.Symbol, "<");

        /// <summary> &lt; </summary>
        private Token GenericArgsEnd = new Token(TokenType.Symbol, ">");

        /// <summary> new </summary>
        private Token NewToken = new Token(TokenType.Identifier, "new");

        /// <summary> : </summary>
        private Token ColonToken = new Token(TokenType.Symbol, ":");
        /// <summary> . </summary>
        private Token PeriodToken = new Token(TokenType.Symbol, ".");

        /// <summary> , </summary>
        private Token CommaToken = new Token(TokenType.Symbol, ",");

        /// <summary> ( </summary>
        private Token LParenToken = new Token(TokenType.Symbol, "(");

        /// <summary> ) </summary>
        private Token RParenToken = new Token(TokenType.Symbol, ")");

        /// <summary> ( </summary>
        private Token LSquareToken = new Token(TokenType.Symbol, "[");

        /// <summary> ) </summary>
        private Token RSquareToken = new Token(TokenType.Symbol, "]");

        /// <summary> ( </summary>
        private Token LBraceToken = new Token(TokenType.Symbol, "{");

        /// <summary> ) </summary>
        private Token RBraceToken = new Token(TokenType.Symbol, "}");

        public Parser(Type t, LinkedList<Token> tokens)
        {
            _deserializedType = t;
            _tokens = tokens;
            _values = new Stack();
        }

        public object Parse()
        {
            ParseStart();
            object result = null;
            if (_values.Count > 1)
            {
                throw new ParseException("Invalid parse exception, too many resulting values");
            }
            if (_values.Count == 1)
                result = _values.Pop();

            return result;
        }

        /// <summary>
        /// Peeks at the next token in the list
        /// </summary>
        /// <returns>the token</returns>
        private Token PeekToken()
        {
            Token tok = Token.Empty;
            if (_tokens.Count > 0)
                tok = _tokens.First.Value;
            return tok;
        }

        /// <summary>
        /// Reads the next token and removes it from the list
        /// </summary>
        /// <returns>the next toke</returns>
        private Token ReadToken()
        {
            Token tok = Token.Empty;
            if (_tokens.Count > 0)
                tok = _tokens.First.Value;
            _tokens.RemoveFirst();
            return tok;
        }

        /// <summary>
        /// Start := (Key : expr) | expr
        /// </summary>
        public void ParseStart()
        {
            if (_tokens.Count > 1)
            {
                LinkedListNode<Token> node = _tokens.First;
                if (IsKey(node.Value) && IsKeyValueSep(node.Next.Value))
                {
                    _tokens.RemoveFirst();
                    _tokens.RemoveFirst();
                }
            }
            ParseExpression(_deserializedType);
        }

        private void ParseExpression(Type desiredType)
        {
            if (_tokens.Count == 0)
            {
                return;
            }
            else
            {
                Token tok = PeekToken();
                if (tok.type == TokenType.Number
                    || (IsIdentifier(tok) && !IsKeyword(tok)))
                {
                    ParsePrimitive(desiredType);
                }
                else if (IsQuotedString(tok))
                {
                    ParseString();
                }
                else if (tok == LSquareToken)
                {
                    ParseCollection(null, desiredType);
                }
                else if (tok == LBraceToken)
                {
                    ParseObject(desiredType);
                }
                else if (tok == NewToken)
                {
                    ParseConstructorSpec(desiredType);
                }
            }
        }

        private void ParseCollection(object constructedObject, Type desiredType)
        {
            if (constructedObject == null)
            {
                //TODO: more fancy stuff with ICollection interfaces and stuff
                constructedObject = Activator.CreateInstance(desiredType);
            }
            Token tok = ReadToken();
            Debug.Assert(tok == LSquareToken);
            //TODO: figure out member type
            while (ReadAhead(CommaToken, RSquareToken, new ParserMethod(ParseExpression), typeof(object)))
            {
                ((IList)constructedObject).Add(_values.Pop());
            }
        }

        private void ParseObject(Type desiredType)
        {
            Token tok = ReadToken();
            Debug.Assert(tok == LBraceToken);
            _values.Push(Activator.CreateInstance(desiredType));
            while (ReadAhead(CommaToken, RBraceToken, new ParserMethod(ParseKeyValue), desiredType))
            {
            }
        }

        private void ParseKeyValue(Type desiredType)
        {
            object instance = _values.Peek();
            Token tok = ReadToken();
            string name = tok.value;
            tok = ReadToken();
            RequireToken(ColonToken, tok, "Syntax error, key should be followed by :.");
            TypeHandler handler = TypeHandler.GetHandler(desiredType);
            TypeHandlerProperty prop = handler.FindProperty(name);
            if (prop == null)
                throw new ParseException("Could not find property: " + name + " on type: " + handler.ForType.FullName);

            ParseExpression(prop.PropertyType);
            prop.SetValue(instance, _values.Pop());
        }

        private delegate void ParserMethod(Type desiredType);

        /// <summary>
        /// Handler for 1 or more construct
        /// </summary>
        /// <param name="separator">the separator token between items</param>
        /// <param name="terminal">the ending token</param>
        /// <param name="meth">the method to call to parse an item</param>
        /// <param name="desiredType">the desired type for the called method</param>
        /// <returns>true if match parsed, false otherwise</returns>
        private bool ReadAhead(Token separator, Token terminal, ParserMethod meth, Type desiredType)
        {
            Token tok = PeekToken();
            if (tok == terminal)
            {
                ReadToken();
                return false;
            }
            else if (tok == separator)
            {
                ReadToken();
            }
            meth(desiredType);
            return true;
        }

        private void ParseConstructorSpec(Type desiredType)
        {
            Token tok = ReadToken();    // should be the new keyword
            Debug.Assert(tok == NewToken);
            ParseTypeSpecifier();

            string type = (string) _values.Pop();
            tok = ReadToken();
            RequireToken(LParenToken, tok, "Missing constructor arguments");

        }

        private void ParseTypeSpecifier()
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
            
            
            if (PeekToken() == GenericArgsStart)
            {
                tok = ReadToken();
                typeSpec.Append(tok.value);
                ParseTypeSpecifier();
                typeSpec.Append(_values.Pop());
                
                while (PeekToken() == CommaToken)
                {
                    typeSpec.Append(',');
                    ParseTypeSpecifier();
                    typeSpec.Append(_values.Pop());
                }
                tok = ReadToken();
                RequireToken(GenericArgsEnd, tok, "Unterminated generic type arguments");
                typeSpec.Append(tok.value);
            }
            _values.Push(typeSpec.ToString());
        }

        private void ParsePrimitive(Type desiredType)
        {
            Token tok = ReadToken();
            object result = Convert.ChangeType(tok.value, desiredType);
            _values.Push(result);
        }

        private void ParseString()
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
            _values.Push(val);
        }

        private void RequireToken(Token expected, Token actual, string message)
        {
            if (actual != expected)
            {
                throw new ParseException(message + " Expected: " + expected + " got: " + actual);
            }
        }

        private bool IsKeyValueSep(Token tok)
        {
            return (tok == ColonToken);
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
