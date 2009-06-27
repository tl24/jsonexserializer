using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Framework.Parsing
{

    public interface ICharacterStream
    {
        int LookAhead(int index);

        /// <summary>
        /// Moves the position in the stream
        /// </summary>
        /// <param name="newPosition">the new position</param>
        void Seek(int newPosition);

        /// <summary>
        /// Consume 1 character and return the next character waiting on the stream
        /// </summary>
        /// <returns>the next character</returns>
        int Consume();

        /// <summary>
        /// Consumes <paramref name="amount"/> characters from the stream and returns 
        /// the next character waiting on the stream.
        /// </summary>
        /// <param name="amount">the amount to consume</param>
        /// <returns>the next character</returns>
        int Consume(int amount);

        /// <summary>
        /// Marks the current position of the stream and ensures that all characters
        /// from the marked position forware are retained in the stream until the mark is released.
        /// </summary>
        /// <returns>the position of the mark</returns>
        int Mark();

        /// <summary>
        /// Returns the current position of the marker or -1 if the stream has not been marked
        /// </summary>
        int Marker { get; }

        /// <summary>
        /// Releases the mark
        /// </summary>
        void Release();

        /// <summary>
        /// Returns the current position in the stream
        /// </summary>
        int Position { get; }

        /// <summary>
        /// Returns the substring from the marked position to the current position - 1 without releasing the Mark.  If the stream
        /// has not been marked an exception is thrown.
        /// </summary>
        /// <returns>string</returns>
        /// <seealso cref="Mark()"/>
        /// <exception cref="InvalidOperationException">The stream has not been marked with a call to Mark()</exception>
        string Substring();

        /// <summary>
        /// Returns the substring from the marked position to the current position - 1 releasing the mark if specified.  If the stream
        /// has not been marked an exception is thrown.
        /// </summary>
        /// <param name="releaseMark">if true the mark is released, otherwise the mark is kept</param>
        /// <returns>string</returns>
        /// <seealso cref="Mark()"/>
        /// <exception cref="InvalidOperationException">The stream has not been marked with a call to Mark()</exception>
        string Substring(bool releaseMark);
        string Substring(int start, int stop);
        string Substring(int start, int stop, bool releaseMark);
        StringBuilder Builder(int start, int stop);
        StringBuilder Builder(int start, int stop, bool releaseMark);

        int LookBehind { get; set; }
    }
}
