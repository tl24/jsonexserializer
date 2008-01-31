using System;
using System.Reflection;
using JsonExSerializer.TypeConversion;
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
        /// Returns true if this property represents a constructor argument for the type
        /// </summary>
        bool IsConstructorArgument { get; }

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

        /// <summary>
        /// Returns true if there is a converter defined for this property
        /// </summary>
        bool HasConverter { get; }

        /// <summary>
        /// Gets or sets a TypeConverter for this property if one exists.  Setting this property
        /// will override any converter declared using attributes
        /// </summary>
        IJsonTypeConverter TypeConverter { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this property is ignored or not.  If the property
        /// is ignored it will not be serialized.
        /// </summary>
        bool Ignored { get; set; }
    }
}
