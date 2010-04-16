using System;
namespace JsonExSerializer.Framework.Parsing
{
    public interface ITokenStream
    {
        bool IsEmpty();
        Token PeekToken();
        Token ReadToken();
    }
}
