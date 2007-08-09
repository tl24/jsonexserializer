using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    [AttributeUsage(AttributeTargets.Property,Inherited=false)]
    public sealed class JsonExIgnoreAttribute : System.Attribute
    {
    }
}
