/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Helper for a type's properties
    /// </summary>
    public class PropertyHandler : IPropertyHandler
    {
        private PropertyInfo _property;
        private int _position = -1;

        public PropertyHandler(PropertyInfo property)
        {
            _property = property;
        }

        public PropertyHandler(PropertyInfo property, int position)
        {
            _property = property;
            _position = position;
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
        public virtual object GetValue(object instance)
        {
            return _property.GetValue(instance, null);
        }

        /// <summary>
        /// Sets the value of the property for the object
        /// </summary>
        /// <param name="instance">the object instance to set the property value on</param>
        /// <param name="value">the new value to set</param>
        public virtual void SetValue(object instance, object value)
        {
            _property.SetValue(instance, value, null);
        }

        /// <summary>
        /// The System.Reflection.PropertyInfo instance represented by this PropertyHandler
        /// </summary>
        public PropertyInfo Property
        {
            get { return this._property; }
        }

        public int Position
        {
            get { return this._position; }
        }
    }
}
