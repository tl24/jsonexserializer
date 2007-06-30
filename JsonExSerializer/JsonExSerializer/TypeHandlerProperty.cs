using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace JsonExSerializer
{
    /// <summary>
    /// Helper for a type's properties
    /// </summary>
    class TypeHandlerProperty
    {
        private PropertyInfo _property;

        internal TypeHandlerProperty(PropertyInfo property)
        {
            _property = property;
        }

        /// <summary>
        /// The type for the property
        /// </summary>
        public Type PropertyType
        {
            get { return _property.PropertyType; }
        }

        /// <summary>
        ///  The name of the property
        /// </summary>
        public string Name
        {
            get { return _property.Name; }
        }

        /// <summary>
        /// Get the value of the property from the given object
        /// </summary>
        /// <param name="instance">the object to retrieve this property value from</param>
        /// <returns>property value</returns>
        public object GetValue(object instance)
        {
            return _property.GetValue(instance, null);
        }

        /// <summary>
        /// Sets the value of the property for the object
        /// </summary>
        /// <param name="instance">the object instance to set the property value on</param>
        /// <param name="value">the new value to set</param>
        public void SetValue(object instance, object value)
        {
            _property.SetValue(instance, value, null);
        }
    }
}
