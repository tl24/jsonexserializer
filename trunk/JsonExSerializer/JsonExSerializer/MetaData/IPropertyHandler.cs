using System;
using System.Reflection;
namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Metadata for a property of an object
    /// </summary>
    public interface IPropertyHandler
    {
        /// <summary>
        /// Gets the value of a property from an object instance
        /// </summary>
        /// <param name="instance">the object instance to get the value from</param>
        /// <returns>the value of the property</returns>
        object GetValue(object instance);

        /// <summary>
        /// The property name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// If this is a constructor property, its position in the constructor args
        /// </summary>
        int Position { get; }
     
        /// <summary>
        /// The Reflection PropertyInfo for this property
        /// </summary>
        PropertyInfo Property { get; }
        
        /// <summary>
        /// The system type of the property
        /// </summary>
        Type PropertyType { get; }

        /// <summary>
        /// Sets the value of the property to the specified value
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        void SetValue(object instance, object value);
    }
}
