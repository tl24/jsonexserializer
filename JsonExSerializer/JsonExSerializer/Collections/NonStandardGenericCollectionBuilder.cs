using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Collections
{
    /// <summary>
    /// A collection builder for non-standard Generic collections which support a constuctor that takes
    /// an IEnumerable&lt;T&gt; parameter.  Examples are Queue&lt;T&gt, Stack&lt;T&gt, etc.
    /// </summary>
    public class NonStandardGenericCollectionBuilder<T> : ICollectionBuilder
    {
        private List<T> _collector;
        private Type _instanceType;
        public NonStandardGenericCollectionBuilder(Type instanceType)
        {
            _instanceType = instanceType;
            _collector = new List<T>();
        }

        #region ICollectionBuilder Members

        public void Add(object item)
        {
            _collector.Add((T) item);
        }

        public object GetResult()
        {
            //Create the collection passing an instance of (ICollection) as the parameter
            return Activator.CreateInstance(_instanceType, _collector);
        }

        #endregion
    }
}
