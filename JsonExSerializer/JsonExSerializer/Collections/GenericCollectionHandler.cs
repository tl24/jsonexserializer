using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;

namespace JsonExSerializer.Collections
{

    /// <summary>
    /// Handler class for Generic ICollection interface
    /// </summary>
    public class GenericCollectionHandler : ICollectionHandler
    {

        private Type _IDictionaryType = typeof(IDictionary);
        private string _IGenericCollectionName = typeof(ICollection<>).Name;

        #region ICollectionHandler Members

        public bool IsCollection(Type collectionType)
        {
            return (!collectionType.IsArray 
                    && collectionType.GetInterface(_IGenericCollectionName) != null
                    && !_IDictionaryType.IsAssignableFrom(collectionType));
        }

        public ICollectionBuilder ConstructBuilder(Type collectionType)
        {
            Type itemType = GetItemType(collectionType);
            return (ICollectionBuilder) Activator.CreateInstance(typeof(GenericCollectionBuilder<>).MakeGenericType(itemType), collectionType);
        }

        public Type GetItemType(Type CollectionType)
        {
            Type intfType = CollectionType.GetInterface(_IGenericCollectionName);
            return intfType.GetGenericArguments()[0];            
        }

        public IEnumerable GetEnumerable(object collection)
        {
            return (IEnumerable)collection;
        }

        #endregion
    }
}
