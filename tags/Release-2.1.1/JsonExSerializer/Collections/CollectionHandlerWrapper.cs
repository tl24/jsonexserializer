using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Collections
{
    /// <summary>
    /// A wrapper class that allows you to wrap another collection handler and specify
    /// the Collection class and Item class
    /// </summary>
    public class CollectionHandlerWrapper : CollectionHandler
    {
        private CollectionHandler _innerHandler;
        private Type _collectionType;
        private Type _itemType;

        public CollectionHandlerWrapper(CollectionHandler Handler)
        {
            _innerHandler = Handler;
        }

        public CollectionHandlerWrapper(CollectionHandler Handler, Type CollectionType, Type ItemType)
        {
            if (Handler == null)
                throw new ArgumentNullException("Handler cannot be null for CollectionHandlerWrapper");

            _innerHandler = Handler;
            _collectionType = CollectionType;
            _itemType = ItemType;
        }

        public override bool IsCollection(Type collectionType)
        {
            if (_collectionType == null)
                return _innerHandler.IsCollection(collectionType);
            else
                return _collectionType.IsAssignableFrom(collectionType);
        }

        public override ICollectionBuilder ConstructBuilder(Type collectionType, int itemCount)
        {
            return _innerHandler.ConstructBuilder(collectionType, itemCount);
        }

        public override ICollectionBuilder ConstructBuilder(object collection)
        {
            return _innerHandler.ConstructBuilder(collection);
        }

        public override Type GetItemType(Type CollectionType)
        {
            if (_itemType != null)
                return _itemType;
            else
                return _innerHandler.GetItemType(CollectionType);
        }

        public override System.Collections.IEnumerable GetEnumerable(object collection)
        {
            return _innerHandler.GetEnumerable(collection);
        }
    }
}
