using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using JsonExSerializer;
using System.Collections;
using JsonExSerializerTests.Mocks;

namespace JsonExSerializerTests
{
    [TestFixture]
    public class CollectionsTests
    {
        [Test]
        public void StringArrayListTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(ArrayList));
            ArrayList strings = new ArrayList();
            strings.Add("one");
            strings.Add("two");
            strings.Add("3");
            string result = s.Serialize(strings);
            ArrayList actual = (ArrayList) s.Deserialize(result);
            Assert.AreEqual(strings, actual);
        }

        [Test]
        public void IntListTest()
        {
            Serializer s = Serializer.GetSerializer(typeof(List<int>));
            List<int> ints = new List<int>();
            ints.Add(0);
            ints.Add(int.MinValue);
            ints.Add(int.MaxValue);
            ints.Add(-23);
            ints.Add(456);
            ints.Add(int.MaxValue - 1);
            ints.Add(int.MinValue + 1);
            string result = s.Serialize(ints);
            List<int> actual = (List<int>)s.Deserialize(result);
            Assert.AreEqual(ints, actual);
        }

        [Test]
        public void SimpleObjectLinkedList()
        {
            Serializer s = Serializer.GetSerializer(typeof(LinkedList<SimpleObject>));
            LinkedList<SimpleObject> objects = new LinkedList<SimpleObject>();
            SimpleObject obj = null;
            
            // object 1
            obj = new SimpleObject();
            obj.BoolValue = true;
            obj.ByteValue = 0xf1;
            obj.CharValue = 'a';
            obj.DoubleValue = double.MinValue;
            obj.FloatValue = float.MinValue;
            obj.IntValue = 32;
            obj.LongValue = 39000;
            obj.ShortValue = 255;
            obj.StringValue = "AA";

            objects.AddLast(obj);

            // object 2
            obj = new SimpleObject();
            obj.BoolValue = false;
            obj.ByteValue = 0xf2;
            obj.CharValue = 'b';
            obj.DoubleValue = double.MaxValue;
            obj.FloatValue = float.MaxValue;
            obj.IntValue = 33;
            obj.LongValue = 39001;
            obj.ShortValue = 256;
            obj.StringValue = "BB";

            objects.AddLast(obj);

            string result = s.Serialize(objects);

            LinkedList<SimpleObject> actual = (LinkedList<SimpleObject>)s.Deserialize(result);

            Assert.AreEqual(objects, actual);
        }

        [Test]
        public void NonGenericQueueTest()
        {
            Queue expectedQueue = new Queue();
            expectedQueue.Enqueue(4);
            expectedQueue.Enqueue(2);
            expectedQueue.Enqueue(1);
            Serializer s = Serializer.GetSerializer(typeof(Queue));
            string result = s.Serialize(expectedQueue);

            Queue actualQueue = (Queue)s.Deserialize(result);
            Assert.AreEqual(expectedQueue, actualQueue, "Non-generic queues not equal");
        }

        [Test]
        public void GenericQueueTest()
        {
            Queue<float> expectedQueue = new Queue<float>();
            expectedQueue.Enqueue(4.3f);
            expectedQueue.Enqueue(2.9934f);
            expectedQueue.Enqueue(-0.456f);
            Serializer s = Serializer.GetSerializer(typeof(Queue<float>));
            string result = s.Serialize(expectedQueue);

            Queue<float> actualQueue = (Queue<float>)s.Deserialize(result);
            Assert.AreEqual(expectedQueue, actualQueue, "Generic queues not equal");
        }

        [Test]
        public void GenericCastTest()
        {
            Queue<float> expectedQueue = new Queue<float>();
            expectedQueue.Enqueue(float.MaxValue);
            Serializer s = Serializer.GetSerializer(typeof(object));
            string result = s.Serialize(expectedQueue);
            Queue<float> actualQueue = (Queue<float>)s.Deserialize(result);
            Assert.AreEqual(expectedQueue, actualQueue, "Generic types not equal");
        }
    }
}
