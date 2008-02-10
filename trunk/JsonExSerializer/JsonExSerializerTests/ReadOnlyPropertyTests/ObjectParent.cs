using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;

namespace JsonExSerializerTests.ReadOnlyPropertyTests
{
    public class ObjectParent
    {
        private CollItem _item = new CollItem();

        [JsonProperty]
        public CollItem Item
        {
            get { return _item; }
        }
    }
}
