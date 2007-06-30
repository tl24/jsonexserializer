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

namespace JsonExSerializer
{
    /// <summary>
    /// Helper class for dealing with types during serialization
    /// </summary>
    class TypeHandler
    {
        private static IDictionary<Type, TypeHandler> _cache;
        private Type _handledType;
        private IList<PropertyHandler> _properties;
        private bool _collectionLookedUp = false;
        private ICollectionHandler _collectionHandler;
        private SerializationContext _context;
        private IDictionary<string, bool> _tempIgnore;

        /// <summary>
        /// Get an instance of a type handler for a given type
        /// </summary>
        /// <param name="t">the type</param>
        /// <returns>a type handler for the given type</returns>
        internal static TypeHandler GetHandler(Type t, SerializationContext context)
        {
            return new TypeHandler(t, context);
        }

        /// <summary>
        /// internal constructor
        /// </summary>
        /// <param name="t"></param>
        internal TypeHandler(Type t, SerializationContext context)
        {
            _handledType = t;
            _context = context;
            _tempIgnore = new Dictionary<string, bool>();
        }

        /// <summary>
        /// Loads the properties for the type if they haven't already been loaded
        /// </summary>
        private void LoadProperties()
        {
            if (_properties == null)
            {
                _properties = new List<PropertyHandler>();
                PropertyInfo[] pInfos = _handledType.GetProperties(BindingFlags.Public|BindingFlags.Instance);
                foreach (PropertyInfo pInfo in pInfos)
                {
                    // must be able to read and write the prop, otherwise its not 2-way 
                    if (pInfo.CanRead && pInfo.CanWrite)
                    {
                        
                        // ignore attribute
                        if (!pInfo.IsDefined(typeof(JsonExIgnoreAttribute), false)
                            && pInfo.GetGetMethod().GetParameters().Length == 0
                            && !_tempIgnore.ContainsKey(pInfo.Name))
                        {
                            _properties.Add(new PropertyHandler(pInfo));
                        }
                    }
                }
                _tempIgnore.Clear();
            }
        }

        /// <summary>
        /// Get the list of properties for this type
        /// </summary>
        public IList<PropertyHandler> Properties
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
        public PropertyHandler FindProperty(string Name)
        {
            LoadProperties();
            foreach (PropertyHandler prop in _properties)
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
        public void IgnoreProperty(string name)
        {
            if (_properties == null)
            {
                _tempIgnore[name] = true;
            }
            else
            {
                PropertyHandler handler = FindProperty(name);
                _properties.Remove(handler);
            }
        }

        /// <summary>
        /// Ignore a property to keep from being serialized, same as if the JsonExIgnore attribute had been set
        /// </summary>
        /// <param name="name">the name of the property</param>
        public void IgnoreProperty(PropertyInfo property)
        {
            IgnoreProperty(property.Name);
        }

        /// <summary>
        /// The type of object that this typehandler represents
        /// </summary>
        public Type ForType
        {
            get { return _handledType; }
        }

        /// <summary>
        /// Returns true if this type is a collection type
        /// </summary>
        /// <param name="context">the serialization context</param>
        /// <returns>true if a collection</returns>
        public bool IsCollection()
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
        public ICollectionHandler GetCollectionHandler()
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
        public Type GetCollectionItemType()
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
        public ICollectionBuilder GetCollectionBuilder()
        {
            if (IsCollection())
            {
                return _collectionHandler.ConstructBuilder(ForType);
            }
            else
            {
                throw new CollectionException("Type " + ForType + " is not recognized as a collection.  A collection handler (ICollectionHandler) may be necessary");
            }
        }       
    }

}
