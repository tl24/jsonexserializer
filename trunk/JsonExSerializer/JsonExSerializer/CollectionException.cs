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
    /// Exception class thrown when errors occur dealing with collections
    /// </summary>
    public class CollectionException : JsonExSerializationException
    {
        public CollectionException()
            : base()
        {
        }

        public CollectionException(string message)
            : base(message)
        {
        }

        public CollectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
