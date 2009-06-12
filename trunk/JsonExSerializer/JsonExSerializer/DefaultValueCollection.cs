using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace JsonExSerializer
{
    public class DefaultValueCollection
    {
        private Dictionary<Type, object> _defaultValues = new Dictionary<Type, object>();


        public object this[Type forType]
        {
            get { return GetDefaultValue(forType); }
            set { SetDefaultValue(forType, value); }
        }

        protected virtual void SetDefaultValue(Type forType, object value)
        {
            _defaultValues[forType] = value;
        }

        protected virtual object GetDefaultValue(Type forType)
        {
            object defaultValue = null;
            if (!_defaultValues.TryGetValue(forType, out defaultValue))
            {
                if (forType.IsClass)
                    return null;
                else
                {
                    defaultValue = GetFrameworkDefaultType(forType);
                    _defaultValues[forType] = defaultValue;
                }
            }
            return defaultValue;
        }

        protected static object GetFrameworkDefaultType(Type forType)
        {
            MethodInfo m = (MethodInfo)typeof(DefaultValueCollection).GetMethod("GenericGetFrameworkDefaultType",BindingFlags.NonPublic|BindingFlags.Static);
            MethodInfo genMeth = m.MakeGenericMethod(forType);
            return genMeth.Invoke(null, null);
        }

        private static object GenericGetFrameworkDefaultType<T>()
        {
            return default(T);
        }
    }
}
