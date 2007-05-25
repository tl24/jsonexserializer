/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JsonExSerializer
{
    public class Tokenizer
    {

        #region Member Variables

        private TextReader _reader;
        private LinkedList<Token> _tokens;
        private char[] _symbols;
        private SerializerOptions _options;

        #endregion

        public Tokenizer(TextReader reader, SerializerOptions options)
        {
            _reader = reader;
            _tokens = new LinkedList<Token>();
            _symbols = "[]<>():,{}".ToCharArray();
            Array.Sort<char>(_symbols);
            _options = options;
        }


        public LinkedList<Token> Tokenize()
        {
            StringBuilder buffer = new StringBuilder();
            int c;
            char ch;

            while ((c = _reader.Read()) != -1) {
                ch = (char) c;
                if (IsQuoteStart(ch)) {
                    _tokens.AddLast(GetQuotedString(ch, buffer));
                } else if (IsNumberStart(ch)) {
                    _tokens.AddLast(GetNumber(ch, buffer));
                } else if (char.IsWhiteSpace(ch)) {
                    // nothing
                } else if (IsIdentifierStart(ch)) {
                    _tokens.AddLast(GetIdentifier(ch, buffer));
                } else if (IsLineCommentStart(ch)) {
                    ReadLineComment(ch);
                } else if (IsMultilineCommentStart(ch)) {
                    ReadMultilineComment(ch);
                } else if (IsSymbolStart(ch)) {
                    _tokens.AddLast(GetSymbol(ch, buffer));
                } else {
                    throw new ParseException("Invalid character");
                }
                buffer.Length = 0;
            }
            return _tokens;

        }

        #region Read Methods

        private void ReadMultilineComment(char ch)
        {
            // read until we see */
            _reader.Read(); // eat the "*" char
            char prev = ' ';
            int c;
            while ((c = _reader.Read()) != -1)
            {
                ch = (char)c;
                if (ch == '/' && prev == '*')
                    return;
                prev = ch;
            }
            // If we get here we didn't reach the end of the comment
            throw new ParseException("Unterminated multiline comment");
        }

        private void ReadLineComment(char ch)
        {
            _reader.Read(); // eat the 2nd "/" char
            int c;
            // read until the end of the line
            while ((c = _reader.Read()) != -1)
            {
                ch = (char)c;
                if (ch == '\r' && _reader.Peek() == '\n') {
                    _reader.Read();
                    return;
                } else if (ch == '\n') {
                    return;
                }
            }
        }

     
        private Token GetSymbol(char ch, StringBuilder buffer)
        {
            // we don't have any symbols at the moment that are more than one character
            // so we can just return any symbols
            return new Token(TokenType.Symbol, ch.ToString());
        }

        private Token GetIdentifier(char start, StringBuilder buffer)
        {

            buffer.Append(start);
            int c;
            char ch;
            while ((c = _reader.Peek()) != -1)
            {
                ch = (char)c;
                if (char.IsLetterOrDigit(ch) || ch == '_')
                {
                    buffer.Append(ch);
                }
                else
                {
                    return new Token(TokenType.Identifier, buffer.ToString());
                }
                _reader.Read();
            }
            return new Token(TokenType.Identifier, buffer.ToString());
        }

        private Token GetNumber(char start, StringBuilder buffer)
        {
            int c;
            char ch = start;
            buffer.Append(ch);
            int i = (start == '.') ? 1 : 0;

            while (i < 3)
            {
                switch (i)
                {
                    case 0: // first part of integer
                        GetIntegerPart(buffer);
                        ch = (char)_reader.Peek();
                        if (ch == '.')
                        {
                            i=1;
                            buffer.Append((char)_reader.Read());
                        }
                        else if (ch == 'e' || ch == 'E')
                        {
                            i = 2;
                            buffer.Append((char)_reader.Read());
                        }
                        else
                        {
                            i = 4;  //break out
                            break;
                        }
                        break;
                    case 1: // fractional part
                        GetIntegerPart(buffer);
                        ch = (char)_reader.Peek();
                        if (ch == '.')
                        {
                            throw new ParseException("Invalid number exception");
                        }
                        else if (ch == 'e' || ch == 'E')
                        {
                            i = 2;
                            buffer.Append((char)_reader.Read());
                        }
                        else
                        {
                            i = 3;
                        }
                        break;
                    case 2: // scientific notation
                        ch = (char)_reader.Peek();
                        //check for an optional sign
                        if (ch == '+' || ch == '-')
                        {
                            buffer.Append((char)_reader.Read());
                        }
                        GetIntegerPart(buffer);
                        ch = (char)_reader.Peek();
                        if (ch == '.')
                        {
                            throw new ParseException("Invalid number exception");
                        }
                        else
                        {
                            i = 3;
                        }
                        break;
                }
            }
            return new Token(TokenType.Number, buffer.ToString());
        }

        private void GetIntegerPart(StringBuilder buffer)
        {
            int c;
            char ch;
            while ((c = _reader.Peek()) != -1)
            {
                ch = (char)c;
                if (char.IsNumber(ch))
                {
                    buffer.Append(ch);
                }
                else if (ch == '.' || ch == 'e' || ch == 'E' || IsSymbolStart(ch) || char.IsWhiteSpace(ch))
                {
                    break;
                }
                else
                {
                    throw new ParseException("Invalid number, unexpected character: " + ch);
                }
                _reader.Read();
            }
        }

        private Token GetQuotedString(char start, StringBuilder buffer)
        {
            char quoteChar = start;
            bool escape = false;
            char ch;
            int c;
            while ((c = _reader.Read()) != -1) {
                ch = (char) c;

                if (escape)
                {
                    if (ch == quoteChar)
                    {
                        buffer.Append(ch);
                    }
                    else if (ch == 't')
                    {
                        buffer.Append('\t');
                    }
                    else if (ch == 'n')
                    {
                        buffer.Append('\n');
                    }
                    else if (ch == '\\')
                    {
                        buffer.Append('\\');
                    }
                    else
                    {
                        buffer.Append('\\').Append(ch);
                    }
                    escape = false;
                }
                else
                {
                    if (ch == '\\')
                    {
                        escape = true;
                    }
                    else if (ch == quoteChar)
                    {
                        return new Token(quoteChar == '"' ? TokenType.DoubleQuotedString : TokenType.SingleQuotedString, buffer.ToString());
                        buffer.Length = 0;
                    }
                    else
                    {
                        buffer.Append(ch);
                    }
                }
            }
            throw new ParseException("Unterminated string constant");
        }

        #endregion

        #region Token Predicates

        private bool IsQuoteStart(char ch)
        {
            return ch == '\'' || ch == '"';
        }

        private bool IsNumberStart(char ch)
        {
            return ch == '+' || ch == '-' || char.IsDigit(ch) || ch == '.';
        }

        private bool IsIdentifierStart(char ch)
        {
            return char.IsLetter(ch) || ch == '_';
        }

        private bool IsSymbolStart(char ch)
        {
            return Array.BinarySearch<char>(_symbols, ch) != -1;
        }

        private bool IsLineCommentStart(char ch)
        {
            return (ch == '/' && _reader.Peek() == '/');
        }

        private bool IsMultilineCommentStart(char ch)
        {
            return (ch == '/' && _reader.Peek() == '*');
        }

        #endregion

    }
}
