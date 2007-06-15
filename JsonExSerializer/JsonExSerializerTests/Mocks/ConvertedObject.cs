using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializerTests.Mocks
{

    /// <summary>
    /// A point class to test type conversion, values can only
    /// be set through the constructor.
    /// </summary>
    [JsonConvert(typeof(MyImmutablePointConverter))]
    public class MyImmutablePoint
    {
        private int _x;
        private int _y;

        public MyImmutablePoint(int x, int y)
        {
            this._x = x;
            this._y = y;
        }

        public int X
        {
            get { return _x; }
        }

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
                if (obj != null && obj is MyImmutablePoint)
                {
                    MyImmutablePoint otherPt = (MyImmutablePoint)obj;
                    return otherPt.X == this.X && otherPt.Y == this.Y;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// A converter for MyImmutablePoint
    /// </summary>
    public class MyImmutablePointConverter : IJsonTypeConverter
    {
        #region IJsonTypeConverter Members

        public object ConvertFrom(object item)
        {
            MyImmutablePoint pt = (MyImmutablePoint) item;
            return pt.X + "," + pt.Y;
        }

        public object ConvertTo(object item, Type sourceType)
        {
            string data = (string)item;
            int x = 0, y = 0;
            if (data.IndexOf(',') != -1)
            {
                string[] splitData = data.Split(',');
                return new MyImmutablePoint(int.Parse(splitData[0]), int.Parse(splitData[1]));
            }
            else
            {
                return new MyImmutablePoint(0, 0);
            }
        }

        public object Context
        {
            set { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IJsonTypeConverter Members

        public Type GetSerializedType(Type sourceType)
        {
            return typeof(string);
        }

        #endregion
    }
}
