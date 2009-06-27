/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JsonExSerializer.Framework.Parsing
{
    /// <summary>
    /// Tokenizes input from the specified reader and returns tokens for the parser to parse.
    /// </summary>
    public class TokenStream
    {
        #region Member Variables

        private ICharacterStream _charStream;
        private char[] _symbols;
        private bool _isEmpty = false;
        private bool _needToken = true;
        private Token _nextToken;
        #endregion

        /// <summary>
        /// Create an instance of the token stream to read from the given reader.
        /// </summary>
        /// <param name="reader"></param>
        public TokenStream(TextReader reader)
        {
            _charStream = new ReaderCharacterStream(reader);
            _charStream.LookBehind = 0;
            _symbols = "[]<>():,{}.$".ToCharArray();
            Array.Sort<char>(_symbols);
        }

        /// <summary>
        /// Peek at the next available token without consuming it.
        /// </summary>
        /// <returns>the next available token, or the empty token if all tokens have been read</returns>
        /// <see cref="Token.Empty"/>
        public Token PeekToken()
        {
            if (_needToken)
            {
                _nextToken = ReadTokenFromReader();
                _needToken = false;
            }
            return _nextToken;
        }

        /// <summary>
        /// Reads the next available token and consumes it.
        /// </summary>
        /// <returns>the next available token, or the empty token if all tokens have been read</returns>
        /// <see cref="Token.Empty"/>
        public Token ReadToken()
        {
            Token result = PeekToken();
            _needToken = !_isEmpty;
            return result;
        }

        /// <summary>
        /// Checks to see if there are any more tokens to be read
        /// </summary>
        /// <returns>true if no more tokens</returns>
        public bool IsEmpty()
        {
            return PeekToken() == Token.Empty;
        }

        /// <summary>
        /// Reads a token from the text reader and returns it
        /// </summary>
        /// <returns></returns>
        private Token ReadTokenFromReader()
        {
            int c;
            char ch;
            if (!_isEmpty)
            {
                while ((c = _charStream.LookAhead(1)) != -1)
                {
                    ch = (char)c;
                    if (IsQuoteStart(ch))
                    {
                        return GetQuotedString(ch);
                    }
                    else if (IsNumberStart(ch))
                    {
                        return GetNumber(ch);
                    }
                    else if (char.IsWhiteSpace(ch))
                    {
                        // nothing
                        _charStream.Consume();
                    }
                    else if (IsIdentifierStart(ch))
                    {
                        return GetIdentifier(ch);
                    }
                    else if (IsLineCommentStart(ch))
                    {
                        ReadLineComment(ch);
                    }
                    else if (IsMultilineCommentStart(ch))
                    {
                        ReadMultilineComment(ch);
                    }
                    else if (IsSymbolStart(ch))
                    {
                        return GetSymbol(ch);
                    }
                    else
                    {
                        throw new ParseException("Invalid character");
                    }
                }
                _isEmpty = true;
            }
            return Token.Empty;
        }

        #region Read Methods

        /// <summary>
        /// Reads a C# multiline comment
        /// <example>
        /// /*
        ///   This is a multiline comment
        /// */
        /// </example>
        /// </summary>
        /// <param name="ch">the starting character</param>
        private void ReadMultilineComment(char ch)
        {
            int restoreLookBehind = _charStream.LookBehind;
            if (_charStream.LookBehind < 2)
                _charStream.LookBehind = 2;

            // read until we see */
            _charStream.Consume(3); // eat the "/*" chars, plus the next one since we look back one
            int c;
            while ((c = _charStream.Consume()) != -1)
            {
                ch = (char)c;
                if (ch == '/' && _charStream.LookAhead(-2) == '*')
                {
                    _charStream.LookBehind = restoreLookBehind;
                    return;
                }
                
            }
            // If we get here we didn't reach the end of the comment
            throw new ParseException("Unterminated multiline comment");
        }

        /// <summary>
        /// Reads a single line comment // comment
        /// </summary>
        /// <param name="ch">the starting character</param>
        private void ReadLineComment(char ch)
        {
            _charStream.Consume(2); // eat the 2nd "/" char
            int c;
            // read until the end of the line
            while ((c = _charStream.LookAhead(1)) != -1)
            {
                ch = (char)c;
                _charStream.Consume();
                if (ch == '\r' && _charStream.LookAhead(1) == '\n')
                {
                    _charStream.Consume();
                    return;
                } else if (ch == '\n') {
                    return;
                }
            }
        }

        /// <summary>
        /// Parses a symbol from the reader such as "," "." etc
        /// </summary>
        /// <param name="ch">the starting character</param>
        /// <param name="buffer">a buffer to store input</param>
        /// <returns>symbol token</returns>
        private Token GetSymbol(char ch)
        {
            // we don't have any symbols at the moment that are more than one character
            // so we can just return any symbols
            return new Token(TokenType.Symbol, ((char)_charStream.Consume()).ToString());
        }

        /// <summary>
        /// Gets an identifier from the reader such as a variable reference, null, true, or false.
        /// Follows C# rules, non-qouted string starting with a letter or "_" followed by letters digits or "_"
        /// </summary>
        /// <param name="start">the starting character</param>
        /// <param name="buffer">a buffer to hold input</param>
        /// <returns>identifier token</returns>
        private Token GetIdentifier(char start)
        {
            int mark =_charStream.Mark();
            _charStream.Consume();
            int c;
            char ch;
            while ((c = _charStream.LookAhead(1)) != -1)
            {
                ch = (char)c;
                if (!char.IsLetterOrDigit(ch) && ch != '_')
                {
                    return new Token(TokenType.Identifier, _charStream.Substring(true));
                }
                _charStream.Consume();
            }
            return new Token(TokenType.Identifier, _charStream.Substring(true));
        }

        /// <summary>
        /// Gets a number from the reader, which can be integer, floating point or scientific notation
        /// Examples: 123343, -123232, 12.345, -45.3434, 3.45E+10
        /// </summary>
        /// <param name="start">the starting character</param>
        /// <param name="buffer">buffer to hold input</param>
        /// <returns>number token</returns>
        private Token GetNumber(char start)
        {
            char ch = start;
            int mark = _charStream.Mark();
            _charStream.Consume();
            int i = (start == '.') ? 1 : 0;

            while (i < 3)
            {
                switch (i)
                {
                    case 0: // first part of integer
                        GetIntegerPart();
                        ch = (char)_charStream.LookAhead(1);
                        if (ch == '.')
                        {
                            i=1;  // try to read fractional now
                            _charStream.Consume();
                        }
                        else if (ch == 'e' || ch == 'E')
                        {
                            i = 2; // try to read exponent now
                            _charStream.Consume();
                        }
                        else
                        {
                            i = 4;  //break out
                            break;
                        }
                        break;
                    case 1: // fractional part
                        GetIntegerPart();
                        ch = (char)_charStream.LookAhead(1);
                        if (ch == '.')
                        {
                            throw new ParseException("Invalid number exception");
                        }
                        else if (ch == 'e' || ch == 'E')
                        {
                            i = 2; // read exponent
                            _charStream.Consume();
                        }
                        else
                        {
                            i = 3; // break out
                        }
                        break;
                    case 2: // scientific notation
                        ch = (char)_charStream.LookAhead(1);
                        //check for an optional sign
                        if (ch == '+' || ch == '-')
                        {
                            _charStream.Consume();
                        }
                        GetIntegerPart();
                        if (_charStream.LookAhead(1) == '.')
                        {
                            throw new ParseException("Invalid number exception");
                        }
                        else
                        {
                            i = 3; // break out
                        }
                        break;
                }
            }
            return new Token(TokenType.Number, _charStream.Substring(true));
        }

        /// <summary>
        /// Gets an integer portion of a number, stopping at a "." or the start of an exponent "e" or "E"
        /// </summary>
        /// <param name="buffer">buffer to store input</param>
        private void GetIntegerPart()
        {
            int c;
            char ch;
            while ((c = _charStream.LookAhead(1)) != -1)
            {
                ch = (char)c;
                if (ch == '.' || ch == 'e' || ch == 'E' || IsSymbolStart(ch) || char.IsWhiteSpace(ch))
                {
                    break;
                }
                else if (!char.IsNumber(ch))
                {
                    throw new ParseException("Invalid number, unexpected character: " + ch);
                }
                _charStream.Consume();
            }
        }

        /// <summary>
        /// Gets a single or double qouted string from the reader, handling and escape characters
        /// </summary>
        /// <param name="start">the starting character</param>
        /// <param name="buffer">buffer for input</param>
        /// <returns>string token</returns>
        private Token GetQuotedString(char start)
        {
            char quoteChar = start;
            bool escape = false;
            _charStream.Consume();
            int mark = _charStream.Mark();
            char ch;
            int c;
            FastStringBuilder builder = null;   // only allocate if needed
            while ((c = _charStream.Consume()) != -1) {
                ch = (char) c;

                if (escape)
                {
                    switch (ch)
                    {
                        case 't': // horizantal tab
                            builder.Append('\t');
                            break;
                        case 'n': // newline
                            builder.Append('\n');
                            break;
                        case '\\': // reverse solidus
                            builder.Append('\\');
                            break;
                        case '/':  // solidus
                            builder.Append('/');
                            break;
                        case 'b':  // backspace
                            builder.Append('\b');
                            break;
                        case 'f':  // formfeed
                            builder.Append('\f');
                            break;
                        case 'r': // carriage return
                            builder.Append('\r');
                            break;
                        case 'u': // unicode escape sequence \unnnn
                            {
                                string ucodeChar = _charStream.Substring(_charStream.Position, _charStream.Position + 3);
                                builder.Append((char)uint.Parse(ucodeChar, System.Globalization.NumberStyles.HexNumber));
                                _charStream.Consume(4);
                            }
                            break;
                        default:
                            builder.Append(ch);
                            break;
                    }
                    escape = false;
                }
                else
                {
                    if (ch == '\\')
                    {
                        escape = true;
                        if (builder == null)
                        {
                            // we have escape sequences so we have to switch to stringbuilder
                            builder = new FastStringBuilder(_charStream.Substring(mark, _charStream.Position-2));
                            _charStream.Release(); // release the mark as we've got everything in the stringbuilder now
                        }
                    }
                    else if (ch == quoteChar)
                    {                        
                        string result = builder != null ? builder.ToString() : _charStream.Substring(mark, _charStream.Position-2, true);
                        _charStream.Release();
                        return new Token(quoteChar == '"' ? TokenType.DoubleQuotedString : TokenType.SingleQuotedString, result);
                        
                    }
                    else
                    {
                        if (builder != null)
                            builder.Append(ch);
                    }
                }
            }
            throw new ParseException("Unterminated string constant");
        }

        private char ExtractUnicode(StringBuilder builder, int index)
        {
            if (builder.Length < index + 4)
                throw new ParseException("Invalid unicode escape sequence, expecting \"\\unnnn\", but got " + builder.ToString(index, builder.Length - index));

            return (char)uint.Parse(builder.ToString(index, 4), System.Globalization.NumberStyles.HexNumber);
        }

        #endregion

        #region Token Predicates

        /// <summary>
        /// Is the character a starting quote character
        /// </summary>
        /// <param name="ch">character to test</param>
        /// <returns>true if quote start</returns>
        private static bool IsQuoteStart(char ch)
        {
            return ch == '\'' || ch == '"';
        }

        /// <summary>
        /// Is the character the start of a number
        /// </summary>
        /// <param name="ch">character to test</param>
        /// <returns>true if number start</returns>
        private bool IsNumberStart(char ch)
        {
            if (ch == '.' && char.IsDigit((char)_charStream.LookAhead(2)))
                return true;
            else
                return ch == '+' || ch == '-' || char.IsDigit(ch);
        }

        /// <summary>
        /// Is the character the start of an identifier
        /// </summary>
        /// <param name="ch">character to test</param>
        /// <returns>true if identifier start</returns>
        private static bool IsIdentifierStart(char ch)
        {
            return char.IsLetter(ch) || ch == '_';
        }

        /// <summary>
        /// Is the character the start of a symbol
        /// </summary>
        /// <param name="ch">character to test</param>
        /// <returns>true if symbol start</returns>
        private bool IsSymbolStart(char ch)
        {
            switch (ch)
            {
                case'[':
                case ']':
                case '<':
                case '>':
                case '(':
                case ')':
                case ':':
                case ',':
                case '{':
                case '}':
                case '.':
                case '$':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Is the character the start of a single line comment
        /// </summary>
        /// <param name="ch">character to start</param>
        /// <returns>true if single line comment start</returns>
        private bool IsLineCommentStart(char ch)
        {
            return (ch == '/' && _charStream.LookAhead(2) == '/');
        }

        /// <summary>
        /// Is the character the start of a multiline comment
        /// </summary>
        /// <param name="ch">character to test</param>
        /// <returns>true if multiline start</returns>
        private bool IsMultilineCommentStart(char ch)
        {
            return (ch == '/' && _charStream.LookAhead(2) == '*');
        }

        #endregion
    }
}
