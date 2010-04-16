using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;

namespace JsonExSerializerTests.Mocks
{
        /// <summary>
        /// A point class to test classes with constructor args
        /// </summary>
        public class MyPointConstructor
        {
            private int _x;
            private int _y;

            public MyPointConstructor(int x, int y)
            {
                this._x = x;
                this._y = y;
            }
            
            [ConstructorParameter(0)]
            public int X
            {
                get { return _x; }
            }

            [ConstructorParameter(1)]
            public int Y
            {
                get { return _y; }
            }

            public override bool Equals(object obj)
            {
                if (base.Equals(obj))
                {
                    return true;
                }
                else
                {
                    if (obj != null && obj is MyPointConstructor)
                    {
                        MyPointConstructor otherPt = (MyPointConstructor)obj;
                        return otherPt.X == this.X && otherPt.Y == this.Y;
                    }
                }
                return false;
            }

            public override int GetHashCode()
            {
                return (X << 4 | Y);
            }
        }
}
