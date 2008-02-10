using System;
using System.Collections.Generic;
using JsonExSerializer.Collections;
using System.Reflection;
using JsonExSerializer.TypeConversion;
namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// A type handler is responsible for exposing type information used to inspect objects
    /// </summary>
    public interface ITypeHandler
    {
        /// <summary>
        /// Constructs an instance of the given type passing the args array to
        /// the constructor
        /// </summary>
        /// <param name="args">the arguments to the constructor</param>
        /// <returns>object instance</returns>
        object CreateInstance(object[] args);

        /// <summary>
        /// The list of constructor parameters for this type if any
        /// </summary>
        IList<IPropertyHandler> ConstructorParameters { get; }

        /// <summary>
        /// Returns true if the type has no serializable properties
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Finds a property by name
        /// </summary>
        /// <param name="Name">the name of the property to find</param>
        /// <returns>property info</returns>
        IPropertyHandler FindProperty(string Name);

        /// <summary>
        /// Returns the properties for this type that can be serialized/deserialized
        /// </summary>
        IEnumerable<IPropertyHandler> Properties { get; }

        /// <summary>
        /// Returns all properties for this type, even those that can NOT be serialized/deserialized
        /// </summary>
        IEnumerable<IPropertyHandler> AllProperties { get; }

        /// <summary>
        /// Sets the ignore flag on the property so that it will not be serialized
        /// </summary>
        /// <param name="property">property to ignore</param>
        void IgnoreProperty(string name);


        /// <summary>
        /// The system type that this TypeHandler exposes metadata for
        /// </summary>
        Type ForType { get; }

        /// <summary>
        /// Gets a collection builder to construct a collection of the
        /// specified size.
        /// </summary>
        /// <param name="itemCount">the number of items that the collection will hold</param>
        /// <returns>a collection builder</returns>
        /// <exception cref="CollectionException">If this type is not a collection type</exception>
        ICollectionBuilder GetCollectionBuilder(int itemCount);

        /// <summary>
        /// Gets a collection builder to construct a collection to modify an existing collection
        /// </summary>
        /// <param name="itemCount">the collection to add items to</param>
        /// <returns>a collection builder</returns>
        /// <exception cref="CollectionException">If this type is not a collection type</exception>
        ICollectionBuilder GetCollectionBuilder(object collection);

        /// <summary>
        /// Returns the collection handler for this type, if the type is
        /// a collection.
        /// </summary>
        /// <returns>collection handler</returns>
        /// <exception cref="CollectionException">If this type is not a collection type</exception>
        ICollectionHandler GetCollectionHandler();

        /// <summary>
        /// Gets the type of items contained within a collection if this is a collection type
        /// </summary>
        /// <returns>item type of the collection</returns>
        /// <exception cref="CollectionException">If this type is not a collection type</exception>
        Type GetCollectionItemType();


        /// <summary>
        /// Tests to see if this type is a collection type
        /// </summary>
        /// <returns></returns>
        bool IsCollection();

        /// <summary>
        /// Returns true if there is a converter defined for this property
        /// </summary>
        bool HasConverter { get; }

        /// <summary>
        /// Gets or sets a TypeConverter for this type if one is available.  Setting this property
        /// will override any converter declared using attributes
        /// </summary>
        IJsonTypeConverter TypeConverter { get; set; }
    }
}
