using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Framework.Parsing
{
    public class BufferedTokenStream : ITokenStream
    {
        ITokenStream sourceStream;
        Queue<Token> tokens;
        int maxBufferSize;

        public BufferedTokenStream(ITokenStream source)
            : this(source, 0)
        {
        }
        public BufferedTokenStream(ITokenStream source, int maxBufferSize)
        {
            this.maxBufferSize = maxBufferSize;
            this.sourceStream = source;
            if (maxBufferSize == 0)
                tokens = new Queue<Token>();
            else
                tokens = new Queue<Token>(maxBufferSize);
        }

        public bool IsEmpty()
        {
            return tokens.Count == 0 && sourceStream.IsEmpty();
        }

        public Token PeekToken()
        {
            if (tokens.Count > 0)
                return tokens.Peek();
            else
            {
                if (sourceStream.IsEmpty())
                    return Token.Empty;
                else
                {
                    Token tok;
                    while ((tok = sourceStream.ReadToken()) != Token.Empty)
                    {
                        tokens.Enqueue(tok);
                        if (maxBufferSize != 0 && tokens.Count >= maxBufferSize)
                            break;
                    }
                    if (tokens.Count > 0)
                        return tokens.Peek();
                    else
                        return Token.Empty;
                }
            }
        }

        public Token ReadToken()
        {
            Token tok = PeekToken();
            if (tokens.Count > 0)
                tokens.Dequeue();
            return tok;
        }
    }
}
