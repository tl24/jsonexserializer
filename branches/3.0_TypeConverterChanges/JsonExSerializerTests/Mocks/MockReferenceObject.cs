using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializerTests.Mocks
{
    public class MockReferenceObject
    {
        private MockReferenceObject _reference;
        private string _name;

        public MockReferenceObject Reference
        {
            get { return this._reference; }
            set { this._reference = value; }
        }

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj is MockReferenceObject)
            {
                return _name == ((MockReferenceObject)obj).Name;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }
    }

    public class MockSubReferenceObject : MockReferenceObject
    {
    }
}
