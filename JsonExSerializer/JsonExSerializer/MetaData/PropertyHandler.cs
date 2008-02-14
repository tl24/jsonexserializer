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
    public class PropertyHandler : PropertyHandlerBase
    {
        public PropertyHandler(PropertyInfo property) : base(property)
        {
            Initialize();
        }

        public PropertyHandler(PropertyInfo property, int position)
            : base(property, position)
        {
            Initialize();
            _position = position;
        }

        private void Initialize()
        {
            if (Property.IsDefined(typeof(ConstructorParameterAttribute), false))
            {
                ConstructorParameterAttribute ctorAttr = (ConstructorParameterAttribute)Property.GetCustomAttributes(typeof(ConstructorParameterAttribute), false)[0];
                _position = ctorAttr.Position;
            }
            if (Property.IsDefined(typeof(JsonExIgnoreAttribute), false)
                || !(Property.GetGetMethod().GetParameters().Length == 0 && Property.CanRead)
                || (!Property.CanWrite && _position == -1))
            {
                _ignored = true;
            }
            if (Property.IsDefined(typeof(JsonPropertyAttribute), false))
                _ignored = false;

            Validate();
        }

        private PropertyInfo Property
        {
            get { return (PropertyInfo)_member; }
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

        public override bool Ignored
        {
            get { return base.Ignored; }
            set
            {
                if (base.Ignored != value)
                {
                    base.Ignored = value;
                    Validate();
                }
            }
        }

        private void Validate()
        {
            if (IsConstructorArgument)
                return;

            if (!Ignored && !CanWrite && PropertyType.IsPrimitive)
                throw new InvalidOperationException("Cannot serialize a primitive property without a public set method: " + ForType.FullName + ":" + Name);
            if (!Ignored && !Property.CanRead)
                throw new InvalidOperationException("Cannot serialize a property without a get method: " + ForType.FullName + ":" + Name);
        }

        public override bool CanWrite
        {
            get { return Property.CanWrite; }
        }
    }
}
