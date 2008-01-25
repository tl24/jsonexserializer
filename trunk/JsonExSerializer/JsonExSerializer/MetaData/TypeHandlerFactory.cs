using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Factory for TypeHandlers
    /// </summary>
    public class TypeHandlerFactory : ITypeHandlerFactory
    {
   
        private SerializationContext _context;
        private IDictionary<Type, ITypeHandler> _cache;

        public TypeHandlerFactory(SerializationContext context)
        {
            _context = context;
            _cache = new Dictionary<Type, ITypeHandler>();
        }

        public SerializationContext Context
        {
            get { return this._context; }
        }

        public ITypeHandler this[Type forType]
        {
            get { return CreateTypeHandler(forType); }
        }

        private ITypeHandler CreateTypeHandler(Type forType)
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

        public void RegisterTypeConverter(Type forType, IJsonTypeConverter converter)
        {
            if (forType.IsPrimitive || forType == typeof(string))
                throw new JsonExSerializationException("Converters can not be registered for primitive types or string. " + forType);
            this[forType].TypeConverter = converter;
        }

        public void RegisterTypeConverter(Type forType, string PropertyName, IJsonTypeConverter converter)
        {
            this[forType].FindProperty(PropertyName).TypeConverter = converter;
        }
    }
}
