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

        [ConstructorParameter]
        public int X
        {
            get { return this.x; }
        }

        [ConstructorParameter]
        public int Y
        {
            get { return this.y; }
        }


    }
}
