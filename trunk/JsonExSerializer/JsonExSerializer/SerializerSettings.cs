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
using JsonExSerializer.Framework.ExpressionHandlers;
using JsonExSerializer.Framework.Parsing;

namespace JsonExSerializer
{

    /// <summary>
    /// Provides options controlling Serializing and Deserializing of objects.
    /// </summary>
    public class SerializerSettings : ISerializerSettings
    {

        #region Member variables
        
        private List<IParsingStage> _parsingStages;


        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instead of serializer settings with default values
        /// </summary>
        public SerializerSettings()
        {
            OutputTypeInformation = true;
            IgnoredPropertyAction = IgnoredPropertyOption.ThrowException;
            MissingPropertyAction = MissingPropertyOptions.Ignore;
            ReferenceWritingType = ReferenceOption.ErrorCircularReferences;
            DefaultValueSetting = DefaultValueOption.WriteAllValues;

            TypeAliases = new TypeAliasCollection();

            // collections
            CollectionHandlers = new List<CollectionHandler>();
            CollectionHandlers.Add(new GenericCollectionHandler());
            CollectionHandlers.Add(new ArrayHandler());
            CollectionHandlers.Add(new ListHandler());
            CollectionHandlers.Add(new StackHandler());
            CollectionHandlers.Add(new GenericStackHandler());
            CollectionHandlers.Add(new CollectionConstructorHandler());

            // type handlers
            Types = new TypeDataRepository(this);
            Parameters = new Hashtable();

            // type conversion
            Types.RegisterTypeConverter(typeof(System.Collections.BitArray), new BitArrayConverter());

            ExpressionHandlers = new ExpressionHandlerCollection(this);
            DefaultValues = new DefaultValueCollection();
        }

        #endregion

        #region Options
        /// <summary>
        /// If true, string output will be as compact as possible with minimal spacing.  Thus, cutting
        /// down on space.  This option has no effect on Deserialization.
        /// </summary>
        public bool IsCompact { get; set; }

        /// <summary>
        /// This will set the serializer to output in Json strict mode which will only
        /// output information compatible with the JSON standard.
        /// </summary>
        public void SetJsonStrictOptions()
        {
            OutputTypeInformation = false;
            ReferenceWritingType = ReferenceOption.ErrorCircularReferences;
        }

        /// <summary>
        /// If set to true, type information will be written when necessary to properly deserialize the 
        /// object.  This is only when the type information derived from the serialized type will not
        /// be specific enough to deserialize correctly.  
        /// </summary>
        public bool OutputTypeInformation { get; set; }

        /// <summary>
        /// Controls how references to previously serialized objects are written out.
        /// If the option is set to WriteIdentifier a reference identifier is written.
        /// The reference identifier is the path from the root to the first reference to the object.
        /// Example: $['SomeProp'][1]['MyClassVar'];
        /// Otherwise a copy of the object is written unless a circular reference is detected, then
        /// this option controls how the circular reference is handled.  If IgnoreCircularReferences
        /// is set, then null is written when a circular reference is detected.  If ErrorCircularReferences
        /// is set, then an error will be thrown.
        /// </summary>
        public ReferenceOption ReferenceWritingType { get; set; }

        /// <summary>
        /// Controls the action taken when an ignored property is encountered upon deserialization
        /// </summary>
        public IgnoredPropertyOption IgnoredPropertyAction { get; set; }

        /// <summary>
        /// Controls the action taken during deserialization when a property is specified in the Json Text,
        /// but does not exist on the class or object.
        /// </summary>
        public MissingPropertyOptions MissingPropertyAction { get; set; }

        /// <summary>
        /// Controls whether properties of an object with the default value set are suppressed when being
        /// serialized.  The default is to write all values during serialization.
        /// </summary>
        public DefaultValueOption DefaultValueSetting { get; set; }

        /// <summary>
        /// Gets the effective DefaultValueSetting in use
        /// </summary>
        /// <returns>DefaultValueSetting</returns>
        public virtual DefaultValueOption GetEffectiveDefaultValueSetting()
        {
            //NOTE: We don't ever return Default, because the buck stops here
            if (this.DefaultValueSetting == DefaultValueOption.SuppressDefaultValues)
                return DefaultValueOption.SuppressDefaultValues;
            else
                return DefaultValueOption.WriteAllValues;
        }

        #endregion

        /// <summary>
        /// Collection of Type-Alias mappings.  When a type has a registered alias, the alias will be 
        /// rendered in place of the type any time the type would normally be rendered.
        /// </summary>
        public TypeAliasCollection TypeAliases { get; set; }

        /// <summary>
        /// User defined parameters
        /// </summary>
        public IDictionary Parameters { get; private set; }

        /// <summary>
        /// Gets the list of parsing stages.  Parsing stages define the processing
        /// that occurs during parsing of the Json and allows changes to be made
        /// to the document tree before it is turned into object instances.
        /// </summary>
        public IList<IParsingStage> ParsingStages
        {
            get
            {
                if (_parsingStages == null)
                {
                    _parsingStages = new List<IParsingStage>();
                    _parsingStages.Add(new AssignReferenceStage());
                }
                return _parsingStages;
            }
        }

        /// <summary>
        /// Gets or sets the default value for a specified type
        /// </summary>
        public DefaultValueCollection DefaultValues { get; set; }

        public static void SetConfigurationAware(object value, ISerializerSettings config)
        {
            IConfigurationAware contextAware = value as IConfigurationAware;
            if (contextAware != null)
                contextAware.Settings = config;
        }

        /// <summary>
        /// Checks to see if by default this is a referencable type
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public virtual bool IsReferenceableType(Type objectType)
        {
            return objectType.IsClass && objectType != typeof(string);
        }

        /// <summary>
        /// Registers a collection handler which provides support for a certain type
        /// or multiple types of collections.
        /// </summary>
        /// <param name="handler">the collection handler</param>
        public void RegisterCollectionHandler(CollectionHandler handler)
        {
            CollectionHandlers.Insert(0, handler);
        }

        /// <summary>
        /// CollectionHandlers provide support for collections(JSON Arrays)
        /// </summary>
        public List<CollectionHandler> CollectionHandlers { get; private set; }

        /// <summary>
        /// Gets or sets the TypeHandlerFactory which is responsible for
        /// creating TypeData instances which manage type metadata
        /// </summary>
        public ITypeSettings Types { get; set; }

        public ExpressionHandlerCollection ExpressionHandlers { get; set; }
    }
}
