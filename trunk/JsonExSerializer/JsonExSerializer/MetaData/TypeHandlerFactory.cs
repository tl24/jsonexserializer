using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Factory for TypeHandlers
    /// </summary>
    public class TypeHandlerFactory
    {
   
        private SerializationContext _context;
        private IDictionary<Type, TypeHandler> _cache;

        public TypeHandlerFactory(SerializationContext context)
        {
            _context = context;
            _cache = new Dictionary<Type, TypeHandler>();
        }

        public SerializationContext Context
        {
            get { return this._context; }
        }

        public TypeHandler this[Type forType]
        {
            get { return CreateTypeHandler(forType); }
        }

        private TypeHandler CreateTypeHandler(Type forType)
        {
            TypeHandler handler;
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

        protected virtual TypeHandler CreateNew(Type forType)
        {
            return new TypeHandler(forType, _context);
        }

        public void RegisterTypeConverter(Type forType, IJsonTypeConverter converter)
        {
            if (forType.IsPrimitive || forType == typeof(string))
                throw new ArgumentException("Converters can not be registered for primitive types or string. " + forType, "forType");
            this[forType].TypeConverter = converter;      
        }

        public void RegisterTypeConverter(Type forType, string PropertyName, IJsonTypeConverter converter)
        {
            this[forType].FindProperty(PropertyName).TypeConverter = converter;
        }
    }
}
