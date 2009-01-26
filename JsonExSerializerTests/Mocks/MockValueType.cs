using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;

namespace JsonExSerializerTests.Mocks
{
    public struct MockValueType
    {
        private int x;
        private int y;

        public MockValueType(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        [ConstructorParameter(0)]
        public int X
        {
            get { return this.x; }
        }

        [ConstructorParameter(1)]
        public int Y
        {
            get { return this.y; }
        }


    }
}
