/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using JsonExSerializer.Collections;
using JsonExSerializer.MetaData;
using JsonExSerializer.TypeConversion;
using JsonExSerializer.Framework;
using System.Diagnostics;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Helper class for dealing with types during serialization
    /// </summary>
    public class TypeData : MetaDataBase
    {
        /// <summary>
        /// The properties for this type
        /// </summary>
        protected IList<IPropertyData> properties;

        /// <summary>
        /// The properties that also correspond to constructor parameters
        /// </summary>
        protected IList<IPropertyData> constructorArgs;

        /// <summary>
        /// Flag that indicates whether the collection handler lookup has been attempted
        /// </summary>
        private bool collectionLookedUp;

        /// <summary>
        /// The collection handler for this type
        /// </summary>
        private CollectionHandler collectionHandler;

        /// <summary>
        /// The serializer's configuration
        /// </summary>
        protected IConfiguration config;

        /// <summary>
        /// flag indicating whether this type has any properties that are not ignored
        /// </summary>
        private bool? empty;

        /// <summary>
        /// Holds default values based on type
        /// </summary>
        private DefaultValueCollection _defaultValues;

        /// <summary>
        /// Initializes an instance with the specific <paramref name="type"/> and
        /// <paramref name="context" />.
        /// </summary>
        /// <param name="type">the .NET type that the metadata is for</param>
        /// <param name="context">the serializer context</param>
        public TypeData(Type type, IConfiguration config) : base(type)
        {
            this.config = config;
        }

        /// <summary>
        /// Loads the properties for the type if they haven't already been loaded
        /// </summary>
        protected virtual void LoadProperties()
        {
            if (properties == null)
            {
                this.properties = ReadDeclaredProperties();
                MergeBaseProperties(this.properties);
            }
        }

        private void LoadConstructorParameters()
        {
            if (this.constructorArgs == null)
            {
                this.constructorArgs = new List<IPropertyData>(GetConstructorParameters(properties));
                if (constructorArgs.Count > 0)
                {
                    constructorArgs = SortConstructorParameters(constructorArgs);
                }
            }
        }

        /// <summary>
        /// Reads the properties and constructor arguments from type metadata declared on this type
        /// </summary>
        /// <param name="Properties">properties collection</param>
        /// <param name="ConstructorArguments">constructor arguments</param>
        protected virtual IList<IPropertyData> ReadDeclaredProperties()
        {
            IList<IPropertyData> properties = new List<IPropertyData>();

            MemberInfo[] members = ForType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (MemberInfo member in members)
            {
                IPropertyData property = null;
                if (member is PropertyInfo)
                {
                    property = CreatePropertyHandler((PropertyInfo) member);
                }
                else if (member is FieldInfo)
                {
                    property = CreateFieldHandler((FieldInfo) member);
                }
                if (property != null) {
                    properties.Add(property);
                }
            }
            return properties;
        }

        /// <summary>
        /// Merges in the properties from the base class to the property list
        /// </summary>
        /// <param name="properties">property list to merge into</param>
        protected virtual void MergeBaseProperties(IList<IPropertyData> properties) {
            if (this.forType.BaseType == typeof(object) || this.forType.BaseType == null)
                return;

            TypeData baseTypeData = this.config.TypeHandlerFactory[this.forType.BaseType];
            List<IPropertyData> baseProps = new List<IPropertyData>(baseTypeData.AllProperties);
            foreach (IPropertyData baseProp in baseProps)
            {
                bool found = false;
                foreach (IPropertyData localProp in properties)
                {
                    if (localProp.Name == baseProp.Name)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    properties.Add(baseProp);
            }
        }

        /// <summary>
        /// Returns an enumerator of the Constructor Parameters in the <paramref name="properties"/> list.
        /// </summary>
        /// <param name="properties">the properties to extract constructor parameters from</param>
        /// <returns>enumerable list of constructor parameters</returns>
        private static IEnumerable<IPropertyData> GetConstructorParameters(IList<IPropertyData> properties)
        {
            foreach (IPropertyData property in properties)
                if (property.IsConstructorArgument)
                    yield return property;
        }

        private IList<IPropertyData> SortConstructorParameters(IList<IPropertyData> parameters)
        {
            IList<IPropertyData> newList = null;
            newList = SortConstructorParameters(parameters, true);
            if (newList != null)
                return newList;

            newList = SortConstructorParameters(parameters, false);
            if (newList != null)
                return newList;

            throw new Exception("Unable to find suitable public constructor matching constructor parameters for " + this.ForType);
        }

        private IList<IPropertyData> SortConstructorParameters(IList<IPropertyData> parameters, bool exactMatch)
        {
            StringComparison comparison = exactMatch ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            foreach(ConstructorInfo constructor in ForType.GetConstructors()) {
                ParameterInfo[] ctorParms = constructor.GetParameters();
                if (ctorParms.Length != parameters.Count)
                    continue;

                IPropertyData[] result = new IPropertyData[parameters.Count];
                int matchCount = 0;
                foreach (IPropertyData property in parameters)
                {
                    if (property.Position >= 0)
                    {
                        if (exactMatch && ctorParms[property.Position].ParameterType != property.PropertyType)
                            break;

                        //if (!exactMatch && !ctorParms[property.Position].ParameterType.IsAssignableFrom(property.PropertyType))
                        //    break;

                        if (result[property.Position] == null)
                        {
                            result[property.Position] = property;
                            matchCount++;
                        }
                        else
                        {
                            // something else occupies this spot already, so there's a conflict with this constructor so try another
                            // most likely none of them will work in this situation, but we'll let the outer method determine that
                            break;
                        }
                    }
                    else
                    {
                        // named argument, search the list
                        int i;
                        for (i = 0; i < ctorParms.Length; i++)
                        {
                            string parmName = ctorParms[i].Name;
                            if (ctorParms[i].IsDefined(typeof(ConstructorParameterAttribute), false))
                                parmName = ReflectionUtils.GetAttribute<ConstructorParameterAttribute>(ctorParms[i], false).Name;

                            if (parmName.Equals(property.ConstructorParameterName, comparison)
                                && ((exactMatch && ctorParms[i].ParameterType == property.PropertyType)
                                    || (!exactMatch/* && ctorParms[i].ParameterType.IsAssignableFrom(property.PropertyType)*/)))
                                break;
                        }
                        if (i < ctorParms.Length && result[i] == null)
                        {
                            result[i] = property;
                            matchCount++;
                        }
                        else
                            break;
                    }
                }
                if (matchCount == result.Length)
                {
                    // found a match, return it
                    if (exactMatch)
                        return result;

                    if (IsLooseMatch(constructor, result))
                        return result;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks that the constructor matches the constructor parameters by doing type conversion if necessary
        /// </summary>
        /// <param name="constructor">the constructor</param>
        /// <param name="properties">constructor parameter fields</param>
        /// <returns>true if this constructor matches the types in the constructor parameters</returns>
        private bool IsLooseMatch(ConstructorInfo constructor, IPropertyData[] properties)
        {
            Type[] types = new Type[properties.Length];
            for(int i = 0; i < properties.Length; i++)
                types[i] = properties[i].PropertyType;

            try
            {
                MethodBase mb = Type.DefaultBinder.SelectMethod(BindingFlags.Default, new MethodBase[] { constructor }, types, new ParameterModifier[0]);
                return mb == constructor;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Constructs a PropertyHandler instance from the PropertyInfo
        /// </summary>
        /// <param name="Property"></param>
        /// <returns></returns>
        protected virtual PropertyData CreatePropertyHandler(PropertyInfo Property)
        {
            PropertyData propData = new PropertyData(Property, this);
            config.TypeHandlerFactory.ProcessAttributes(propData, Property);
            return propData;
        }

        /// <summary>
        /// Constructs a FieldHandler instance from the FieldInfo
        /// </summary>
        /// <param name="Field"></param>
        /// <returns></returns>
        protected virtual FieldData CreateFieldHandler(FieldInfo Field)
        {
            FieldData fieldData = new FieldData(Field, this);
            config.TypeHandlerFactory.ProcessAttributes(fieldData, Field);
            return fieldData;
        }

        /// <summary>
        /// Creates an instance of this Type with the specified arguments
        /// </summary>
        /// <param name="args">the arguments passed to the constructor if any</param>
        /// <returns>the created object</returns>
        public virtual object CreateInstance(object[] args)
        {
            return Activator.CreateInstance(this.ForType, args);
        }

        public virtual IConfiguration Config
        {
            get { return this.config; }
        }

        /// <summary>
        /// Get the list of constructor parameters for this type
        /// </summary>
        public virtual IList<IPropertyData> ConstructorParameters
        {
            get
            {
                LoadProperties();
                LoadConstructorParameters();
                return this.constructorArgs;
            }
        }

        /// <summary>
        /// Indicates whether there are any properties for this object that are not ignored.
        /// </summary>
        public virtual bool IsEmpty
        {
            get
            {
                if (!empty.HasValue)
                    foreach (IPropertyData prop in AllProperties)
                    {
                        if (!prop.Ignored)
                        {
                            empty = false;
                            break;
                        }
                    }
                if (!empty.HasValue)
                    empty = true;


                return empty.Value;
            }
        }

        public virtual IEnumerable<IPropertyData> Properties
        {
            get
            {
                foreach (IPropertyData prop in AllProperties)
                {
                    if (!prop.Ignored)
                        yield return prop;
                }
            }
        }
        /// <summary>
        /// Get the list of properties for this type
        /// </summary>
        public virtual IEnumerable<IPropertyData> AllProperties
        {
            get {
                LoadProperties();
                return properties; 
            }
        }

        /// <summary>
        /// Finds a property by its name.  The property must follow the same rules as
        /// those returned from the Properties list, i.e. must be readable and writable and
        /// not have an ignore attribute.
        /// </summary>
        /// <param name="Name">the name of the property</param>
        /// <returns>TypeHandlerProperty instance for the property or null if not found</returns>
        public IPropertyData FindProperty(string Name)
        {
            foreach (IPropertyData prop in AllProperties)
            {
                if (prop.Name == Name)
                    return prop;
            }
            return null;
        }

        /// <summary>
        /// Ignore a property to keep from being serialized, same as if the JsonExIgnore attribute had been set
        /// </summary>
        /// <param name="name">the name of the property</param>
        public virtual void IgnoreProperty(string name)
        {
            IPropertyData handler = FindProperty(name);
            if (handler == null)
                throw new ArgumentException("Property " + name + " does not exist on Type " + this.ForType, "name");
            handler.Ignored = true;
        }

        /// <summary>
        /// Returns true if this type is a collection type
        /// </summary>
        /// <param name="context">the serialization context</param>
        /// <returns>true if a collection</returns>
        public virtual bool IsCollection()
        {
            if (!collectionLookedUp)
            {
                collectionHandler = FindCollectionHandler();
                collectionLookedUp = true;
            }
            return collectionHandler != null;
        }

        public CollectionHandler FindCollectionHandler()
        {
            foreach (CollectionHandler handler in config.CollectionHandlers)
            {
                if (handler.IsCollection(ForType))
                {
                    return handler;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Returns a collection handler if this object is a collection
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [Obsolete("Use the CollectionHandler property instead")]
        public virtual CollectionHandler GetCollectionHandler()
        {
            if (IsCollection()) {
                return collectionHandler;
            } else {
                throw new InvalidOperationException("Type " + ForType + " is not recognized as a collection.  A collection handler (ICollectionHandler) may be necessary");
            }            
        }

        public virtual CollectionHandler CollectionHandler
        {
            get { return GetCollectionHandler(); }
            set
            {
                this.collectionHandler = value;
                collectionLookedUp = true;
            }
        }

        protected override IJsonTypeConverter CreateTypeConverter()
        {
            return TypeConverterAdapter.GetAdapter(ForType);
        }

        public override IJsonTypeConverter TypeConverter
        {
            get
            {
                if (ForType.IsPrimitive || ForType == typeof(string))
                {
                    converterCreated = true;
                    return null;
                }
                else
                {
                    bool tempCreated = converterCreated;
                    IJsonTypeConverter converterResult = base.TypeConverter;
                    // if no converter registered, try to use the base converter if applicable
                    if (!tempCreated && converterResult == null && this.ForType.BaseType != null)
                    {
                        converterResult = converterInstance = this.config.TypeHandlerFactory[this.forType.BaseType].TypeConverter;
                    }
                    return converterResult;
                }
            }
            set
            {
                base.TypeConverter = value;
            }
        }

        public override DefaultValueOption GetEffectiveDefaultValueSetting()
        {
            DefaultValueOption option = base.GetEffectiveDefaultValueSetting();
            if (option == DefaultValueOption.InheritParentSetting)
                return Config.GetEffectiveDefaultValueSetting();
            else
                return option;
        }

        public virtual DefaultValueCollection DefaultValues
        {
            get
            {
                if (_defaultValues == null)
                    _defaultValues = new DefaultValueCollection(Config.DefaultValues);
                return _defaultValues;
            }
            set
            {
                _defaultValues = value;
            }
        }

        public virtual object GetDefaultValue(Type forType)
        {
            if (_defaultValues == null)
                return Config.DefaultValues[forType];
            else
                return _defaultValues[forType];
        }
    }
}
