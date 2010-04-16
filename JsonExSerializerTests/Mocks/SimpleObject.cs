using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializerTests.Mocks
{
    public enum SimpleEnum
    {
        EnumValue1,
        EnumValue2,
        EnumValue3,
    }

    /// <summary>
    /// Contains simple primitive properties for testing
    /// </summary>
    public class SimpleObject
    {
        private byte _byteValue;
        private short _shortValue;
        private int _intValue;
        private long _longValue;
        private float _floatValue;
        private double _doubleValue;
        private bool _boolValue;
        private string _stringValue;
        private char _charValue;
        private SimpleEnum _enumValue;
        public byte ByteValue
        {
            get { return this._byteValue; }
            set { this._byteValue = value; }
        }

        public short ShortValue
        {
            get { return this._shortValue; }
            set { this._shortValue = value; }
        }

        public int IntValue
        {
            get { return this._intValue; }
            set { this._intValue = value; }
        }

        public long LongValue
        {
            get { return this._longValue; }
            set { this._longValue = value; }
        }

        public float FloatValue
        {
            get { return this._floatValue; }
            set { this._floatValue = value; }
        }

        public double DoubleValue
        {
            get { return this._doubleValue; }
            set { this._doubleValue = value; }
        }

        public bool BoolValue
        {
            get { return this._boolValue; }
            set { this._boolValue = value; }
        }

        public string StringValue
        {
            get { return this._stringValue; }
            set { this._stringValue = value; }
        }

        public char CharValue
        {
            get { return this._charValue; }
            set { this._charValue = value; }
        }


        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                return true;
            }
            else if (obj != null && obj.GetType() == typeof(SimpleObject))
            {
                SimpleObject other = (SimpleObject)obj;
                return other._boolValue == this._boolValue
                    && other._byteValue == this._byteValue
                    && other._charValue == this._charValue
                    && other._doubleValue == this._doubleValue
                    && other._floatValue == this._floatValue
                    && other._intValue == this._intValue
                    && other._longValue == this._longValue
                    && other._shortValue == this._shortValue
                    && other._stringValue == this._stringValue;

            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_boolValue);
            sb.Append(_byteValue);
            sb.Append(_charValue);
            sb.Append(_doubleValue);
            sb.Append(_floatValue);
            sb.Append(_intValue);
            sb.Append(_longValue);
            sb.Append(_shortValue);
            sb.Append(_stringValue);
            return sb.ToString().GetHashCode();
        }

        public SimpleEnum EnumValue
        {
            get { return this._enumValue; }
            set { this._enumValue = value; }
        }
    }
}
