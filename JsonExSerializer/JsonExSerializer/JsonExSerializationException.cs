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
    /// Base class for all serialization exceptions
    /// </summary>
    public class JsonExSerializationException : ApplicationException
    {
        public JsonExSerializationException()
            : base()
        {
        }

        public JsonExSerializationException(string message)
            : base(message)
        {
        }

        public JsonExSerializationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
