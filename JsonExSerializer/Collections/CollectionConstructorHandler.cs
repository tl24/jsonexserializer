using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JsonExSerializer.Collections
{
    /// <summary>
    /// Handles collection classes implementing ICollection
    /// with an constructor matching (ICollection) or (IEnumerable&lt;&gt;)
    /// or (IEnumerable).
    /// </summary>
    public class CollectionConstructorHandler : ICollectionHandler
    {
        #region ICollectionHandler Members

        public bool IsCollection(Type collectionType)
        {
            // Implements ICollection and has a constructor that takes a single element of type ICollection
            Type ienumGeneric = collectionType.GetInterface(typeof(IEnumerable<>).Name);
            if ((typeof(ICollection).IsAssignableFrom(collectionType)
                 || ienumGeneric != null)
                && (collectionType.GetConstructor(new Type[] { typeof(ICollection) }) != null)
                    || (ienumGeneric != null && collectionType.GetConstructor(new Type[] { ienumGeneric }) != null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICollectionBuilder ConstructBuilder(Type collectionType, int itemCount)
        {
            Type itemType = GetItemType(collectionType);
            // will make a generic builder either way, but itemType might be object
            return (ICollectionBuilder)Activator.CreateInstance(typeof(GenericCollectionCtorBuilder<>).MakeGenericType(itemType), collectionType);
        }

        public Type GetItemType(Type CollectionType)
        {
            Type t = null;
            if ((t = CollectionType.GetInterface(typeof(ICollection<>).Name)) != null)
            {
                return t.GetGenericArguments()[0];
            }
            else if ((t = CollectionType.GetInterface(typeof(IEnumerable<>).Name)) != null)
            {
                return t.GetGenericArguments()[0];
            }
            else
            {
                return typeof(object);
            }
        }

        public IEnumerable GetEnumerable(object collection)
        {
            return (IEnumerable) collection;
        }

        #endregion
    }
}
