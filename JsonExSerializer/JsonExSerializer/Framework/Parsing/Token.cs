/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Framework.Parsing
{
    /// <summary>
    /// The type for a given token
    /// </summary>
    public enum TokenType
    {
        Number,
        Identifier,
        DoubleQuotedString,
        SingleQuotedString,
        Symbol
    }

    /// <summary>
    /// Structure to represent a token from the input stream
    /// </summary>
    public struct Token
    {
        public static Token Empty = new Token();

        public TokenType type;
        public string value;
        public int linenumber;
        public int position;

        public Token(TokenType type, string value)
        {
            this.type = type;
            this.value = value;
            this.linenumber = 0;
            this.position = 0;
        }

        public Token(TokenType type, string value, int linenumber, int position)
        {
            this.type = type;
            this.value = value;
            this.linenumber = linenumber;
            this.position = position;
        }

        public override bool Equals(object other)
        {
            return other is Token && Equals((Token)other);
        }

        public static bool operator ==(Token lhs, Token rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Token lhs, Token rhs)
        {
            return !lhs.Equals(rhs);
        }

        private bool Equals(Token other)
        {
            return type == other.type && value == other.value;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} {2}", GetType().Name, type, value);
        }

        public override int GetHashCode()
        {
            return ((string)(type.ToString() + ":" + value.ToString())).GetHashCode();
        }
    }
}
