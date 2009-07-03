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
    /// Implementation of IPropertyData that uses a <see cref="System.Reflection.PropertyInfo"/> instance
    /// as the underlying property type.
    /// </summary>
    public class PropertyData : MemberInfoPropertyDataBase
    {
        /// <summary>
        /// Initializes an instance of PropertyData with the specified PropertyInfo object that is
        /// not a constructor argument.
        /// </summary>
        /// <param name="property">the backing property object</param>
        public PropertyData(PropertyInfo property, TypeData parent) : base(property, parent)
        {
            Initialize();
        }


        /// <summary>
        /// Initializes the object
        /// </summary>
        private void Initialize()
        {
            if (!(Property.CanRead && Property.GetGetMethod().GetParameters().Length == 0)
                || !Property.CanWrite || !this.PublicWriter || !this.PublicGetter)
            {
                this.Ignored = true;
            }
            if (IsConstructorArgument)
                this.Ignored = false;
        }

        /// <summary>
        /// The backing property object
        /// </summary>
        protected PropertyInfo Property
        {
            get { return (PropertyInfo)member; }
        }

        /// <summary>
        /// The type for the property
        /// </summary>
        public override Type PropertyType
        {
            get { return Property.PropertyType; }
        }

        /// <summary>
        /// Get the value of the property from the given object
        /// </summary>
        /// <param name="instance">the object to retrieve this property value from</param>
        /// <returns>property value</returns>
        public override object GetValue(object instance)
        {
            return Property.GetValue(instance, null);
        }

        /// <summary>
        /// Sets the value of the property for the object
        /// </summary>
        /// <param name="instance">the object instance to set the property value on</param>
        /// <param name="value">the new value to set</param>
        public override void SetValue(object instance, object value)
        {
            Property.SetValue(instance, value, null);
        }

        /// <summary>
        /// Gets a value indicating whether this property can be written to
        /// </summary>
        public override bool CanWrite
        {
            get { return Property.CanWrite; }
        }

        public bool PublicWriter
        {
            get { return Property.CanWrite && Property.GetSetMethod(false) != null; }
        }

        public bool PublicGetter
        {
            get { return Property.CanRead && Property.GetGetMethod(false) != null; }
        }
    }
}
