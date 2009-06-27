using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace JsonExSerializer.Framework.Parsing
{
    public class ReaderCharacterStream : ICharacterStream, IDisposable
    {
        private const int DEFAULT_BUFFER_SIZE = 1024;
        private const int DEFAULT_READ_SIZE = 1024;

        private char[] buffer;
        private int bufferOffset = 0;
        private int position = 0;
        private int marker = -1;
        private TextReader reader;
        private int nRead = 0;
        private int readSize = DEFAULT_READ_SIZE;
        private int bufferSize = DEFAULT_BUFFER_SIZE;
        private bool streamExhausted = false;
        private int lookBehind = 1;

        public ReaderCharacterStream(string input)
            : this(new StringReader(input))
        {
        }

        public ReaderCharacterStream(string input, int bufferSize)
            : this(new StringReader(input), bufferSize)
        {
        }

        public ReaderCharacterStream(TextReader reader)
            :
            this(reader, DEFAULT_BUFFER_SIZE, DEFAULT_READ_SIZE)
        {
        }

        public ReaderCharacterStream(TextReader reader, int bufferSize)
            : this(reader, bufferSize, bufferSize)
        {
        }

        public ReaderCharacterStream(TextReader reader, int bufferSize, int readSize) {
            this.reader = reader;
            if (bufferSize <= 0)
                bufferSize = DEFAULT_BUFFER_SIZE;
            if (readSize <= 0)
                readSize = DEFAULT_READ_SIZE;
            this.readSize = readSize;
            this.bufferSize = bufferSize;
            buffer = new char[bufferSize];
        }

        /// <summary>
        /// The current position in the stream
        /// </summary>
        public int Position
        {
            get { return this.position; }
        }

        /// <summary>
        /// Controls how many characters are guaranteed to be saved in the buffer
        /// for look behind.  Trying to do a negative LookAhead of a greater amount
        /// than LookBehind is not gauranteed to return valid data.  The Mark position
        /// overrides LookBehind and everything starting with the Marked position is
        /// saved in the buffer until it's released.
        /// </summary>
        public int LookBehind
        {
            get { return this.lookBehind; }
            set { this.lookBehind = value; }
        }

        /// <summary>
        /// Looks ahead (or behind) by <paramref name="offset"/> characters.  An <paramref name="offset"/> of 1
        /// returns the next character waiting to be read.  An <paramref name="offset"/> of -1 returns the character
        /// that was just read.  An <paramref name="offset"/> of 0 is undefined and returns -1.
        /// </summary>
        /// <param name="offset">the amount to look ahead by</param>
        /// <returns>the character value or -1 if the offset is not valid or out of range</returns>
        public int LookAhead(int offset)
        {
            if (offset == 0)
                return 0;
            if (offset < 0)
            {
                offset++;
                if (ComputeBufferIndex(position + offset - 1) < 0)
                {
                    return -1; // invalid; no char before first char
                }
            }
            EnsureData(position + offset - 1);
            if ((position + offset - 1) >= nRead)
            {
                return -1;
            }
            return buffer[ComputeBufferIndex(position + offset - 1)];
        }

        private void EnsureData(int position)
        {
            while (!streamExhausted && position >= nRead)
            {
                if (AmountToRead == 0)
                {
                    // if we must retain everything starting at the beginning of the buffer then we need to resize
                    int minPos = MinPosition;
                    int minPosOffset = ComputeBufferIndex(MinPosition);
                    if (minPosOffset == 0)
                    {
                        Array.Resize<char>(ref buffer, buffer.Length + bufferSize);
                    }
                    else
                    {
                        // copy over characters we don't need anymore
                        int start = minPosOffset;
                        int copyLen = nRead - minPos;
                        if (copyLen > 0)
                            Array.Copy(buffer, start, buffer, 0, copyLen);
                        bufferOffset += start;
                    }
                }
                int len = reader.Read(buffer, ComputeBufferIndex(nRead), AmountToRead);
                if (len == 0)
                    streamExhausted = true;
                nRead += len;
            }
        }

        // Computes the amount of characters that should be read on the next Read call
        private int AmountToRead
        {
            get
            {
                return Math.Min(buffer.Length - ComputeBufferIndex(nRead), readSize);
            }
        }

        /// <summary>
        /// Returns the earliest position in the stream that
        /// must be retained
        /// </summary>
        /// <returns></returns>
        private int MinPosition
        {
            get
            {
                if (marker > -1)
                    return marker;
                else
                    return Math.Max(Position - lookBehind, 0);
            }
        }


        public int Marker
        {
            get { return this.marker; }
        }

        private int ComputeBufferIndex(int position)
        {
            return position - bufferOffset;
        }

        public int Consume()
        {
            return Consume(1);
        }

        public void Seek(int newPosition)
        {
            if (newPosition < 0)
                throw new InvalidOperationException("Attempt to seek before the start of the stream");
            int iterations = 0;
            while (position < (streamExhausted? nRead : newPosition))
            {
                position = Math.Min(nRead + (streamExhausted ? 0 : 1), newPosition);
                EnsureData(position);
                iterations++;
            }
        }

        public int Consume(int amount)
        {
            int result = LookAhead(amount);
            Seek(this.Position + amount);
            return result;
        }

        public int Mark()
        {
            if (this.marker != -1)
                throw new InvalidOperationException("Only one mark can be remembered at a time");
            this.marker = this.Position;
            return this.marker;
        }

        public void Release()
        {
            this.marker = -1;
        }

        public string Substring()
        {
            return Substring(false);
        }

        public string Substring(bool releaseMark)
        {
            if (this.Marker < 0)
                throw new InvalidOperationException("Substring called with no Start,Stop parameters must have Mark set");
            return Substring(this.Marker, this.Position - 1, releaseMark);
        }
        public string Substring(int start, int stop)
        {
            return Substring(start, stop, false);
        }

        public string Substring(int start, int stop, bool releaseMark)
        {
            VerifySubString(start, stop);
            string result = new string(buffer, ComputeBufferIndex(start), stop - start + 1);
            if (releaseMark)
                Release();
            return result;
        }


        private void VerifySubString(int start, int stop)
        {
            EnsureData(stop);
            if (ComputeBufferIndex(start) < 0)
                throw new ArgumentOutOfRangeException("start", start, "Value at start position is no longer contained in the Stream. Either Mark the stream to ensure characters are retained or use a higher LookAhead");
            if (stop >= nRead)
                throw new ArgumentOutOfRangeException("stop", stop, "Stop position is past the end of the stream");
        }

        public StringBuilder Builder(int start, int stop) {
            return Builder(start, stop, false);
        }

        public StringBuilder Builder(int start, int stop, bool releaseMark)
        {
            VerifySubString(start, stop);
            StringBuilder builder = new StringBuilder(stop - start + 1);
            builder.Append(buffer, ComputeBufferIndex(start), stop - start + 1);
            if (releaseMark)
                Release();
            return builder;
        }

        public void Dispose()
        {
            if (reader != null)
                reader.Dispose();
            reader = null;
        }
    }
}
