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
        private bool? _empty;

        /// <summary>
        /// internal constructor
        /// </summary>
        /// <param name="t"></param>
        public TypeHandler(Type t, SerializationContext context) : base(t)
        {
            _context = context;
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
                    prop = CreatePropertyHandler((PropertyInfo) mInfo);
                }
                else if (mInfo is FieldInfo)
                {
                    prop = CreateFieldHandler((FieldInfo) mInfo);
                }
                if (prop != null) {
                    if (prop.IsConstructorArgument)
                        ConstructorArguments.Add(prop);
                    else
                        Properties.Add(prop);
                }
            }
        }

        /// <summary>
        /// Constructs a PropertyHandler instance from the PropertyInfo
        /// </summary>
        /// <param name="Property"></param>
        /// <returns></returns>
        protected virtual PropertyHandler CreatePropertyHandler(PropertyInfo Property)
        {
            return new PropertyHandler(Property);
        }

        /// <summary>
        /// Constructs a FieldHandler instance from the FieldInfo
        /// </summary>
        /// <param name="Field"></param>
        /// <returns></returns>
        protected virtual FieldHandler CreateFieldHandler(FieldInfo Field)
        {
            return new FieldHandler(Field);
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
            foreach (AbstractPropertyHandler prop in AllProperties)
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
            AbstractPropertyHandler handler = FindProperty(name);
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
            if (!_collectionLookedUp)
            {
                if (this.ForType.IsDefined(typeof(JsonExCollectionAttribute), true))
                    _collectionHandler = GetCollectionHandlerFromAttribute();
                else
                    _collectionHandler = FindCollectionHandler();
                _collectionLookedUp = true;
            }
            return _collectionHandler != null;
        }

        private CollectionHandler FindCollectionHandler()
        {
            foreach (CollectionHandler handler in _context.CollectionHandlers)
            {
                if (handler.IsCollection(ForType))
                {
                    return handler;
                }
            }
            return null;
        }

        /// <summary>
        /// Reads the JsonExCollection attribute for the class and loads the collection handler from that.  It first
        /// checks the list of collectionhandlers to see if its already been loaded.
        /// </summary>
        /// <returns>CollectionHandler specified by the JsonExCollection attribute</returns>
        protected virtual CollectionHandler GetCollectionHandlerFromAttribute() {
            JsonExCollectionAttribute attr = ((JsonExCollectionAttribute[])this.ForType.GetCustomAttributes(typeof(JsonExCollectionAttribute), true))[0];
            if (!attr.IsValid())
                throw new Exception("Invalid JsonExCollectionAttribute specified for " + this.ForType + ", either CollectionHandlerType or ItemType or both must be specified");

            Type collHandlerType = attr.GetCollectionHandlerType();
            Type itemType = attr.GetItemType();

            // Try exact type match first
            CollectionHandler handler = null;

            if (collHandlerType == null) {
                handler = FindCollectionHandler();
                handler = new CollectionHandlerWrapper(handler, this.ForType, itemType);
            }

            if (handler == null)
            {
                handler = _context.CollectionHandlers.Find(delegate(CollectionHandler h) { return h.GetType() == collHandlerType; });
                if (handler != null)
                    return handler;

                // try inherited type next
                handler = _context.CollectionHandlers.Find(delegate(CollectionHandler h) { return collHandlerType.IsInstanceOfType(h); });
                if (handler != null)
                    return handler;

                // create the handler
                handler = (CollectionHandler)Activator.CreateInstance(collHandlerType);
            }

            // install the handler
            _context.RegisterCollectionHandler(handler);
            return handler;
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
