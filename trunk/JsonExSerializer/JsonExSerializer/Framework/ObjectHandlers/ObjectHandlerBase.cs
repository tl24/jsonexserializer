using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public abstract class ObjectHandlerBase : IObjectHandler, IContextAware
    {
        private SerializationContext _context;

        public ObjectHandlerBase()
        {
        }

        public ObjectHandlerBase(SerializationContext Context)
        {
            this.Context = Context;
        }

        public virtual SerializationContext Context
        {
            get { return _context; }
            set { _context = value; }
        }


        public abstract ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler Serializer);

        public abstract bool CanHandle(Type ObjectType);
    }
}
