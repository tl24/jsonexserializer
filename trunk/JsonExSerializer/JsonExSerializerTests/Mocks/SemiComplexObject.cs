using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializerTests.Mocks
{
    public class SemiComplexObject
    {
        private string _name;
        private SimpleObject _simpleObject;
        private int _count;

        public SemiComplexObject(int count) {
            this._count = count;
        }

        public SemiComplexObject() : this(0) { }


        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public SimpleObject SimpleObject
        {
            get { return this._simpleObject; }
            set { this._simpleObject = value; }
        }

        public int Count
        {
            get { return _count; }
        }
    }
}
