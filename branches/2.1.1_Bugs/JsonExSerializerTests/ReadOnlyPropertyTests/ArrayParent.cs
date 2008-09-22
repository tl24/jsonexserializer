using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;

namespace JsonExSerializerTests.ReadOnlyPropertyTests
{
    public class ArrayParent
    {
        private string[] _items = { "one", "two" };

        [JsonExProperty]
        public string[] Items
        {
            get { return _items; }
        }
    }
}
