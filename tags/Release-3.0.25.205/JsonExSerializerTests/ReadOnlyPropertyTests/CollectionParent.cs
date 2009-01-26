using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;

namespace JsonExSerializerTests.ReadOnlyPropertyTests
{
    public class CollParent
    {
        private List<CollItem> _items = new List<CollItem>();

        [JsonExProperty]
        public IList<CollItem> Items
        {
            get { return _items; }
        }
    }
}
