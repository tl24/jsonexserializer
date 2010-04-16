using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;

namespace JsonExSerializerTests.Mocks
{
    public class SpecializedMock
    {
        private string _name;
        private int _count;
        private string _ignoredProp;

        public SpecializedMock(int count)
        {
            this._count = count;
        }

        public SpecializedMock() : this(0) { }

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public int Count
        {
            get { return this._count; }
        }

        [JsonExIgnore]
        public string IgnoredProp
        {
            get { return this._ignoredProp; }
            set { this._ignoredProp = value; }
        }


    }
}
