using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using JsonExSerializer.Collections;
using JsonExSerializer;

namespace JsonExSerializerTests.Mocks
{
    [JsonExCollection(typeof(StronglyTypedCollectionHandler))]
    public class StronglyTypedCollection : CollectionBase
    {

        public StronglyTypedCollection()
        {
        }

        public void Add(string Item)
        {
            this.List.Add(Item);
        }
    }

    public class StronglyTypedCollectionHandler : CollectionHandler
    {
        public override bool IsCollection(Type collectionType)
        {
            return typeof(StronglyTypedCollection).IsAssignableFrom(collectionType);
        }

        public override Type GetItemType(Type CollectionType)
        {
            return typeof(string);
        }

        public override ICollectionBuilder ConstructBuilder(object collection)
        {
            return new ListCollectionBuilder((IList)collection);
        }

        public override ICollectionBuilder ConstructBuilder(Type collectionType, int itemCount)
        {
            return new ListCollectionBuilder(collectionType);
        }
    }
}
