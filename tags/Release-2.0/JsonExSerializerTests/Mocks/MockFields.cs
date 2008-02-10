using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializerTests.Mocks
{
    public class MockFields
    {
        public int IntValue;
        protected bool IsProtected;

        public bool GetProtected()
        {
            return IsProtected;
        }

        public void SetProtected(bool value)
        {
            IsProtected = value;
        }

        public SimpleObject SimpleObj;
    }
}
