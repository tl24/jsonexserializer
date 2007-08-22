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
        private Array result;
        private int index = 0;
        public ArrayBuilder(Type arrayType, int itemCount)
        {
            _arrayType = arrayType;
            if (_arrayType.IsArray)
            {
                result = Array.CreateInstance(_arrayType.GetElementType(), itemCount);
            }
            else
            {
                throw new ArgumentException("arrayType parameter must be of type Array");
            }
        }
        #region ICollectionBuilder Members

        public void Add(object item)
        {
            result.SetValue(item, index++);
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
