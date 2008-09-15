using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer.Framework;
using System.IO;

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
        public void SymbolTests(string symbol)
        {
            TestTokens(symbol, new Token(TokenType.Symbol, symbol));  
        }

        [RowTest]
        [Row("0",Description="Zero")]
        [Row("1234", Description="Nosign Pos Integer")]
        [Row("-4567", Description="Negative Integer")]
        [Row("+2345", Description="Signed Positive Integer")]
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
