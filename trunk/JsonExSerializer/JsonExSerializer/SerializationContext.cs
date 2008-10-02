/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;
using System.Reflection;
using JsonExSerializer.Collections;
using JsonExSerializer.MetaData;
using System.Collections;
using JsonExSerializer.Framework;
using JsonExSerializer.Framework.ObjectHandlers;

namespace JsonExSerializer
{
    /// <summary>
    /// Provides options controlling Serializing and Deserializing of objects.
    /// </summary>
    public class SerializationContext
    {

        #region Member variables

        private bool _isCompact;
        private bool _outputTypeComment;
        private bool _outputTypeInformation;
        public enum ReferenceOption
        {
            WriteIdentifier,
            IgnoreCircularReferences,
            ErrorCircularReferences
        }

        private ReferenceOption _referenceWritingType;

        private TwoWayDictionary<Type, string> _typeBindings;
        private Serializer _serializerInstance;
        private List<CollectionHandler> _collectionHandlers;
        
        private TypeHandlerFactory _typeHandlerFactory;
        private IDictionary _parameters;

        private List<IParsingStage> _parsingStages;

        private ExpressionHandlerCollection _objectHandlers;

        /// <summary>
        /// Set of options for handling Ignored properties encountered upon Deserialization
        /// </summary>
        public enum IgnoredPropertyOption
        {
            Ignore,
            SetIfPossible,
            ThrowException
        }

        private IgnoredPropertyOption _ignoredPropertyAction = IgnoredPropertyOption.ThrowException;

        #endregion

        #region Constructor

        public SerializationContext()
        {
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

            // collections
            _collectionHandlers = new List<CollectionHandler>();
            _collectionHandlers.Add(new GenericCollectionHandler());
            _collectionHandlers.Add(new ArrayHandler());
            _collectionHandlers.Add(new ListHandler());
            _collectionHandlers.Add(new StackHandler());
            _collectionHandlers.Add(new GenericStackHandler());
            //_collectionHandlers.Add(new CollectionConstructorHandler());

            // type handlers
            _typeHandlerFactory = new TypeHandlerFactory(this);
            _parameters = new Hashtable();

            // type conversion
            _typeHandlerFactory.RegisterTypeConverter(typeof(System.Collections.BitArray), new BitArrayConverter());

            _objectHandlers = new ExpressionHandlerCollection(this);
        }

        #endregion

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

        /// <summary>
        /// Controls the action taken when an ignored property is encountered upon deserialization
        /// </summary>
        public IgnoredPropertyOption IgnoredPropertyAction
        {
            get { return this._ignoredPropertyAction; }
            set { _ignoredPropertyAction = value; }
        }
        #endregion

        #region Type Binding
        /// <summary>
        /// Adds a different binding for a type.  When the type is encountered during serialization, the alias
        /// will be written out instead of the normal type info if a cast or type information is needed.
        /// </summary>
        /// <param name="t">the type object</param>
        /// <param name="typeAlias">the type's alias</param>
        public void AddTypeBinding(Type type, string typeAlias)
        {
            _typeBindings[type] = typeAlias;
        }

        /// <summary>
        /// Clears all type bindings
        /// </summary>
        public void ClearTypeBindings()
        {
            _typeBindings.Clear();
        }

        /// <summary>
        /// Removes a type binding
        /// </summary>
        /// <param name="t">the bound type to remove</param>
        public void RemoveTypeBinding(Type type)
        {
            _typeBindings.Remove(type);
        }

        /// <summary>
        /// Removes a type binding
        /// </summary>
        /// <param name="alias">the type alias to remove</param>
        public void RemoveTypeBinding(string alias)
        {
            Type key;
            if (_typeBindings.TryGetKey(alias, out key))
                _typeBindings.Remove(key);

        }

        /// <summary>
        /// Looks up an alias for a given type that was registered with AddTypeBinding.
        /// </summary>
        /// <param name="t">the type to lookup</param>
        /// <returns>a type alias or null</returns>
        public string GetTypeAlias(Type type)
        {
            string alias = null;
            if (!_typeBindings.TryGetValue(type, out alias))
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

        #endregion


        /// <summary>
        /// The current serializer instance that created and is using this
        /// context.
        /// </summary>
        public Serializer SerializerInstance
        {
            get { return this._serializerInstance; }
            internal set
            {
                if (this._serializerInstance != null)
                {
                    throw new InvalidOperationException("SerializationContext can not be used in more than one Serializer instance");
                }
                this._serializerInstance = value;
            }
        }

        /// <summary>
        /// User defined parameters
        /// </summary>
        public IDictionary Parameters
        {
            get { return _parameters; }
        }

        internal IList<IParsingStage> ParsingStages
        {
            get
            {
                if (_parsingStages == null)
                {
                    _parsingStages = new List<IParsingStage>();
                    _parsingStages.Add(new AssignReferenceStage(this));
                }
                return _parsingStages;
            }
        }
        #region TypeConverter

        /// <summary>
        /// Register a type converter with the DefaultConverterFactory.
        /// </summary>
        /// <param name="forType">the type to register</param>
        /// <param name="converter">the converter</param>
        public void RegisterTypeConverter(Type forType, IJsonTypeConverter converter)
        {
            TypeHandlerFactory.RegisterTypeConverter(forType, converter);
        }

        /// <summary>
        /// Register a type converter with the DefaultConverterFactory.
        /// </summary>
        /// <param name="forType">the property to register</param>
        /// <param name="converter">the converter</param>
        public void RegisterTypeConverter(Type forType, string propertyName, IJsonTypeConverter converter)
        {
            TypeHandlerFactory.RegisterTypeConverter(forType, propertyName, converter);
        }

        #endregion

        #region Handlers

        /// <summary>
        /// Registers a collection handler which provides support for a certain type
        /// or multiple types of collections.
        /// </summary>
        /// <param name="handler">the collection handler</param>
        public void RegisterCollectionHandler(CollectionHandler handler)
        {
            _collectionHandlers.Insert(0, handler);
        }

        /// <summary>
        /// CollectionHandlers provide support for collections(JSON Arrays)
        /// </summary>
        public List<CollectionHandler> CollectionHandlers
        {
            get { return _collectionHandlers; }
        }

        public TypeHandler GetTypeHandler(Type objectType)
        {
            return TypeHandlerFactory[objectType];
        }

        /// <summary>
        /// Gets or sets the TypeHandlerFactory which is responsible for
        /// creating ITypeHandlers which manage type metadata
        /// </summary>
        public TypeHandlerFactory TypeHandlerFactory
        {
            get { return _typeHandlerFactory; }
            set { _typeHandlerFactory = value; }
        }

        public ExpressionHandlerCollection ObjectHandlers
        {
            get { return _objectHandlers; }
            set { _objectHandlers = value; }
        }

        #endregion

        #region Ignore Properties
        /// <summary>
        /// Ignore a property to keep from being serialized, same as if the JsonExIgnore attribute had been set
        /// </summary>
        /// <param name="objectType">the type that contains the property</param>
        /// <param name="propertyName">the name of the property</param>
        public void IgnoreProperty(Type objectType, string propertyName)
        {
            TypeHandler handler = GetTypeHandler(objectType);
            handler.IgnoreProperty(propertyName);
        }

        #endregion

    }
}
