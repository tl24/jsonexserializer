using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    public interface IContextAware 
    {
        SerializationContext Context { get; set; }
    }
}
