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

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Helper class for dealing with types during serialization
    /// </summary>
    public class TypeHandler : MemberHandlerBase
    {
        protected IList<AbstractPropertyHandler> _properties;
        protected IList<AbstractPropertyHandler> _constructorArgs;

        private bool _collectionLookedUp = false;
        private CollectionHandler _collectionHandler;
        protected SerializationContext _context;
        private IDictionary<string, bool> _tempIgnore;
        private bool? _empty;

        /// <summary>
        /// internal constructor
        /// </summary>
        /// <param name="t"></param>
        public TypeHandler(Type t, SerializationContext context) : base(t)
        {
            _context = context;
            _tempIgnore = new Dictionary<string, bool>();
        }

        /// <summary>
        /// Loads the properties for the type if they haven't already been loaded
        /// </summary>
        protected virtual void LoadProperties()
        {   
            if (_properties == null)
            {
                ReadProperties(out _properties, out _constructorArgs);
                if (_constructorArgs.Count > 0)
                {
                    ((List<AbstractPropertyHandler>)_constructorArgs).Sort(
                        new Comparison<AbstractPropertyHandler>(PropertyHandlerComparison));
                }
                _tempIgnore.Clear();
            }
        }

        /// <summary>
        /// Reads the properties and constructor arguments from type metadata
        /// </summary>
        /// <param name="Properties">properties collection</param>
        /// <param name="ConstructorArguments">constructor arguments</param>
        protected virtual void ReadProperties(out IList<AbstractPropertyHandler> Properties, out IList<AbstractPropertyHandler> ConstructorArguments)
        {
            Properties = new List<AbstractPropertyHandler>();
            ConstructorArguments = new List<AbstractPropertyHandler>();

            MemberInfo[] mInfos = ForType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
            foreach (MemberInfo mInfo in mInfos)
            {
                AbstractPropertyHandler prop = null;
                // must be able to read and write the prop, otherwise its not 2-way 
                if (mInfo is PropertyInfo)
                {
                    prop = new PropertyHandler((PropertyInfo) mInfo);
                }
                else if (mInfo is FieldInfo)
                {
                    prop = new FieldHandler((FieldInfo) mInfo);
                }
                if (prop != null) {
                    if (prop.IsConstructorArgument)
                        ConstructorArguments.Add(prop);
                    else
                        Properties.Add(prop);
                    if (_tempIgnore != null && _tempIgnore.ContainsKey(prop.Name))
                        prop.Ignored = true;
                }
            }
        }

        protected int PropertyHandlerComparison(AbstractPropertyHandler a, AbstractPropertyHandler b)
        {
            return a.Position - b.Position;
        }

        public virtual object CreateInstance(object[] args)
        {
            return Activator.CreateInstance(this.ForType, args);
        }

        /// <summary>
        /// Get the list of constructor parameters for this type
        /// </summary>
        public virtual IList<AbstractPropertyHandler> ConstructorParameters
        {
            get
            {
                LoadProperties();
                return _constructorArgs;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsEmpty
        {
            get
            {
                if (!_empty.HasValue)
                    foreach (AbstractPropertyHandler prop in AllProperties)
                    {
                        if (!prop.Ignored)
                        {
                            _empty = false;
                            break;
                        }
                    }
                if (!_empty.HasValue)
                    _empty = true;


                return _empty.Value;
            }
        }

        public virtual IEnumerable<AbstractPropertyHandler> Properties
        {
            get
            {
                foreach (AbstractPropertyHandler prop in AllProperties)
                {
                    if (!prop.Ignored)
                        yield return prop;

                }
            }
        }
        /// <summary>
        /// Get the list of properties for this type
        /// </summary>
        public virtual IEnumerable<AbstractPropertyHandler> AllProperties
        {
            get {
                LoadProperties();
                return _properties; 
            }
        }

        /// <summary>
        /// Finds a property by its name.  The property must follow the same rules as
        /// those returned from the Properties list, i.e. must be readable and writable and
        /// not have an ignore attribute.
        /// </summary>
        /// <param name="Name">the name of the property</param>
        /// <returns>TypeHandlerProperty instance for the property or null if not found</returns>
        public AbstractPropertyHandler FindProperty(string Name)
        {
            foreach (AbstractPropertyHandler prop in Properties)
            {
                if (prop.Name == Name)
                    return prop;
            }
            foreach (AbstractPropertyHandler prop in ConstructorParameters)
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
            if (_properties == null)
            {
                _tempIgnore[name] = true;
            }
            else
            {
                AbstractPropertyHandler handler = FindProperty(name);
                _properties.Remove(handler);
            }
        }

        /// <summary>
        /// Returns true if this type is a collection type
        /// </summary>
        /// <param name="context">the serialization context</param>
        /// <returns>true if a collection</returns>
        public virtual bool IsCollection()
        {
            if (!_collectionLookedUp)
            {
                foreach (CollectionHandler handler in _context.CollectionHandlers)
                {
                    if (handler.IsCollection(ForType))
                    {
                        _collectionHandler = handler;
                        break;
                    }
                }
                _collectionLookedUp = true;
            }
            return _collectionHandler != null;
        }

        /// <summary>
        /// Returns a collection handler if this object is a collection
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual CollectionHandler GetCollectionHandler()
        {
            if (IsCollection()) {
                return _collectionHandler;
            } else {
                throw new InvalidOperationException("Type " + ForType + " is not recognized as a collection.  A collection handler (ICollectionHandler) may be necessary");
            }            
        }

        protected override IJsonTypeConverter CreateTypeConverter()
        {
            IJsonTypeConverter converter = CreateTypeConverter(ForType);
            if (converter == null)
                return TypeConverterAdapter.GetAdapter(ForType);
            else
                return converter;
        }

        public override IJsonTypeConverter TypeConverter
        {
            get
            {
                if (ForType.IsPrimitive || ForType == typeof(string))
                {
                    _converterCreated = true;
                    return null;
                }
                else
                    return base.TypeConverter;
            }
            set
            {
                base.TypeConverter = value;
            }
        }
    }

}