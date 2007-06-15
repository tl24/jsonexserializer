using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JsonExSerializer.Collections
{

    /// <summary>
    /// Builder for a non-generic System.Collections.Stack
    /// class.
    /// </summary>
    public class StackBuilder : ICollectionBuilder
    {
        private Stack _stack;

        public StackBuilder(Type stackType)
        {
            _stack = (Stack)Activator.CreateInstance(stackType);
        }
        #region ICollectionBuilder Members

        public void Add(object item)
        {
            _stack.Push(item);
        }

        public object GetResult()
        {
            return _stack;
        }

        #endregion
    }
}
