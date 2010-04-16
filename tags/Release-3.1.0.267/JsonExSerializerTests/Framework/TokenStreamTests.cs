using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer.Framework;
using System.IO;
using JsonExSerializer.Framework.Parsing;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class TokenStreamTests
    {

        [Test]
        public void DoubleQuotedStringTest()
        {
            TestTokens("\"Patrick O'Henry\"", new Token(TokenType.DoubleQuotedString, "Patrick O'Henry"));
        }

        [Test]
        public void SingleQuotedStringTest()
        {
            TestTokens("'He said \"she said\"'", new Token(TokenType.SingleQuotedString, "He said \"she said\""));
        }

        [RowTest]
        [Row("\"b4 \\b aft\"", "b4 \b aft", Description="bell")]
        [Row("\"b4 \\n aft\"", "b4 \n aft", Description = "newline")]
        [Row("\"b4 \\r aft\"", "b4 \r aft", Description = "carriage return")]
        [Row("\"b4 \\t aft\"", "b4 \t aft", Description = "tab")]
        [Row("\"b4 \\f aft\"", "b4 \f aft", Description = "form feed")]
        [Row("\"b4 \\\\ aft\"", "b4 \\ aft", Description = "backslash")]
        [Row("\"b4 \\/ aft\"", "b4 / aft", Description = "solidus")]
        [Row("\"b4 \\u0000 aft\"", "b4 \u0000 aft", Description = "unicode")]
        public void SpecialCharactersStringTest(string input, string value)
        {
            TestTokens(input, new Token(TokenType.DoubleQuotedString, value));
        }
        [Test]
        public void BooleanTrueTest()
        {
            TestTokens("true", new Token(TokenType.Identifier, "true"));
        }

        [Test]
        public void NullTest()
        {
            TestTokens("null", new Token(TokenType.Identifier, "null"));
        }

        [RowTest] 
        [Row("{", Description="Left Brace")]
        [Row("}", Description = "Right Brace")]
        [Row("[", Description = "Left Bracket")]
        [Row("]", Description = "Right Bracket")]
        [Row("<", Description = "Less Than")]
        [Row(">", Description = "Greater Than")]
        [Row("(", Description="Left Paren")]
        [Row(")", Description = "Right Paren")]
        [Row(":", Description = "Colon")]
        [Row(",", Description = "Comma")]
        [Row(".", Description = "Period")]
        [Row("$", Description = "Dollar Sign")]
        public void SymbolTests(string symbol)
        {
            TestTokens(symbol, new Token(TokenType.Symbol, symbol));  
        }

        [RowTest]
        [Row("0",Description="Zero")]
        [Row("1234", Description="Nosign Pos Integer")]
        [Row("-4567", Description="Negative Integer")]
        [Row("1234.567", Description="Unsigned positive Float")]
        [Row("-0.567", Description = "Negative Float")]
        [Row("1.34e+10", Description="Positve Float with Positive Exponent")]
        [Row("1.34e-10", Description = "Positve Float with Negative Exponent")]
        [Row("-1.34e+10", Description = "Negative Float with Positive Exponent")]
        [Row("-1.34e-10", Description = "Negative Float with Negative Exponent")]
        [Row("12e+10", Description = "Positve Integer with Positive Exponent")]
        [Row("14e-10", Description = "Positve Integer with Negative Exponent")]
        [Row("-13e+10", Description = "Negative Integer with Positive Exponent")]
        [Row("-14e-10", Description = "Negative Integer with Negative Exponent")]
        [Row(".1", Description="Leading decimal point")]
        public void NumberTests(string number)
        {
            TestTokens(number, new Token(TokenType.Number, number));
        }

        [Test]
        public void SimpleObjectTest()
        {
            string toParse = "{\"key\":\"value\"}";
            Token[] expected = new Token[] { 
                new Token(TokenType.Symbol,"{"),
                new Token(TokenType.DoubleQuotedString, "key"),
                new Token(TokenType.Symbol, ":"),
                new Token(TokenType.DoubleQuotedString, "value"),
                new Token(TokenType.Symbol,"}")
            };
            TestTokens(toParse, expected);
        }

        [Test]
        public void SimpleListTest()
        {
            string toParse = "[\"item1\",\"item2\"]";
            Token[] expected = new Token[] { 
                new Token(TokenType.Symbol,"["),
                new Token(TokenType.DoubleQuotedString, "item1"),
                new Token(TokenType.Symbol, ","),
                new Token(TokenType.DoubleQuotedString, "item2"),
                new Token(TokenType.Symbol,"]")
            };
            TestTokens(toParse, expected);
        }
        internal void TestTokens(string toParse, params Token[] expected)
        {
            TokenStream stream = new TokenStream(new StringReader(toParse));
            List<Token> actual = new List<Token>();
            while (!stream.IsEmpty())
                actual.Add(stream.ReadToken());
            Assert.IsTrue(stream.IsEmpty());
            CollectionAssert.AreEqual(expected, actual);
        }
    }       
}
