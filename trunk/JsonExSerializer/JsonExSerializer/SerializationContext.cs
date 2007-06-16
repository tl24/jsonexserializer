using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;
using System.Reflection;
using JsonExSerializer.Collections;

namespace JsonExSerializer
{
    /// <summary>
    /// Provides options controlling Serializing and Deserializing of objects.
    /// </summary>
    public class SerializationContext
    {
        private bool _isCompact;
        private bool _outputTypeComment;
        private bool _outputTypeInformation;
        public enum ReferenceOption
        {
            //WriteIdentifier,
            IgnoreCircularReferences,
            ErrorCircularReferences
        }

        private ReferenceOption _referenceWritingType;

        private TwoWayDictionary<Type, string> _typeBindings;
        private Serializer _serializerInstance;
        private List<ITypeConverterFactory> _converterFactories;
        private DefaultConverterFactory _defaultConverterFactory;
        private List<ICollectionHandler> _collectionHandlers;
        private IDictionary<Type, TypeHandler> _cache;

        public SerializationContext(Serializer serializerInstance)
        {
            this._serializerInstance = serializerInstance;
            _isCompact = false;
            _outputTypeComment = true;
            _outputTypeInformation = true;
            _referenceWritingType = ReferenceOption.ErrorCircularReferences;

            _typeBindings = new TwoWayDictionary<Type, string>();
            // add bindings for primitive types
            _typeBindings[typeof(byte)] = "byte";
            _typeBindings[typeof(sbyte)] = "sbyte";
            _typeBindings[typeof(bool)] = "bool";
            _typeBindings[typeof(short)] = "short";
            _typeBindings[typeof(ushort)] = "ushort";
            _typeBindings[typeof(int)] = "int";
            _typeBindings[typeof(uint)] = "uint";
            _typeBindings[typeof(long)] = "long";
            _typeBindings[typeof(ulong)] = "ulong";
            _typeBindings[typeof(string)] = "string";
            _typeBindings[typeof(object)] = "object";
            _typeBindings[typeof(char)] = "char";
            _typeBindings[typeof(decimal)] = "decimal";
            _typeBindings[typeof(float)] = "float";
            _typeBindings[typeof(double)] = "double";

            // type conversion
            _defaultConverterFactory = new DefaultConverterFactory();
            _converterFactories = new List<ITypeConverterFactory>();
            _converterFactories.Add(_defaultConverterFactory);
            _defaultConverterFactory.RegisterConverter(typeof(System.Collections.BitArray), new BitArrayConverter());

            // collections
            _collectionHandlers = new List<ICollectionHandler>();
            _collectionHandlers.Add(new GenericCollectionHandler());
            _collectionHandlers.Add(new ArrayHandler());
            _collectionHandlers.Add(new ListHandler());
            _collectionHandlers.Add(new StackHandler());
            _collectionHandlers.Add(new GenericStackHandler());
            _collectionHandlers.Add(new CollectionConstructorHandler());

            // type handlers
            _cache = new Dictionary<Type, TypeHandler>();
        }

        #region Options
        /// <summary>
        /// If true, string output will be as compact as possible with minimal spacing.  Thus, cutting
        /// down on space.  This option has no effect on Deserialization.
        /// </summary>
        public bool IsCompact
        {
            get { return this._isCompact; }
            set { this._isCompact = value; }
        }

        /// <summary>
        /// If true a comment will be written out containing type information for the root object
        /// </summary>
        public bool OutputTypeComment
        {
            get { return this._outputTypeComment; }
            set { this._outputTypeComment = value; }
        }

        /// <summary>
        /// This will set the serializer to output in Json strict mode which will only
        /// output information compatible with the JSON standard.
        /// </summary>
        public void SetJsonStrictOptions()
        {
            _outputTypeComment = false;
            _outputTypeInformation = false;
            _referenceWritingType = ReferenceOption.ErrorCircularReferences;
        }

        /// <summary>
        /// If set to true, type information will be written when necessary to properly deserialize the 
        /// object.  This is only when the type information derived from the serialized type will not
        /// be specific enough to deserialize correctly.  
        /// </summary>
        public bool OutputTypeInformation
        {
            get { return this._outputTypeInformation; }
            set { this._outputTypeInformation = value; }
        }

        /// <summary>
        /// Controls how references to previously serialized objects are written out.
        /// If the option is set to WriteIdentifier a reference identifier is written.
        /// The reference identifier is the path from the root to the first reference to the object.
        /// Example: this.SomeProp.1.MyClassVar;
        /// Otherwise a copy of the object is written unless a circular reference is detected, then
        /// this option controls how the circular reference is handled.  If IgnoreCircularReferences
        /// is set, then null is written when a circular reference is detected.  If ErrorCircularReferences
        /// is set, then an error will be thrown.
        /// </summary>
        public ReferenceOption ReferenceWritingType
        {
            get { return this._referenceWritingType; }
            set { this._referenceWritingType = value; }
        }

