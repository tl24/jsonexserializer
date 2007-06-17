using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    /// <summary>
    /// An interface to control deserialization.  The OnAfterDeserialization method
    /// is called after an object has been deserialized.  All properties will be set before
    /// the method is called.
    /// </summary>
    public interface IDeserializationCallback
    {
        /// <summary>
        /// Called after an object has been deserialized
        /// </summary>
        void OnAfterDeserialization();
    }
}
