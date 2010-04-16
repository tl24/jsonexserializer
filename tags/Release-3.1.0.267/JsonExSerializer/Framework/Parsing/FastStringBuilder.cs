using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Framework.Parsing
{
    public class FastStringBuilder
    {
        char[] buffer;
        int length = 0;
        public FastStringBuilder() : this(64)
        {
        }

        public FastStringBuilder(int capacity)
        {
            buffer = new char[capacity];
        }

        public FastStringBuilder(string source)
        {
            int capacity = 16;
            while (capacity < source.Length)
                capacity += 16;
            buffer = new char[capacity];
            source.CopyTo(0, buffer, 0, source.Length);
            length = source.Length;
        }

        public int Capacity
        {
            get { return buffer.Length; }
        }

        public int Length
        {
            get { return this.length; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Length cannot be less than zero");
                if (value > buffer.Length)
                    EnsureCapacity(value);
                this.length = value;
            }
        }

        private void EnsureCapacity(int value)
        {
            if (value <= buffer.Length)
                return;
            int newCapacity = buffer.Length * 2;
            while (newCapacity < value)
                newCapacity += buffer.Length;
            Array.Resize(ref buffer, value);
        }

        public FastStringBuilder Append(char c)
        {
            if (length + 1> buffer.Length)
                EnsureCapacity(Length+1);
            buffer[length++] = c;
            return this;
        }

        public FastStringBuilder Append(string s)
        {
            if (length + s.Length > buffer.Length) 
                EnsureCapacity(Length + s.Length);
            s.CopyTo(0, buffer, Length, s.Length);
            this.length += s.Length;
            return this;
        }

        public override string ToString()
        {
            return ToString(0, Length);
        }

        public string ToString(int start, int length)
        {
            return new string(buffer, start, length);
        }
    }
}
