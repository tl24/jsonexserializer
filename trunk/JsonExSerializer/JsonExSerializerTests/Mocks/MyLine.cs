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

        [MyLinePointConverter(":")]
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

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this.GetType().Equals(obj.GetType()))
                return Equals((MyLine)obj);
            else
                return base.Equals(obj);
        }

        public bool Equals(MyLine line)
        {
            return this.Start.Equals(line.Start) && this.End.Equals(line.End);
        }
    }

    public class MyLinePointConverterAttribute : JsonConvertAttribute
    {
        public MyLinePointConverterAttribute()
            : base(typeof(MyLinePointConverter))
        {
        }

        public MyLinePointConverterAttribute(string separator)
            : this()
        {
            Separator = separator;
        }

        /// <summary>
        /// Gets or sets the separator to use, defaults to comma ','.
        /// </summary>
        /// <value>
        /// The separator.
        /// </value>
        public string Separator { get; set; }

        public override IJsonTypeConverter CreateTypeConverter()
        {
            return new MyLinePointConverter(Separator);
        }
    }

    public class MyLinePointConverter : JsonConverterBase, IJsonTypeConverter
    {
        private static int _convertFromCount;
        private static int _convertToCount;
        private string _separator;

        public MyLinePointConverter(string separator)
        {
            _separator = separator;
        }
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

        [MyLinePointConverter(":")]
        [ConstructorParameter]
        public MyImmutablePoint Start
        {
            get { return this._start; }
        }

        [MyLinePointConverter(":")]
        [ConstructorParameter]
        public MyImmutablePoint End
        {
            get { return this._end; }
        }
    }
}
