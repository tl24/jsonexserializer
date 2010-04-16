using System;
using JsonExSerializer.TypeConversion;
namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Defines data about a property or field of a type
    /// </summary>
    public interface IPropertyData : IMetaData
    {
        /// <summary>
        /// Gets or sets a value indicating whether this property is ignored.  Ignored properties
        /// do not get written during serialization, and may not be read during deserialization depending
        /// on the <see cref="JsonExSerializer.SerializationContext.IgnoredPropertyOption" />
        /// </summary>
        bool Ignored { get; set; }

        /// <summary>
        /// Gets a value indicating whether this property is a Constructor argument.
        /// </summary>
        bool IsConstructorArgument { get; }

        /// <summary>
        /// Gets the name of this property in the type's constructor parameter list, if this property is a
        /// constructor argument.
        /// </summary>
        string ConstructorParameterName { get; set;}

        /// <summary>
        /// Gets the name of the property
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the alias for the property.  The alias will be used
        /// in JSON.
        /// </summary>
        string Alias { get; set; }

        /// <summary>
        /// Returns the 0-based index in the constructor arguments for a constructor parameter
        /// </summary>
        [Obsolete("Named constructor parameters should be used instead of Position")]
        int Position { get; set; }


        /// <summary>
        /// Gets the type of the property
        /// </summary>
        Type PropertyType { get; }

        /// <summary>
        /// Returns true if this member can be written to
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// The declaring type for this member
        /// </summary>
        Type ForType { get; }

        /// <summary>
        /// Gets or sets the TypeConverter defined for this object
        /// </summary>
        IJsonTypeConverter TypeConverter { get; set; }

        /// <summary>
        /// Returns true if there is a TypeConverter defined for this object
        /// </summary>
        bool HasConverter { get; }

        /// <summary>
        /// Gets the value of the property from the specified object instance
        /// </summary>
        /// <param name="instance">the instance of an object for the declaring type of this property</param>
        /// <returns>the value of the property</returns>
        object GetValue(object instance);

        /// <summary>
        /// Sets the value of the property on the specified object instance
        /// </summary>
        /// <param name="instance">the instance of an object for the declaring type of this property</param>
        ///<param name="value">the value to set the property to</param>
        void SetValue(object instance, object value);

        /// <summary>
        /// Check to see if this property should be written to the stream for the given value
        /// </summary>
        /// <param name="value">the value to be written</param>
        /// <returns>true if the value should be written, false otherwise</returns>
        bool ShouldWriteValue(IConfiguration config, object value);

        DefaultValueOption DefaultValueSetting { get; set; }

        object DefaultValue { get; set; }
    }
}
