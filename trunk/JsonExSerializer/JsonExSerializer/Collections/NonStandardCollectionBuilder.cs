using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JsonExSerializer.Collections
{
    /// <summary>
    /// A collection builder for non-standard collections which support a constuctor that takes
    /// an ICollection parameter.  Examples are Queue, Stack, etc.
    /// </summary>
    public class NonStandardCollectionBuilder : ICollectionBuilder
    {
        private ArrayList _collector;
        private Type _instanceType;
        public NonStandardCollectionBuilder(Type instanceType)
        {
            _instanceType = instanceType;
            //_list = (IList)Activator.CreateInstance(instanceType);
            _collector = new ArrayList();
        }

        #region ICollectionBuilder Members

        public void Add(object item)
        {
            _collector.Add(item);
        }

        public object GetResult()
        {
            //Create the collection passing an instance of (ICollection) as the parameter
            return Activator.CreateInstance(_instanceType, _collector);
        }

        #endregion
    }
}
