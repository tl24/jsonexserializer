using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JsonExSerializer.Collections
{
    /// <summary>
    /// Interface for an item that can build a collection object
    /// </summary>
    public interface ICollectionBuilder
    {
        void Add(object item);
        object GetResult();
    }
}
