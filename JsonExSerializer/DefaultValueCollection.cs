using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace JsonExSerializer
{
    /// <summary>
    /// DefaultValueCollection holds mappings from Types to their default values.  If no default value for
    /// a Type has been set the .NET framework default type will be returned.
    /// </summary>
    public class DefaultValueCollection
    {
        private Dictionary<Type, object> _defaultValues = new Dictionary<Type, object>();
        private DefaultValueCollection parent;

        /// <summary>
        /// Constructs a DefaultValueCollection with no parent collection
        /// </summary>
        public DefaultValueCollection()
        {
        }

        /// <summary>
        /// Constructs a DefaultValueCollection with a parent collection
        /// </summary>
        public DefaultValueCollection(DefaultValueCollection parent)
        {
            this.parent = parent;
        }

        public object this[Type forType]
        {
            get { return GetDefaultValue(forType); }
            set { SetDefaultValue(forType, value); }
        }

        protected virtual void SetDefaultValue(Type forType, object value)
        {
            if (value != null && value.GetType() != forType)
            {
                try
                {
                    object convertedValue = Convert.ChangeType(value, forType, CultureInfo.InvariantCulture);
                    value = convertedValue;
                }
                catch
                {
                }
            }
            _defaultValues[forType] = value;
        }

        protected virtual object GetDefaultValue(Type forType)
        {
            object defaultValue = null;
            if (!_defaultValues.TryGetValue(forType, out defaultValue))
            {
                if (parent != null)
                    return parent[forType];
                else
                {
                    if (forType.IsClass)
                        return null;
                    else
                    {
                        defaultValue = GetFrameworkDefaultType(forType);
                        _defaultValues[forType] = defaultValue;
                    }
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
