using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers.Collections
{
    public class QueueHandler : CollectionObjectHandlerBase
    {
        public QueueHandler() { }
        public QueueHandler(SerializationContext Context) : base(Context) { }

        protected override void AddItem(object Collection, object itemResult)
        {
            ((Queue)Collection).Enqueue(itemResult);
        }

        public override bool CanHandle(Type ObjectType)
        {
            return typeof(Queue).IsAssignableFrom(ObjectType);
        }
    }

    public class GenericQueueHandler : CollectionObjectHandlerBase
    {
        public GenericQueueHandler() { }
        public GenericQueueHandler(SerializationContext Context) : base(Context) { }

        public override bool CanHandle(Type ObjectType)
        {
            return ObjectType.IsGenericType && typeof(Queue<>).IsAssignableFrom(ObjectType.GetGenericTypeDefinition());
        }

        public override object Evaluate(ExpressionBase Expression, object ExistingObject, IDeserializerHandler Deserializer)
        {
            Expression.OnObjectConstructed(ExistingObject);
            Type collectionType = null;
            if (ExistingObject != null)
                collectionType = ExistingObject.GetType();
            else
                collectionType = Expression.ResultType;
            Type itemType = GetItemType(collectionType);
            Type wrapperType = typeof(GenericQueueWrapper<>).MakeGenericType(itemType);
            IList wrapper = (IList) Activator.CreateInstance(wrapperType, ExistingObject);
            foreach (ExpressionBase itemExpr in ((ListExpression)Expression).Items)
            {
                itemExpr.ResultType = itemType;
                wrapper.Add(Deserializer.Evaluate(itemExpr));
            }
            if (ExistingObject is IDeserializationCallback)
                ((IDeserializationCallback)ExistingObject).OnAfterDeserialization();
            return ExistingObject;
        }
        protected override void EvaluateItems(ListExpression Expression, object Collection, Type ItemType, IDeserializerHandler Deserializer)
        {
            base.EvaluateItems(Expression, Collection, ItemType, Deserializer);
        }

        protected override void AddItem(object Collection, object itemResult)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override Type GetItemType(Type CollectionType)
        {
            return CollectionType.GetGenericArguments()[0];
        }

        private class GenericQueueWrapper<T> : IList
        {
            public Queue<T> instance;
            public GenericQueueWrapper(Queue<T> queue)
            {
                instance = queue ?? new Queue<T>();
            }

            public object Value
            {
                get { return instance; }
            }

            public int Add(object Item)
            {
                instance.Enqueue((T)Item);
                return instance.Count;
            }

            public void Clear()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public bool Contains(object value)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public int IndexOf(object value)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void Insert(int index, object value)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public bool IsFixedSize
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            public bool IsReadOnly
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            public void Remove(object value)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void RemoveAt(int index)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public object this[int index]
            {
                get
                {
                    throw new Exception("The method or operation is not implemented.");
                }
                set
                {
                    throw new Exception("The method or operation is not implemented.");
                }
            }

            public void CopyTo(Array array, int index)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public int Count
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            public bool IsSynchronized
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            public object SyncRoot
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            public IEnumerator GetEnumerator()
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
    }
}
