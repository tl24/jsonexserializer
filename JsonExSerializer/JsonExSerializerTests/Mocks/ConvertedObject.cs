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
        private int x;
        private int y;

        public MyImmutablePoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X
        {
            get { return x; }
        }

        public int Y
        {
            get { return y; }
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

        public override int GetHashCode()
        {
            return (X << 4 | Y);
        }
    }

    /// <summary>
    /// A converter for MyImmutablePoint
    /// </summary>
    public class MyImmutablePointConverter : JsonConverterBase, IJsonTypeConverter
    {
        public override object ConvertFrom(object item, ISerializerSettings serializationContext)
        {
            MyImmutablePoint pt = (MyImmutablePoint) item;
            return pt.X + "," + pt.Y;
        }

        public override object ConvertTo(object item, Type sourceType, ISerializerSettings serializationContext)
        {
            string data = (string)item;
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

        public override Type GetSerializedType(Type sourceType)
        {
            return typeof(string);
        }

    }


}
