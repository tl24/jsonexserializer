using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
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