        #endregion

        /// <summary>
        /// Adds a different binding for a type
        /// </summary>
        /// <param name="t">the type object</param>
        /// <param name="typeAlias">the type's alias</param>
        public void AddTypeBinding(Type t, string typeAlias)
        {
            _typeBindings[t] = typeAlias;
        }

        /// <summary>
        /// Looks up an alias for a given type that was registered with AddTypeBinding.
        /// </summary>
        /// <param name="t">the type to lookup</param>
        /// <returns>a type alias or null</returns>
        public string GetTypeAlias(Type t)
        {
            string alias = null;
            if (!_typeBindings.TryGetValue(t, out alias))
            {
                alias = null;
            }
            return alias;
        }

       
        /// <summary>
        /// Looks up a type, given an alias for the type.
        /// </summary>
        /// <param name="typeAlias">the alias to look up</param>
        /// <returns>the type representing the alias or null</returns>
        public Type GetTypeBinding(string typeAlias)
        {
            Type t = null;
            if (!_typeBindings.TryGetKey(typeAlias, out t))
            {
                t = null;
            }
            return t;
        }

        /// <summary>
        /// The current serializer instance that created and is using this
        /// context.
        /// </summary>
        public Serializer SerializerInstance
        {
            get { return this._serializerInstance; }
        }

        #region TypeConverter

        /// <summary>
        /// Register a type converter with the DefaultConverterFactory.
        /// </summary>
        /// <param name="forType">the type to register</param>
        /// <param name="converter">the converter</param>
        public void RegisterTypeConverter(Type forType, IJsonTypeConverter converter)
        {
            _defaultConverterFactory.RegisterConverter(forType, converter);
        }

        /// <summary>
        /// Register a type converter with the DefaultConverterFactory.
        /// </summary>
        /// <param name="forType">the property to register</param>
        /// <param name="converter">the converter</param>
        public void RegisterTypeConverter(PropertyInfo forProperty, IJsonTypeConverter converter)
        {
            _defaultConverterFactory.RegisterConverter(forProperty, converter);
        }

        public void AddTypeConverterFactory(ITypeConverterFactory factory)
        {
            _converterFactories.Add(factory);
        }

        /// <summary>
        /// Constructs and returns a type converter for the property
        /// </summary>
        /// <param name="forProperty">property to convert</param>
        /// <returns>type converter</returns>
        public IJsonTypeConverter GetConverter(PropertyInfo forProperty)
        {
            
            foreach (ITypeConverterFactory factory in _converterFactories)
            {
                if (factory.HasConverter(forProperty))
                    return factory.GetConverter(forProperty);
            }
            return null;
        }

        /// <summary>
        /// Constructs and returns a type converter for the type
        /// </summary>
        /// <param name="forProperty">type to convert</param>
        /// <returns>type converter</returns>
        public IJsonTypeConverter GetConverter(Type forType)
        {
            foreach (ITypeConverterFactory factory in _converterFactories)
            {
                if (factory.HasConverter(forType))
                    return factory.GetConverter(forType);
            }
            return null;
        }

        /// <summary>
        /// Checks to see if a type converter can be found for the given type
        /// </summary>
        /// <param name="forType">type to check</param>
        /// <returns>true if the type has a converter</returns>
        public bool HasConverter(Type forType)
        {
            foreach (ITypeConverterFactory factory in _converterFactories)
            {
                if (factory.HasConverter(forType))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if a type converter can be found for the given property
        /// </summary>
        /// <param name="forProperty">property to check</param>
        /// <returns>true if the property has a converter</returns>
        public bool HasConverter(PropertyInfo forProperty)
        {
            foreach (ITypeConverterFactory factory in _converterFactories)
            {
                if (factory.HasConverter(forProperty))
                    return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Registers a collection handler which provides support for a certain type
        /// or multiple types of collections.
        /// </summary>
        /// <param name="handler">the collection handler</param>
        public void RegisterCollectionHandler(ICollectionHandler handler)
        {
            _collectionHandlers.Add(handler);
        }

        internal List<ICollectionHandler> CollectionHandlers
        {
            get { return _collectionHandlers; }
        }

        internal TypeHandler GetTypeHandler(Type objectType)
        {
            TypeHandler handler;
            if (!_cache.ContainsKey(objectType))
            {
                _cache[objectType] = handler = new TypeHandler(objectType);
            }
            else
            {
                handler = _cache[objectType];
            }
            return handler;
        }
    }
}
