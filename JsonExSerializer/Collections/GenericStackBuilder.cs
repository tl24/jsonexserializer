using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Collections
{
    public class GenericStackBuilder<T> : ICollectionBuilder
    {
        private Stack<T> _stack;
        public GenericStackBuilder()
        {
            _stack = new Stack<T>();
        }

        #region ICollectionBuilder Members

        public void Add(object item)
        {
            _stack.Push((T)item);
        }

        public object GetResult()
        {
            return _stack;
        }

        #endregion
    }
}
