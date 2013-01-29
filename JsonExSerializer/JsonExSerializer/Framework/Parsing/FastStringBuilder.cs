using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Framework.Parsing
{
    public class FastStringBuilder
    {
        private const int DefaultCapacity = 128;
        char[] buffer;
        int length = 0;

        public FastStringBuilder()
            : this(DefaultCapacity)
        {
        }

        public FastStringBuilder(int capacity)
        {
            if (capacity < DefaultCapacity)
                capacity = DefaultCapacity;
            EnsureCapacity(capacity);
        }

        public FastStringBuilder(string source)
        {
            int capacity = DefaultCapacity;
            while (capacity < source.Length)
                capacity += 16;
            EnsureCapacity(capacity);
            source.CopyTo(0, buffer, 0, source.Length);
            length = source.Length;
        }

        /// <summary>
        /// The total amount the current buffer can hold
        /// </summary>
        public int Capacity { get; private set; }

        public int Length
        {
            get { return this.length; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Length cannot be less than zero");
                if (value > Capacity)
                    EnsureCapacity(value);
                this.length = value;
            }
        }

        private void EnsureCapacity(int value)
        {
            if (value <= Capacity)
                return;
            int newCapacity = Capacity == 0 ? value : Capacity * 2;
            while (newCapacity < value)
                newCapacity += Capacity;
            Array.Resize(ref buffer, newCapacity);
            Capacity = newCapacity;
        }

        public FastStringBuilder Append(char c)
        {
            if (length + 1 > Capacity)
                EnsureCapacity(Length+1);
            buffer[length++] = c;
            return this;
        }

        public FastStringBuilder Append(string s)
        {
            if (length + s.Length > Capacity) 
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
