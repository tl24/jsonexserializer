using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;

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
        private TwoWayDictionary<Type, string> _typeBindings;
        private Serializer _serializerInstance;
        private TypeConverterFactory _converterFactory;

        public SerializationContext(Serializer serializerInstance)
        {
            this._serializerInstance = serializerInstance;
            _isCompact = false;
            _outputTypeComment = true;
            _outputTypeInformation = true;
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
            _converterFactory = new TypeConverterFactory();
        }

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

        /// <summary>
        /// Gets a factory for converting objects between various types
        /// </summary>
        public TypeConverterFactory ConverterFactory
        {
            get { return this._converterFactory; }
        }


    }
}
