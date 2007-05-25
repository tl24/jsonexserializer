using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

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
            _properties = new List<TypeHandlerProperty>();
            LoadProperties();
        }

        private void LoadProperties()
        {
            PropertyInfo[] pInfos = _handledType.GetProperties();
            foreach(PropertyInfo pInfo in pInfos) {
                _properties.Add(new TypeHandlerProperty(pInfo));
            }
        }

        public IList<TypeHandlerProperty> Properties
        {
            get { return _properties; }
        }

        public TypeHandlerProperty FindProperty(string Name)
        {
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
