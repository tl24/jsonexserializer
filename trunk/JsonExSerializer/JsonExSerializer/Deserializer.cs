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
    public class Deserializer
    {
        private Type _deserializedType;
        private TextReader _reader;
        private LinkedList<Token> _tokens;
        private SerializerOptions _options;

        public Deserializer(Type t, TextReader reader, SerializerOptions options)
        {
            _deserializedType = t;
            _reader = reader;
            _options = options;
        }

        public object Deserialize()
        {
            Tokenizer tokenizer = new Tokenizer(_reader, _options);
            _tokens = tokenizer.Tokenize();
            Parser p = new Parser(_deserializedType, _tokens, _options);
            return p.Parse();            
        }

    }
}
