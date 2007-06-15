using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JsonExSerializer.Collections
{

    /// <summary>
    /// Collection builder class for arrays
    /// </summary>
    public class ArrayBuilder : ICollectionBuilder
    {
        private Type _arrayType;
        private IList _collector;
        public ArrayBuilder(Type arrayType)
        {
            _arrayType = arrayType;
            if (_arrayType.IsArray)
            {
                // construct a strongly-typed List with the array element type
                Type tempType = typeof(List<>).MakeGenericType(new Type[] { _arrayType.GetElementType() });
                _collector = (IList)Activator.CreateInstance(tempType);
            }
            else
            {
                throw new ArgumentException("arrayType parameter must be of type Array");
            }
        }
        #region ICollectionBuilder Members

        public void Add(object item)
        {
            _collector.Add(item);
        }

        public object GetResult()
        {
            Array result = Array.CreateInstance(_arrayType.GetElementType(), _collector.Count);
            if (_collector.Count > 0)
            {
                _collector.CopyTo(result, 0);
            }
            return result;            
        }

        #endregion
    }
}
