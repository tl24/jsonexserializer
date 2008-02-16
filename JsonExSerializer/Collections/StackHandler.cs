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
    /// Collection handler for a non-generic System.Collections.Stack
    /// class.
    /// </summary>
    public class StackHandler : CollectionHandler
    {
        public override bool IsCollection(Type collectionType)
        {
            return typeof(Stack).IsAssignableFrom(collectionType);
        }

        public override ICollectionBuilder ConstructBuilder(Type collectionType, int itemCount)
        {
            return new StackBuilder(collectionType);
        }

        public override ICollectionBuilder ConstructBuilder(object collection)
        {
            return new StackBuilder((Stack) collection);
        }

        public override IEnumerable GetEnumerable(object collection)
        {
            object[] items = new object[((Stack)collection).Count];
            ((Stack)collection).CopyTo(items, 0);
            Array.Reverse(items);
            return items;
        }
    }
}
