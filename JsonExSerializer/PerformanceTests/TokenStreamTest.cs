using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using JsonExSerializer.Framework.Parsing;

namespace PerformanceTests
{
    public class TokenStreamTest : JsonSerializerTest
    {
        public TokenStreamTest(PerfTestOptions options)
            : base(options)
        {
        }

        public override string SerializerType
        {
            get
            {
                return "TokenStream";
            }
        }

        public override object Deserialize(Type t)
        {
            using (StreamReader fr = new StreamReader(FileName))
            {
                List<Token> tokens = new List<Token>();
                TokenStream stream = new TokenStream(fr);
                Token tok;
                while (!stream.IsEmpty())
                    tokens.Add(stream.ReadToken());

                return tokens;
            }
        }
    }
}
