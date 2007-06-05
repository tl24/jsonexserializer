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
using JsonExSerializer.CollectionBuilder;

namespace JsonExSerializer
{
    public class TypeHandler
    {
        private static IDictionary<Type, TypeHandler> _cache;
        private Type _handledType;
        private IList<TypeHandlerProperty> _properties;

        static TypeHandler()
        {
            _cache = new Dictionary<Type, TypeHandler>();
        }

        public static TypeHandler GetHandler(Type t)
        {
            TypeHandler handler;
            if (!_cache.ContainsKey(t))
            {
                _cache[t] = handler = new TypeHandler(t);
            }
            else
            {
                handler = _cache[t];
            }
            return handler;
        }

        private TypeHandler(Type t)
        {
            _handledType = t;
            LoadProperties();
        }

        private void LoadProperties()
        {
            if (_properties == null)
            {
                _properties = new List<TypeHandlerProperty>();
                PropertyInfo[] pInfos = _handledType.GetProperties();
                foreach (PropertyInfo pInfo in pInfos)
                {
                    // must be able to read and write the prop, otherwise its not 2-way 
                    if (pInfo.CanRead && pInfo.CanWrite)
                    {
                        // ignore attribute
                        if (!pInfo.IsDefined(typeof(JsonExIgnoreAttribute), false))
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

        /// <summary>
        /// If the object is a collection or array gets the type 
        /// of its elements.
        /// </summary>
        /// <returns></returns>
        public Type GetElementType()
        {
            // This is very crude, but it will work for now
            if (_handledType.HasElementType)
            {
                return _handledType.GetElementType();
            }
            else if (_handledType.IsGenericType && typeof(IDictionary).IsAssignableFrom(_handledType))
            {
                // get the type of the 
                return _handledType.GetGenericArguments()[1];
            }
            else if (_handledType.IsGenericType && typeof(ICollection).IsAssignableFrom(_handledType))
            {
                // get the type of the collection
                return _handledType.GetGenericArguments()[0];
            }
            else
            {
                return typeof(object);
            }
        }

        public ICollectionBuilder GetCollectionBuilder()
        {
            //TODO: this is ugly code, there has to be a better way
            if (_handledType.IsArray || typeof(IList).IsAssignableFrom(_handledType))
            {
                return new ListCollectionBuilder(_handledType);
            } else if (typeof(ICollection).IsAssignableFrom(_handledType)) {
                // make a strongly typed IEnumerable interface to compare against
                Type ienumGeneric = typeof(IEnumerable<object>).GetGenericTypeDefinition().MakeGenericType(new Type[] { this.GetElementType() });
                if (_handledType.IsGenericType && ienumGeneric.IsAssignableFrom(_handledType))
                {
                    Type cbType = typeof(NonStandardGenericCollectionBuilder<object>).GetGenericTypeDefinition();
                    cbType = cbType.MakeGenericType(new Type[] { this.GetElementType() });
                    return (ICollectionBuilder)Activator.CreateInstance(cbType, new object[] { _handledType });
                }
                else
                {
                    return new NonStandardCollectionBuilder(_handledType);
                }
            }
            else
            {
                throw new Exception("GetCollectionBuilder called on an object that is not a collection");
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
