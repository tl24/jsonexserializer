using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    public interface IConfigurationAware 
    {
        ISerializerSettings Config { get; set; }
    }
}
