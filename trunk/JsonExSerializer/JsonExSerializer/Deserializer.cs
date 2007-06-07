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
        private SerializationContext _options;

        public Deserializer(Type t, TextReader reader, SerializationContext options)
        {
            _deserializedType = t;
            _reader = reader;
            _options = options;
        }

        public object Deserialize()
        {
            TokenStream tokenStream = new TokenStream(_reader, _options);
            Parser p = new Parser(_deserializedType, tokenStream, _options);
            return p.Parse();            
        }

    }
}
