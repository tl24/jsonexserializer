/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    /// <summary>
    /// Indicates an error parsing the input stream for deserialization
    /// </summary>
    public class ParseException : JsonExSerializationException
    {
        public ParseException()
            : base()
        {
        }

        public ParseException(string message)
            : base(message)
        {
        }

        public ParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
