using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.TypeConversion
{
    /// <summary>
    /// An interface for converting from one type to another for serialization/deserialization.  The resulting
    /// type is left unspecified.
    /// </summary>
    /// <typeparam name="SrcT">The type of the object before serialization or after deserialization</typeparam>
    public interface IJsonTypeConverter
    {
        /// <summary>
        /// Specifies the source type that will be converted to the destination type
        /// </summary>
        Type SourceType { get; set; }

        /// <summary>
        /// Returns the destination type that will be returned by ConvertTo.
        /// </summary>
        /// <returns></returns>
        Type GetDestinationType();

        /// <summary>
        /// This method is called before serialization.  The <paramref name="item"/> parameter should be converted
        /// to a type suitable for serialization and returned.
        /// </summary>
        /// <param name="item">the item to be converted</param>
        /// <returns>the converted item to be serialized</returns>
        object ConvertFrom(object item);

        /// <summary>
        /// This method will be called upon deserialization.  The item returned from ConvertFrom on serialization
        /// will be passed as the <paramref name="item"/> parameter.  This object should be converted back to the
        /// desired type and returned.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>the desired object</returns>
        object ConvertTo(object item);

        /// <summary>
        /// Context parameter to control conversion
        /// </summary>
        object Context { set; }
    }
}
