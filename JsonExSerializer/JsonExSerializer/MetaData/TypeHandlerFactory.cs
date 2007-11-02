using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Factory for TypeHandlers
    /// </summary>
    public class TypeHandlerFactory : JsonExSerializer.MetaData.ITypeHandlerFactory
    {
        private SerializationContext _context;
        private IDictionary<Type, ITypeHandler> _cache;

        public TypeHandlerFactory(SerializationContext context)
        {
            _context = context;
            _cache = new Dictionary<Type, ITypeHandler>();
        }

        public virtual ITypeHandler CreateTypeHandler(Type forType)
        {
            ITypeHandler handler;
            if (!_cache.ContainsKey(forType))
            {
                _cache[forType] = handler = CreateNew(forType);
            }
            else
            {
                handler = _cache[forType];
            }
            return handler;
            
        }

        protected virtual ITypeHandler CreateNew(Type forType)
        {
            return new TypeHandler(forType, _context);
        }
    }
}
