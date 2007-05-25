using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    /// <summary>
    /// Provides options controlling Serializing and Deserializing of objects.
    /// </summary>
    public class SerializerOptions
    {
        public bool _isCompact;

        public SerializerOptions()
        {
        }

        /// <summary>
        /// If true, string output will be as compact as possible with minimal spacing.  Thus, cutting
        /// down on space.  This option has no effect on Deserialization.
        /// </summary>
        public bool IsCompact
        {
            get { return this._isCompact; }
            set { this._isCompact = value; }
        }


    }
}
