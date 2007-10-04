using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    public interface IJsonWriter : IDisposable
    {
        /// <summary>
        /// Starts a constructed object
        /// </summary>
        /// <param name="constructorType"></param>
        /// <returns></returns>
        IJsonWriter ConstructorStart(Type constructorType);

        /// <summary>
        /// Starts a constructed object with the given type information
        /// </summary>
        /// <param name="NamespaceAndClass">The fully-qualified class name without assembly reference</param>
        /// <returns></returns>
        IJsonWriter ConstructorStart(string NamespaceAndClass);

        /// <summary>
        /// Starts a constructed object
        /// </summary>
        /// <param name="NamespaceAndClass">The fully-qualified class name without assembly reference</param>
        /// <param name="Assembly">The assembly name</param>
        /// <returns></returns>
        IJsonWriter ConstructorStart(string NamespaceAndClass, string Assembly);

        /// <summary>
        /// Starts the arguments for a constructed object
        /// </summary>
        /// <returns></returns>
        IJsonWriter ConstructorArgsStart();

        /// <summary>
        /// Ends the arguments for a constructed object
        /// </summary>
        /// <returns></returns>
        IJsonWriter ConstructorArgsEnd();
        

        /// <summary>
        /// Ends the constructed object
        /// </summary>
        /// <returns></returns>
        IJsonWriter ConstructorEnd();
        
 

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
        /// Writes an object cast with the type name specified as a string.  The NamespaceAndClass
        /// contains the class name and possibly the Namespace but no assembly.
        /// (MyNamespace.MyClass) ...
        /// </summary>
        /// <param name="NamespaceAndClass">The fully-qualified class name without assembly reference</param>
        /// <returns></returns>
        IJsonWriter Cast(string NamespaceAndClass);

        /// <summary>
        /// Writes an object cast with the fully qualified type name and assemble reference
        /// ("MyNamespace.MyClass, MyAssembly") ...
        /// </summary>
        /// <param name="NamespaceAndClass">The fully-qualified class name without assembly reference</param>
        /// <param name="Assembly">The assembly name</param>
        /// <returns></returns>
        IJsonWriter Cast(string NamespaceAndClass, string Assembly);

        /// <summary>
        /// Serializes any type of object completely
        /// </summary>
        /// <param name="value">the object to write</param>
        /// <returns>json writer</returns>
        IJsonWriter WriteObject(object value);
    }
}
