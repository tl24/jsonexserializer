using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public class ObjectHandlerCollection : CollectionBase, IContextAware
    {
        private IObjectHandler _valueHandler;
        private IObjectHandler _numericHandler;
        private IObjectHandler _booleanHandler;
        private IObjectHandler _nullHandler;
        private IObjectHandler _defaultHandler;
        private SerializationContext _context;

        public ObjectHandlerCollection(SerializationContext Context)
        {
            _context = Context;
            _valueHandler = new ValueObjectHandler(Context);
            _numericHandler = new NumericObjectHandler(Context);
            _booleanHandler = new BooleanObjectHandler(Context);
            _defaultHandler = new JsonObjectHandler(Context);
            this.InnerList.Add(new CollectionObjectHandler(Context));
            this.InnerList.Add(new DictionaryObjectHandler(Context));
        }

        public IObjectHandler ValueHandler
        {
            get { return this._valueHandler; }
            set { this._valueHandler = value; }
        }

        public IObjectHandler NumericHandler
        {
            get { return this._numericHandler; }
            set { this._numericHandler = value; }
        }

        public IObjectHandler BooleanHandler
        {
            get { return this._booleanHandler; }
            set { this._booleanHandler = value; }
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

        public IObjectHandler GetHandler(object Data)
        {
            if (Data == null)
                return NullHandler;

            foreach (IObjectHandler handler in this)
                if (handler.CanHandle(Data))
                    return handler;

            return DefaultHandler;
        }
    }

}
