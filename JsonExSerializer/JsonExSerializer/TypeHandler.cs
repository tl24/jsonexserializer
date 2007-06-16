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
    public class TypeHandler
    {
        private static IDictionary<Type, TypeHandler> _cache;
        private Type _handledType;
        private IList<TypeHandlerProperty> _properties;
        private bool _collectionLookedUp = false;
        private ICollectionHandler _collectionHandler;        

        internal static TypeHandler GetHandler(Type t)
        {
            return new TypeHandler(t);
        }

        internal TypeHandler(Type t)
        {
            _handledType = t;
        }

        private void LoadProperties()
        {
            if (_properties == null)
            {
                _properties = new List<TypeHandlerProperty>();
                PropertyInfo[] pInfos = _handledType.GetProperties(BindingFlags.Public|BindingFlags.Instance);
                foreach (PropertyInfo pInfo in pInfos)
                {
                    // must be able to read and write the prop, otherwise its not 2-way 
                    if (pInfo.CanRead && pInfo.CanWrite)
                    {
                        
                        // ignore attribute
                        if (!pInfo.IsDefined(typeof(JsonExIgnoreAttribute), false)
                            && pInfo.GetGetMethod().GetParameters().Length == 0)
                        {
                            _properties.Add(new TypeHandlerProperty(pInfo));
                        }
                    }
                }
            }
        }

        public IList<TypeHandlerProperty> Properties
        {
            get {
                LoadProperties();
                return _properties; 
            }
        }

        public TypeHandlerProperty FindProperty(string Name)
        {
            LoadProperties();
            foreach (TypeHandlerProperty prop in _properties)
            {
                if (prop.Name == Name)
                    return prop;
            }
            return null;
        }

        public Type ForType
        {
            get { return _handledType; }
        }

        public bool IsCollection(SerializationContext context)
        {
            if (!_collectionLookedUp)
            {
                foreach (ICollectionHandler handler in context.CollectionHandlers)
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

        public ICollectionHandler GetCollectionHandler(SerializationContext context)
        {
            if (IsCollection(context)) {
                return _collectionHandler;
            } else {
                throw new ApplicationException("Type " + ForType + " is not recognized as a collection.  A collection handler (ICollectionHandler) may be necessary");
            }            
        }

        /// <summary>
        /// If the object is a collection or array gets the type 
        /// of its elements.
        /// </summary>
        /// <returns></returns>
        public Type GetCollectionItemType(SerializationContext context)
        {
            if (IsCollection(context))
            {
                return _collectionHandler.GetItemType(ForType);
            }
            else
            {
                throw new ApplicationException("Type " + ForType + " is not recognized as a collection.  A collection handler (ICollectionHandler) may be necessary");
            }
        }

        public ICollectionBuilder GetCollectionBuilder(SerializationContext context)
        {
            if (IsCollection(context))
            {
                return _collectionHandler.ConstructBuilder(ForType);
            }
            else
            {
                throw new ApplicationException("Type " + ForType + " is not recognized as a collection.  A collection handler (ICollectionHandler) may be necessary");
            }
        }       
    }

    public class TypeHandlerProperty
    {
        private PropertyInfo _property;

        public TypeHandlerProperty(PropertyInfo property)
        {
            _property = property;
        }

        public Type PropertyType
        {
            get { return _property.PropertyType; }            
        }

        public string Name
        {
            get { return _property.Name; }
        }

        public object GetValue(object instance)
        {
            return _property.GetValue(instance, null);
        }

        public void SetValue(object instance, object value)
        {
            _property.SetValue(instance, value, null);
        }
    }
}
