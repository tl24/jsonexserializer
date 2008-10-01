using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public abstract class ObjectHandlerBase : IObjectHandler, IContextAware
    {
        private SerializationContext _context;

        protected ObjectHandlerBase()
        {
        }

        protected ObjectHandlerBase(SerializationContext Context)
        {
            _context = Context;
        }

        public virtual SerializationContext Context
        {
            get { return _context; }
            set { _context = value; }
        }


        public abstract ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler serializer);

        public abstract bool CanHandle(Type ObjectType);

        public virtual bool CanHandle(ExpressionBase Expression)
        {
            return false;
        }

        public abstract object Evaluate(ExpressionBase Expression, IDeserializerHandler deserializer);

        public abstract object Evaluate(ExpressionBase Expression, object existingObject, IDeserializerHandler deserializer);
    }
}
