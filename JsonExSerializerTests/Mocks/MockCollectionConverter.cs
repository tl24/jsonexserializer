using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Collections;

namespace JsonExSerializerTests.Mocks
{
    public class MockCollectionHandler : ICollectionHandler
    {
        #region ICollectionHandler Members

        public bool IsCollection(Type collectionType)
        {
            return collectionType == typeof(MockCollection);
        }

        public ICollectionBuilder ConstructBuilder(Type collectionType, int itemCount)
        {
            return new MockCollectionBuilder();
        }

        public Type GetItemType(Type CollectionType)
        {
            return typeof(char);
        }

        public System.Collections.IEnumerable GetEnumerable(object collection)
        {
            return ((MockCollection)collection).Value();
        }

        #endregion

        #region ICollectionHandler Members


        public ICollectionBuilder ConstructBuilder(object collection)
        {
            return new MockCollectionBuilder((MockCollection) collection);
        }

        #endregion
    }

    public class MockCollection
    {
        private StringBuilder builder;
        public MockCollection()
        {
            builder = new StringBuilder();
        }

        public MockCollection(string init)
        {
            builder = new StringBuilder(init);
        }

        public void Add(char c)
        {
            builder.Append(c);
        }

        public string Value()
        {
            return builder.ToString();
        }
    }

    public class MockCollectionBuilder : ICollectionBuilder
    {
        private MockCollection result;

        public MockCollectionBuilder()
        {
            result = new MockCollection();
        }


        public MockCollectionBuilder(MockCollection collection)
        {
            result = collection;
        }

        #region ICollectionBuilder Members

        public void Add(object item)
        {
            result.Add((char)item);
        }

        public object GetResult()
        {
            return result;
        }

        public object GetReference()
        {
            return result;
        }

        #endregion
    }
}
