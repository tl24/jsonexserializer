using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JsonExSerializer.Collections
{    
    /// <summary>
    /// Collection builder for types implementing IList
    /// </summary>
    public class ListCollectionBuilder : ICollectionBuilder
    {
        private IList _list;
        private Type _instanceType;
        public ListCollectionBuilder(Type instanceType)
        {
            _instanceType = instanceType;
            _list = (IList)Activator.CreateInstance(instanceType);
        }

        #region ICollectionBuilder Members

        public void Add(object item)
        {
            _list.Add(item);
        }

        public virtual object GetResult()
        {
            return _list;
        }

        #endregion
    }
}
