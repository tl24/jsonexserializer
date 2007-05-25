using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializerTests.Mocks
{
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

    }
}
