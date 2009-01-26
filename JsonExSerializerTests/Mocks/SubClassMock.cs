using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializerTests.Mocks
{
    public class SubClassMock : SpecializedMock
    {
        private string _subClassProp;
        
        public string SubClassProp
        {
            get { return this._subClassProp; }
            set { this._subClassProp = value; }
        }


    }
}
