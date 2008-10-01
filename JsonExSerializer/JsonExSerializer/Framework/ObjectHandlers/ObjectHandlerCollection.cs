using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using JsonExSerializer.Expression;
using JsonExSerializer.Framework.ObjectHandlers.Collections;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public sealed class ObjectHandlerCollection : CollectionBase, IContextAware, IList<IObjectHandler>
    {
        private IObjectHandler _nullHandler;
        private IObjectHandler _defaultHandler;
        private SerializationContext _context;
        private Dictionary<Type, IObjectHandler> _cache = new Dictionary<Type, IObjectHandler>();
        public ObjectHandlerCollection(SerializationContext Context)
        {
            _context = Context;
            InitializeDefaultHandlers();
        }

        private void InitializeDefaultHandlers()
        {
            Add(new NumericObjectHandler(Context));
            Add(new BooleanObjectHandler(Context));
            Add(new ValueObjectHandler(Context));
            Add(new TypeConverterObjectHandler(Context));
            Add(new QueueHandler(Context));
            Add(new GenericQueueHandler(Context));
            Add(new CollectionObjectHandler(Context));
            Add(new DictionaryObjectHandler(Context));
            _nullHandler = new NullObjectHandler();
            Add(_nullHandler);
            Add(new CastObjectHandler(Context));
            Add(new ReferenceObjectHandler(Context));
            _defaultHandler = new JsonObjectHandler(Context);
        }


        public IObjectHandler DefaultHandler
        {
            get { return this._defaultHandler; }
            set { this._defaultHandler = value; }
        }

        public IObjectHandler NullHandler
        {
            get { return this._nullHandler; }
            set { this._nullHandler = value; }
        }

        public SerializationContext Context
        {
            get { return this._context; }
            set { this._context = value; }
        }

        public void Add(IObjectHandler Handler)
        {
            List.Add(Handler);
        }

        public IObjectHandler Find(Type handlerType)
        {
            foreach (IObjectHandler handler in this)
                if (handler.GetType() == handlerType)
                    return handler;
            return null;
        }

        /// <summary>
        /// Retrieves a handler that can serialize the specified object
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public IObjectHandler GetHandler(object data)
        {
            if (data == null)
                return NullHandler;

            Type dataType = data.GetType();
            return GetHandler(dataType);
        }

        /// <summary>
        /// Get a handler based on Expression type
        /// </summary>
        public IObjectHandler GetHandler(ExpressionBase expression)
        {
            foreach (IObjectHandler handler in this)
                if (handler.CanHandle(expression))
                    return handler;
            return GetHandler(expression.ResultType);
        }

        /// <summary>
        /// Get a handler based on data type
        /// </summary>
        public IObjectHandler GetHandler(Type dataType)
        {
            IObjectHandler h = null;
            if (!_cache.TryGetValue(dataType, out h))
            {
                foreach (IObjectHandler handler in this)
                    if (handler.CanHandle(dataType))
                    {
                        return _cache[dataType] = handler;
                        return handler;
                    }
            }
            return h ?? DefaultHandler;
        }

        public int IndexOf(IObjectHandler item)
        {
            return List.IndexOf(item);
        }

        public void Insert(int index, IObjectHandler item)
        {
            List.Insert(index, item);
        }

        public void InsertBefore(Type handlerType, IObjectHandler item)
        {
            int index = IndexOf(handlerType);
            if (index == -1)
                index = Count;
            Insert(index, item);
        }

        public void InsertAfter(Type handlerType, IObjectHandler item)
        {
            int index = IndexOf(handlerType);
            if (index == -1)
                index = Count;
            else
                index = Math.Min(index + 1, Count);
            Insert(index, item);
        }

        private int IndexOf(Type handlerType)
        {
            int index;
            for (index = 0; index < Count; index++)
                if (this[index].GetType() == handlerType)
                    return index;
            return -1;
        }


        public IObjectHandler this[int index]
        {
            get { return (IObjectHandler) List[index]; }
            set { List[index] = value; }
        }


        public bool Contains(IObjectHandler item)
        {
            return List.Contains(item);
        }

        public void CopyTo(IObjectHandler[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return List.IsReadOnly; }
        }

        public bool Remove(IObjectHandler item)
        {
            int index = IndexOf(item);
            if (index != -1)
                RemoveAt(index);
            return index != -1;
        }

        public new IEnumerator<IObjectHandler> GetEnumerator()
        {
            foreach (IObjectHandler handler in List)
                yield return handler;
        }

        private void InvalidateCache()
        {
            _cache.Clear();
        }
        protected override void OnClearComplete()
        {
            base.OnClearComplete();
            InvalidateCache();
        }

        protected override void OnInsertComplete(int index, object value)
        {
            base.OnInsertComplete(index, value);
            InvalidateCache();
        }

        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            base.OnSetComplete(index, oldValue, newValue);
            InvalidateCache();
        }

        /// <summary>
        /// Clears all existing handlers and resets to the default handlers
        /// </summary>
        public void Reset()
        {
            Clear();
            InitializeDefaultHandlers();
        }
    }

}
