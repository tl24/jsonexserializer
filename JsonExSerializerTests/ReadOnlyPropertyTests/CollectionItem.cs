using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializerTests.ReadOnlyPropertyTests
{
    public class CollItem
    {
        private string _value;

        public CollItem()
        {
        }

        public CollItem(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return this._value; }
            set { this._value = value; }
        }
    }
}
