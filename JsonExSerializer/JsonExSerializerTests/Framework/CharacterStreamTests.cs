using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer.Framework.Parsing;

namespace JsonExSerializerTests.Framework
{
    [TestFixture]
    public class CharacterStreamTests
    {
        [RowTest]
        [Row(-1, -1)]
        [Row(0, 0)]
        [Row(1,'f')]
        [Row(2, 'o')]
        [Row(3, 'r')]
        [Row(4, -1)]
        public void LookAheadTests(int lookAhead, int value)
        {
            ReaderCharacterStream stm = new ReaderCharacterStream("for");
            Assert.AreEqual(value, stm.LookAhead(lookAhead));
        }

        [Test]
        public void IndexStartsAtZero()
        {
            ReaderCharacterStream stm = new ReaderCharacterStream("");
            Assert.AreEqual(0, stm.Position);
        }

        [Test]
        public void ConsumeMovesAhead()
        {
            ReaderCharacterStream stm = new ReaderCharacterStream("for");
            int result = stm.Consume();
            Assert.AreEqual('f', result, "Consume return");
            Assert.AreEqual('o', stm.LookAhead(1), "Look Ahead");
            Assert.AreEqual(1, stm.Position, "Index");
        }

        [Test]
        public void ConsumeMovesToEndAfterStreamExhausted()
        {
            ReaderCharacterStream stm = new ReaderCharacterStream("f");
            stm.LookAhead(2);
            int result = stm.Consume();
            Assert.AreEqual('f', result, "Consume return");
            Assert.AreEqual(-1, stm.LookAhead(1), "Look Ahead");
            Assert.AreEqual(1, stm.Position, "Index");
        }

        [RowTest]
        [Row(1,-1, 'f')]
        [Row(1, -2, -1)]
        [Row(2, -2, 'f')]
        [Row(3, -1, 'r')]
        public void LookBehindTests(int numConsumes, int lookBehind, int value)
        {
            ReaderCharacterStream stm = new ReaderCharacterStream("for");
            while (numConsumes-- != 0)
                stm.Consume();
            Assert.AreEqual(value, stm.LookAhead(lookBehind));
        }

        [Test]
        public void SubstringBuildsString()
        {
            ReaderCharacterStream stm = new ReaderCharacterStream("test string");
            Assert.AreEqual("test", stm.Substring(0, 3));
        }

        [Test]
        public void BufferExpandsAsNeeded()
        {
            ReaderCharacterStream stm = new ReaderCharacterStream("test stream", 4);
            stm.Seek(6);
            Assert.AreEqual('t', stm.LookAhead(1));
        }

        [Test]
        public void MarkEnsuresDataIsKept()
        {
            ReaderCharacterStream stm = new ReaderCharacterStream("test stream", 4);
            int mark = stm.Mark();
            Assert.AreEqual(stm.Position, mark, "Current Position should be returned");
            stm.Seek(6);
            Assert.AreNotEqual(stm.Position, mark);
            Assert.AreEqual("tream", stm.Substring(6, 10));
            stm.Release();
        }

        [Test]
        public void Resize_AfterCopy_ComputesCorrectOffset()
        {
            ReaderCharacterStream stm = new ReaderCharacterStream("test stream", 4);
            stm.Seek(4);    // copy occurs
            stm.Mark();
            Assert.AreEqual(" stream", stm.Substring(4, 10));
        }

        [Test]
        public void SubstringNoStartStop_ReturnsMarkToPreviousPosition()
        {
            ReaderCharacterStream stm = new ReaderCharacterStream("test stream", 4);
            stm.Consume();
            stm.Mark();
            stm.Consume(4);
            Assert.AreEqual("est ", stm.Substring(), "substring()");
        }
    }
}
