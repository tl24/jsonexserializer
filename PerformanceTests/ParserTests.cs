using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.Parsing;
using System.IO;
using JsonExSerializer.Framework.Expressions;

namespace PerformanceTests
{
    public class ParserTests : JsonSerializerTest
    {
        List<Token> tokenCache;

        public ParserTests(PerfTestOptions options)
            : base(options)
        {
        }

        public override string SerializerType
        {
            get
            {
                return "Parser";
            }
        }

        protected override void PrepareForDeserializeTest()
        {
            base.PrepareForDeserializeTest();
            tokenCache = new List<Token>();
            using (StreamReader fr = new StreamReader(FileName))
            {
                TokenStream stream = new TokenStream(fr);
                Token tok;
                while ((tok = stream.ReadToken()) != Token.Empty)
                    tokenCache.Add(tok);
                
            }
        }

        public override object Deserialize(Type t)
        {
            Parser p = new Parser(new StaticTokenStream(tokenCache), this.serializer.Config.TypeAliases);
            Expression e = p.Parse();
            e.ResultType = t;
            foreach (IParsingStage stage in serializer.Config.ParsingStages)
            {
                e = stage.Execute(e);
            }
            Evaluator eval = new Evaluator(serializer.Config);
            return eval.Evaluate(e);
        }

        private class StaticTokenStream : ITokenStream
        {
            private Queue<Token> tokens;

            public StaticTokenStream(IEnumerable<Token> tokens)
            {
                this.tokens = new Queue<Token>(tokens);
            }


            public bool IsEmpty()
            {
                return tokens.Count == 0;
            }

            public Token PeekToken()
            {
                if (IsEmpty())
                    return Token.Empty;
                else
                    return tokens.Peek();
            }

            public Token ReadToken()
            {
                if (IsEmpty())
                    return Token.Empty;
                else
                    return tokens.Dequeue();
            }
        }
    }
}
