using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    class LinkedListCollectionBuilder<T> : ICollectionBuilder
    {

        private ICollection<T> _list;
        private Type _instanceType;

        public LinkedListCollectionBuilder(Type instanceType)
        {
            this._instanceType = instanceType;
            _list = (ICollection<T>)Activator.CreateInstance(_instanceType);
        }

        #region ICollectionBuilder Members

        public void Add(object item)
        {
            _list.Add((T) item);
        }

        public object GetResult()
        {
            return _list;
        }

        #endregion
    }
}
