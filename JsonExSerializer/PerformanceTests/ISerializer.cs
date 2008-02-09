using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceTests
{
    public interface ISerializer
    {
        string FileName { get; }
        string Name { get; }
        void Serialize(object o);
        object Deserialize(Type t);
    }
}
