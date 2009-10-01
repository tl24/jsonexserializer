using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;
using JsonExSerializer.MetaData.Attributes;
using System.Reflection;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Factory for TypeHandlers
    /// </summary>
    public class TypeDataRepository
    {
   
        private IConfiguration _config;
        private IDictionary<Type, TypeData> _cache;
        private List<AttributeProcessor> _attributeProcessors;
        private IPropertyNamingStrategy _propertyNamingStrategy;

        public TypeDataRepository(IConfiguration config)
        {
            _config = config;
            _cache = new Dictionary<Type, TypeData>();
            _attributeProcessors = new List<AttributeProcessor>();
            _attributeProcessors.Add(new JsonIgnoreAttributeProcessor());
            _attributeProcessors.Add(new JsonPropertyAttributeProcessor());
            _attributeProcessors.Add(new JsonDefaultAttributeProcessor());
            _attributeProcessors.Add(new ConstructorParameterAttributeProcessor());
            _attributeProcessors.Add(new TypeConverterAttributeProcessor());
            _attributeProcessors.Add(new JsonCollectionAttributeProcessor());
            _propertyNamingStrategy = new DefaultPropertyNamingStrategy();
        }

        public IConfiguration Config
        {
            get { return this._config; }
        }

        public virtual TypeData this[Type forType]
        {
            get { return CreateTypeHandler(forType); }
        }

        public virtual IList<AttributeProcessor> AttributeProcessors
        {
            get { return this._attributeProcessors; }
        }

        public virtual IPropertyNamingStrategy PropertyNamingStrategy
        {
            get { return this._propertyNamingStrategy; }
        }

        public void SetPropertyNamingStrategy(IPropertyNamingStrategy namingStrategy)
        {
            SetPropertyNamingStrategy(namingStrategy, true);
        }

        public void SetPropertyNamingStrategy(IPropertyNamingStrategy namingStrategy, bool clearExistingMetaData)
        {
            _propertyNamingStrategy = namingStrategy;
            if (clearExistingMetaData)
                _cache.Clear();
        }

        private TypeData CreateTypeHandler(Type forType)
        {
            TypeData handler;
            if (!_cache.ContainsKey(forType))
            {
                _cache[forType] = handler = CreateNew(forType);
                ProcessAttributes(handler, handler.ForType);
            }
            else
            {
                handler = _cache[forType];
            }
            return handler;            
        }

        protected virtual TypeData CreateNew(Type forType)
        {
            return new TypeData(forType, this);            
        }

        public virtual void RegisterTypeConverter(Type forType, IJsonTypeConverter converter)
        {
            if (forType.IsPrimitive || forType == typeof(string))
                throw new ArgumentException("Converters can not be registered for primitive types or string. " + forType, "forType");
            this[forType].TypeConverter = converter;      
        }

        public virtual void RegisterTypeConverter(Type forType, string PropertyName, IJsonTypeConverter converter)
        {
            this[forType].FindProperty(PropertyName).TypeConverter = converter;
        }

        public virtual void ProcessAttributes(IMetaData metaData, ICustomAttributeProvider attributeProvider)
        {
            foreach(AttributeProcessor processor in AttributeProcessors)
            {
                processor.Process(metaData, attributeProvider, this.Config);
            }
        }
    }
}
