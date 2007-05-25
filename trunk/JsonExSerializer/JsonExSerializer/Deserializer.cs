using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JsonExSerializer
{
    public class Deserializer
    {
        private Type _deserializedType;
        private TextReader _reader;
        private LinkedList<Token> _tokens;

        public Deserializer(Type t, TextReader reader)
        {
            _deserializedType = t;
            _reader = reader;
        }

        public object Deserialize()
        {
            Tokenizer tokenizer = new Tokenizer(_reader);
            _tokens = tokenizer.Tokenize();
            Parser p = new Parser(_deserializedType, _tokens);
            return p.Parse();            
        }

    }
}
