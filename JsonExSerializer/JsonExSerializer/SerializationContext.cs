using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    /// <summary>
    /// Provides options controlling Serializing and Deserializing of objects.
    /// </summary>
    public class SerializationContext
    {
        public bool _isCompact;
        public bool _outputTypeComment;
        public bool _outputTypeInformation;

        public SerializationContext()
        {
            _isCompact = false;
            _outputTypeComment = true;
            _outputTypeInformation = true;
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

        /// <summary>
        /// If true a comment will be written out containing type information for the root object
        /// </summary>
        public bool OutputTypeComment
        {
            get { return this._outputTypeComment; }
            set { this._outputTypeComment = value; }
        }

        /// <summary>
        /// This will set the serializer to output in Json strict mode which will only
        /// output information compatible with the JSON standard.
        /// </summary>
        public void SetJsonStrictOptions()
        {
            _outputTypeComment = false;
            _outputTypeInformation = false;
        }

        /// <summary>
        /// If set to true, type information will be written when necessary to properly deserialize the 
        /// object.  This is only when the type information derived from the serialized type will not
        /// be specific enough to deserialize correctly.  
        /// </summary>
        public bool OutputTypeInformation
        {
            get { return this._outputTypeInformation; }
            set { this._outputTypeInformation = value; }
        }


    }
}
