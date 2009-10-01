using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Naming strategy that uses a delegate to generate the name.  This is for simple strategies
    /// that could be done with a lambda or anonymous delegate.
    /// </summary>
    public class DelegateNamingStrategy : IPropertyNamingStrategy
    {
        Converter<string, string> _namingDelegate;

        public DelegateNamingStrategy(Converter<string, string> namingDelegate)
        {
            _namingDelegate = namingDelegate;
        }

        public string GetName(string originalName)
        {
            return _namingDelegate(originalName);
        }
    }
}
