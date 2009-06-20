using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    public interface IConfigurationAware 
    {
        IConfiguration Config { get; set; }
    }
}
