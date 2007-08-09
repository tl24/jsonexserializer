using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JsonExSerializer.Collections
{
    /// <summary>
    /// Handler class for classes implementing IList
    /// </summary>
    public class ListHandler : ICollectionHandler
    {
        private Type _IListType = typeof(IList);
        private Type _ICollectionGenericType = typeof(ICollection<>);
        private Type _IDictionaryType = typeof(IDictionary);
        #region ICollectionHandler Members

        public bool IsCollection(Type collectionType)
        {
            return (_IListType.IsAssignableFrom(collectionType)
                && !_ICollectionGenericType.IsAssignableFrom(collectionType)
                && !_IDictionaryType.IsAssignableFrom(collectionType));
        }

        public ICollectionBuilder ConstructBuilder(Type collectionType)
        {
            return new ListCollectionBuilder(collectionType);
        }

        public Type GetItemType(Type CollectionType)
        {
            return typeof(object);
        }

        public IEnumerable GetEnumerable(object collection)
        {
            return (IEnumerable)collection;
        }

        #endregion
    }
}
