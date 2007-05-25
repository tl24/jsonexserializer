using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializerTests.Mocks
{
    public class SemiComplexObject
    {
        private string _name;
        private SimpleObject _simpleObject;

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


    }
}
