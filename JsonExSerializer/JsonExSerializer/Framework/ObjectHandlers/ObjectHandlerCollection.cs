using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public class ObjectHandlerCollection : CollectionBase, IContextAware
    {
        private IObjectHandler _nullHandler;
        private IObjectHandler _defaultHandler;
        private SerializationContext _context;
        private Dictionary<Type, IObjectHandler> _cache = new Dictionary<Type, IObjectHandler>();
        public ObjectHandlerCollection(SerializationContext Context)
        {
            _context = Context;

            Add(new NumericObjectHandler(Context));
            Add(new BooleanObjectHandler(Context));
            Add(new ValueObjectHandler(Context));
            Add(new TypeConverterObjectHandler(Context));
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
            this.InnerList.Add(Handler);
        }

        public IObjectHandler Find(Type HandlerType)
        {
            foreach (IObjectHandler handler in this)
                if (handler.GetType() == HandlerType)
                    return handler;
            return null;
        }

        /// <summary>
        /// Retrieves a handler that can serialize the specified object
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public IObjectHandler GetHandler(object Data)
        {
            if (Data == null)
                return NullHandler;

            Type dataType = Data.GetType();
            return GetHandler(dataType);
        }

        /// <summary>
        /// Get a handler based on Expression type
        /// </summary>
        public IObjectHandler GetHandler(ExpressionBase Expression)
        {
            foreach (IObjectHandler handler in this)
                if (handler.CanHandle(Expression))
                    return handler;
            return GetHandler(Expression.ResultType);
        }

        /// <summary>
        /// Get a handler based on data type
        /// </summary>
        public IObjectHandler GetHandler(Type dataType)
        {
            foreach (IObjectHandler handler in this)
                if (handler.CanHandle(dataType))
                    return handler;
            return DefaultHandler;
        }
    }

}
