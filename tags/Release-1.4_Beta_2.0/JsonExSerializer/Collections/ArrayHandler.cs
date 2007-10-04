using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JsonExSerializer.Collections
{

    /// <summary>
    /// Collection handler class for arrays
    /// </summary>
    public class ArrayHandler : ICollectionHandler
    {
        #region ICollectionHandler Members

        public bool IsCollection(Type collectionType)
        {
            return collectionType.IsArray;
        }

        public ICollectionBuilder ConstructBuilder(Type collectionType, int itemCount)
        {
            return new ArrayBuilder(collectionType, itemCount);
        }

        public Type GetItemType(Type CollectionType)
        {
            return CollectionType.GetElementType();
        }

        #endregion

        #region ICollectionHandler Members


        public IEnumerable GetEnumerable(object collection)
        {
            return (IEnumerable) collection;
        }

        #endregion
    }
}
