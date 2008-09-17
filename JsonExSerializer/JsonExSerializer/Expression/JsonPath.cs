/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Expression
{    
    /// <summary>    
    /// a class for working with a reference identifier
    /// e.g. this.Customer.Address[1].Name;
    /// </summary>
    public sealed class JsonPath 
    {
        private Queue<string> parts;

        public const string Root = "this";

        public JsonPath()
        {
            parts = new Queue<string>();
        }

        public JsonPath(string partsString) : this()
        {
            string[] partsArray = partsString.Split('.');
            foreach (string p in partsArray)
                parts.Enqueue(p);

        }
        /// <summary>
        /// Adds a part to the reference.  A part
        /// is one value between the period separators of a reference.
        /// </summary>
        /// <param name="part">the part to add</param>
        public void AddPart(string part)
        {
            parts.Enqueue(part);
        }

        /// <summary>
        /// The current piece of the reference
        /// </summary>
        public string Top {
            get
            {
                return parts.Peek();
            }
        }

        /// <summary>
        /// The current piece as an integer, for collections
        /// </summary>
        public int TopAsInt {
            get
            {
                return int.Parse(parts.Peek());
            }
        }

        /// <summary>
        /// The child path
        /// </summary>
        /// <returns>the child path</returns> 
        public JsonPath ChildReference()
        {
            parts.Dequeue();
            return this;
        }

        /// <summary>
        /// Returns true if the path is empty
        /// </summary>
        public bool IsEmpty {
            get { return parts.Count == 0; }
        }

        public override string ToString()
        {
            string result = "";
            foreach (string part in parts)
            {
                if (result != string.Empty)
                    result += ".";

                result += part;
            }
            return result;
        }
    }
}
