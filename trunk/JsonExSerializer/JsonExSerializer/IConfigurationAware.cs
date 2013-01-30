using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    public interface IConfigurationAware 
    {
        ISerializerSettings Settings { get; set; }
    }
}
