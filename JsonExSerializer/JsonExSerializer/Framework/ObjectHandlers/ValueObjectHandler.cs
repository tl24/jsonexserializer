using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public class ValueObjectHandler : ObjectHandlerBase, IObjectHandler
    {
        private SerializationContext _context;
        public ValueObjectHandler()
        {
        }

        public ValueObjectHandler(SerializationContext Context)
        {
            this.Context = Context;
        }

        public override ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler Serializer)
        {
            return new ValueExpression(data);
        }


        public override bool CanHandle(Type ObjectType)
        {
            return (typeof(string).IsAssignableFrom(ObjectType)
                || typeof(char).IsAssignableFrom(ObjectType)
                || typeof(DateTime).IsAssignableFrom(ObjectType));
        }
    }
}
