/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
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

        public ICollectionBuilder ConstructBuilder(object collection)
        {
            throw new InvalidOperationException("ArrayHandler does not support modify existing collections");
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
