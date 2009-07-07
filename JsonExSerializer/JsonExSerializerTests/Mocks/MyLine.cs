using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;
using JsonExSerializer;

namespace JsonExSerializerTests.Mocks
{
    /// <summary>
    /// test class for property converters
    /// </summary>
    public class MyLine
    {
        private MyImmutablePoint _start;
        private MyImmutablePoint _end;

        public MyLine()
        {
        }

        [JsonConvert(typeof(MyLinePointConverter), /* separator */ Context=":")]
        public MyImmutablePoint Start
        {
            get { return this._start; }
            set { this._start = value; }
        }

        public MyImmutablePoint End
        {
            get { return this._end; }
            set { this._end = value; }
        }
    }

    public class MyLinePointConverter : JsonConverterBase, IJsonTypeConverter
    {
        private static int _convertFromCount;
        private static int _convertToCount;
        private string _separator;

        public override Type GetSerializedType(Type sourceType)
        {
            return typeof(string);
        }

        public override object ConvertFrom(object item, SerializationContext serializationContext)
        {
            _convertFromCount++;
            // seperate with ":" and surround with () this time
            MyImmutablePoint pt = (MyImmutablePoint)item;
            return "(" + pt.X + _separator + pt.Y + ")";
        }

        public override object ConvertTo(object item, Type sourceType, SerializationContext serializationContext)
        {
            _convertToCount++;
            string data = (string)item;
            if (data.IndexOf(_separator) != -1)
            {
                string[] splitData = data.Replace("(", "").Replace(")", "").Split(_separator.ToCharArray());
                return new MyImmutablePoint(int.Parse(splitData[0]), int.Parse(splitData[1]));
            }
            else
            {
                return new MyImmutablePoint(0, 0);
            }
        }

        public override object Context
        {
            set { _separator = value != null ? value.ToString() : ":"; }
        }

        public static int ConvertFromCount
        {
            get { return _convertFromCount; }
        }

        public static int ConvertToCount
        {
            get { return _convertToCount; }
        }

        public static void clear()
        {
            _convertToCount = 0;
            _convertFromCount = 0;
        }
    }

    public class MyImmutableLine
    {
        private MyImmutablePoint _start;
        private MyImmutablePoint _end;

        public MyImmutableLine(MyImmutablePoint start, MyImmutablePoint end)
        {
            _start = start;
            _end = end;
        }

        [JsonConvert(typeof(MyLinePointConverter), /* separator */ Context=":")]
        [ConstructorParameter]
        public MyImmutablePoint Start
        {
            get { return this._start; }
        }

        [JsonConvert(typeof(MyLinePointConverter), /* separator */ Context=":")]
        [ConstructorParameter]
        public MyImmutablePoint End
        {
            get { return this._end; }
        }
    }
}
