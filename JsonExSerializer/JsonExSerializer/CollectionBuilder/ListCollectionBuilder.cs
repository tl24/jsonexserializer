using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JsonExSerializer.CollectionBuilder
{    
    /// <summary>
    /// Collection builder for types implementing IList and arrays
    /// </summary>
    public class ListCollectionBuilder : ICollectionBuilder
    {
        private IList _list;
        private Type _instanceType;
        public ListCollectionBuilder(Type instanceType)
        {
            _instanceType = instanceType;
            if (instanceType.IsArray)
            {
                // construct a strongly-typed List with the array element type
                Type tempType = typeof(List<object>).GetGenericTypeDefinition();
                tempType = tempType.MakeGenericType(new Type[] { instanceType.GetElementType() });
                _list = (IList)Activator.CreateInstance(tempType);
            }
            else
            {
                _list = (IList)Activator.CreateInstance(instanceType);
            }
        }

        #region ICollectionBuilder Members

        public void Add(object item)
        {
            _list.Add(item);
        }

        public object GetResult()
        {
            if (_instanceType.IsArray)
            {
                Array result = Array.CreateInstance(_instanceType.GetElementType(), _list.Count);
                if (_list.Count > 0)
                {
                    _list.CopyTo(result, 0);
                }
                return result;
            }
            else
            {
                return _list;
            }
        }

        #endregion
    }
}
