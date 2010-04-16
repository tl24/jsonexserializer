using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Property naming strategy that returns the name without changing it
    /// </summary>
    public class DefaultPropertyNamingStrategy : IPropertyNamingStrategy
    {

        public DefaultPropertyNamingStrategy()
        {
        }

        public string GetName(string originalName)
        {
            return originalName;
        }
    }
}
