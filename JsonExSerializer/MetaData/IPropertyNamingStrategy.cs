using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Strategy for naming properties
    /// </summary>
    public interface IPropertyNamingStrategy
    {
        string GetName(string originalName);
    }
}
