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
    public class TypeHandler : MemberHandlerBase, ITypeHandler
    {
        protected IList<IPropertyHandler> _properties;
        protected IList<IPropertyHandler> _constructorArgs;

        private bool _collectionLookedUp = false;
        private ICollectionHandler _collectionHandler;
        protected SerializationContext _context;
        private IDictionary<string, bool> _tempIgnore;

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
                    ((List<IPropertyHandler>)_constructorArgs).Sort(
                        new Comparison<IPropertyHandler>(PropertyHandlerComparison));
                }
                _tempIgnore.Clear();
            }
        }

        /// <summary>
        /// Reads the properties and constructor arguments from type metadata
        /// </summary>
        /// <param name="Properties">properties collection</param>
        /// <param name="ConstructorArguments">constructor arguments</param>
        protected virtual void ReadProperties(out IList<IPropertyHandler> Properties, out IList<IPropertyHandler> ConstructorArguments)
        {
            Properties = new List<IPropertyHandler>();
            ConstructorArguments = new List<IPropertyHandler>();

            PropertyInfo[] pInfos = ForType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            MemberInfo[] mInfos = ForType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
            foreach (MemberInfo mInfo in mInfos)
            {
                PropertyInfo pInfo = mInfo as PropertyInfo;
                FieldInfo fInfo = mInfo as FieldInfo;
                // must be able to read and write the prop, otherwise its not 2-way 
                if (pInfo != null && pInfo.CanRead)
                {
                    if (pInfo.IsDefined(typeof(ConstructorParameterAttribute), false))
                    {
                        ConstructorParameterAttribute ctorAttr = (ConstructorParameterAttribute)pInfo.GetCustomAttributes(typeof(ConstructorParameterAttribute), false)[0];

                        ConstructorArguments.Add(new PropertyHandler(pInfo, ctorAttr.Position));
                    }
                    else if (!pInfo.IsDefined(typeof(JsonExIgnoreAttribute), false)
                        && pInfo.GetGetMethod().GetParameters().Length == 0
                        && !_tempIgnore.ContainsKey(pInfo.Name)
                        && pInfo.CanWrite)
                    {
                        Properties.Add(new PropertyHandler(pInfo));
                    }
                }
                else if (fInfo != null)
                {
                    if (fInfo.IsDefined(typeof(ConstructorParameterAttribute), false))
                    {
                        ConstructorParameterAttribute ctorAttr = (ConstructorParameterAttribute)fInfo.GetCustomAttributes(typeof(ConstructorParameterAttribute), false)[0];

                        ConstructorArguments.Add(new FieldHandler(fInfo, ctorAttr.Position));
                    }
                    else if (!fInfo.IsDefined(typeof(JsonExIgnoreAttribute), false)
                        && !_tempIgnore.ContainsKey(fInfo.Name))
                    {
                        Properties.Add(new FieldHandler(fInfo));
                    }
                }
            }
        }
        protected int PropertyHandlerComparison(IPropertyHandler a, IPropertyHandler b) {
            return a.Position - b.Position;
        }

        public virtual object CreateInstance(object[] args)
        {
            return Activator.CreateInstance(this.ForType, args);
        }

        /// <summary>
        /// Get the list of constructor parameters for this type
        /// </summary>
        public virtual IList<IPropertyHandler> ConstructorParameters
        {
            get
            {
                LoadProperties();
                return _constructorArgs;
            }
        }

        /// <summary>
        /// Get the list of properties for this type
        /// </summary>
        public virtual IList<IPropertyHandler> Properties
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
        public IPropertyHandler FindProperty(string Name)
        {
            foreach (IPropertyHandler prop in Properties)
            {
                if (prop.Name == Name)
                    return prop;
            }
            foreach (IPropertyHandler prop in ConstructorParameters)
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
                IPropertyHandler handler = FindProperty(name);
                _properties.Remove(handler);
            }
        }

        /// <summary>
        /// Ignore a property to keep from being serialized, same as if the JsonExIgnore attribute had been set
        /// </summary>
        /// <param name="name">the name of the property</param>
        public virtual void IgnoreProperty(PropertyInfo property)
        {
            IgnoreProperty(property.Name);
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
                foreach (ICollectionHandler handler in _context.CollectionHandlers)
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
        public virtual ICollectionHandler GetCollectionHandler()
        {
            if (IsCollection()) {
                return _collectionHandler;
            } else {
                throw new CollectionException("Type " + ForType + " is not recognized as a collection.  A collection handler (ICollectionHandler) may be necessary");
            }            
        }

        /// <summary>
        /// If the object is a collection or array gets the type 
        /// of its elements.
        /// </summary>
        /// <returns></returns>
        public virtual Type GetCollectionItemType()
        {
            if (IsCollection())
            {
                return _collectionHandler.GetItemType(ForType);
            }
            else
            {
                throw new CollectionException("Type " + ForType + " is not recognized as a collection.  A collection handler (ICollectionHandler) may be necessary");
            }
        }

        /// <summary>
        /// Returns a collection builder object for this type if it is a collection.
        /// </summary>
        /// <param name="context">the serialization context</param>
        /// <returns>collection builder</returns>
        public virtual ICollectionBuilder GetCollectionBuilder(int itemCount)
        {
            if (IsCollection())
            {
                return _collectionHandler.ConstructBuilder(ForType, itemCount);
            }
            else
            {
                throw new CollectionException("Type " + ForType + " is not recognized as a collection.  A collection handler (ICollectionHandler) may be necessary");
            }
        }

        protected override IJsonTypeConverter CreateTypeConverter()
        {
            IJsonTypeConverter converter = CreateTypeConverter(ForType);
            if (converter == null)
                return GetDefaultTypeConverter(ForType);
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
