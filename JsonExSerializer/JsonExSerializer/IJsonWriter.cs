using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    public interface IJsonWriter : IDisposable
    {
        /*
        IJsonWriter ConstructorStart(Type constructorType);

        // arguments??

        IJsonWriter ConstructorEnd();
         */
 

        /// <summary>
        /// Starts an object
        /// </summary>
        /// <returns>the writer instance for stacking</returns>
        IJsonWriter ObjectStart();

        IJsonWriter Key(string key);

        /// <summary>
        /// Ends an object definition
        /// </summary>
        /// <returns>the writer instance for stacking</returns>
        IJsonWriter ObjectEnd();

        /// <summary>
        /// Starts an array sequence
        /// </summary>
        /// <returns></returns>
        IJsonWriter ArrayStart();

        /// <summary>
        /// Ends an array
        /// </summary>
        /// <returns></returns>
        IJsonWriter ArrayEnd();

        /// <summary>
        /// Writes a boolean value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IJsonWriter Value(bool value);

        /// <summary>
        /// Writes a long value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IJsonWriter Value(long value);

        /// <summary>
        /// Writes a double value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IJsonWriter Value(double value);

        /// <summary>
        /// Writes a float value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IJsonWriter Value(float value);

        /// <summary>
        /// Writes a quoted value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IJsonWriter QuotedValue(string value);

        /// <summary>
        /// Writes a special string value that is not
        /// quoted such as null, or some other keyword.
        /// </summary>
        /// <param name="value">the value to write</param>
        /// <returns></returns>
        IJsonWriter SpecialValue(string value);

        /// <summary>
        /// Writes a comment.  The comment characters /* */ or // should be included in the comment string
        /// </summary>
        /// <param name="comment">the comment string</param>
        /// <returns></returns>
        IJsonWriter Comment(string comment);

        /// <summary>
        /// Writes an object cast
        /// (MyClass) ...
        /// </summary>
        /// <param name="castedType">The type for the cast</param>
        /// <returns></returns>
        IJsonWriter Cast(Type castedType);

        /// <summary>
        /// Serializes any type of object completely
        /// </summary>
        /// <param name="value">the object to write</param>
        /// <returns>json writer</returns>
        IJsonWriter WriteObject(object value);
    }
}
