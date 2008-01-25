/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Helper for a type's properties
    /// </summary>
    public class PropertyHandler : PropertyHandlerBase, IPropertyHandler
    {
        public PropertyHandler(PropertyInfo property) : base(property)
        {
        }

        public PropertyHandler(PropertyInfo property, int position)
            : base(property, position)
        {
        }

        private PropertyInfo Property
        {
            get { return (PropertyInfo)_member; }
        }
        /// <summary>
        /// The type for the property
        /// </summary>
        public Type PropertyType
        {
            get { return Property.PropertyType; }
        }

        /// <summary>
        /// Get the value of the property from the given object
        /// </summary>
        /// <param name="instance">the object to retrieve this property value from</param>
        /// <returns>property value</returns>
        public virtual object GetValue(object instance)
        {
            return Property.GetValue(instance, null);
        }

        /// <summary>
        /// Sets the value of the property for the object
        /// </summary>
        /// <param name="instance">the object instance to set the property value on</param>
        /// <param name="value">the new value to set</param>
        public virtual void SetValue(object instance, object value)
        {
            Property.SetValue(instance, value, null);
        }

    }
}
