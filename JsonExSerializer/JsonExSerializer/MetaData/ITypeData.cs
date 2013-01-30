using System;
using System.Collections.Generic;
using JsonExSerializer.Collections;
using JsonExSerializer.TypeConversion;
using System.Linq.Expressions;
namespace JsonExSerializer.MetaData
{
    public interface ITypeData : IMetaData
    {
        /// <summary>
        /// Get the list of properties for this type
        /// </summary>
        IEnumerable<IPropertyData> AllProperties { get; }

        /// <summary>
        /// Gets the collection handler if this object is a collection
        /// </summary>
        CollectionHandler CollectionHandler { get; set; }

        /// <summary>
        /// Get the list of constructor parameters for this type
        /// </summary>
        IList<IPropertyData> ConstructorParameters { get; }

        /// <summary>
        /// Creates an instance of this Type with the specified arguments
        /// </summary>
        /// <param name="args">the arguments passed to the constructor if any</param>
        /// <returns>the created object</returns>
        object CreateInstance(object[] args);


        DefaultValueCollection DefaultValues { get; set; }

        /// <summary>
        /// Finds a property by its name or alias.
        /// </summary>
        /// <param name="Name">the name of the property</param>
        /// <returns>IPropertyData instance for the property or null if not found</returns>
        IPropertyData FindProperty(string Name);

        /// <summary>
        /// Finds a property by its alias.
        /// </summary>
        /// <param name="alias">the alias of the property to search for</param>
        /// <returns>IPropertyData instance for the property or null if not found</returns>
        IPropertyData FindPropertyByAlias(string alias);

        /// <summary>
        /// Finds a property by its name.
        /// </summary>
        /// <param name="Name">the name of the property</param>
        /// <returns>IPropertyData instance for the property or null if not found</returns>
        IPropertyData FindPropertyByName(string Name);

        object GetDefaultValue(Type forType);

        /// <summary>
        /// Ignore a property to keep from being serialized, same as if the JsonExIgnore attribute had been set
        /// </summary>
        /// <param name="name">the name of the property</param>
        void IgnoreProperty(string name);

        bool IsCollection();

        Type ForType { get; }
        /// <summary>
        /// Returns all the properties on the type that are not ignored
        /// </summary>
        IEnumerable<IPropertyData> Properties { get; }

        IJsonTypeConverter TypeConverter { get; set; }

        /// <summary>
        /// Returns true if there is a TypeConverter defined for this object
        /// </summary>
        bool HasConverter { get; }
    }

    public interface ITypeData<T> : ITypeData
    {
        IPropertyData Property<P>(Expression<Func<T, P>> propertyExpression);
        void IgnoreProperty<P>(Expression<Func<T, P>> propertyExpression);
    }
}
