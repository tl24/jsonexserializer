using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JsonExSerializer.Collections
{

    /// <summary>
    /// Collection handler for a non-generic System.Collections.Stack
    /// class.
    /// </summary>
    public class StackHandler : ICollectionHandler
    {
        #region ICollectionHandler Members

        public bool IsCollection(Type collectionType)
        {
            return typeof(Stack).IsAssignableFrom(collectionType);
        }

        public ICollectionBuilder ConstructBuilder(Type collectionType)
        {
            return new StackBuilder(collectionType);
        }

        public Type GetItemType(Type CollectionType)
        {
            return typeof(object);
        }

        public System.Collections.IEnumerable GetEnumerable(object collection)
        {
            object[] items = new object[((Stack)collection).Count];
            ((Stack)collection).CopyTo(items, 0);
            Array.Reverse(items);
            return items;
        }

        #endregion
    }
}
