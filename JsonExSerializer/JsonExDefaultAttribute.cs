using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    /// <summary>
    /// This attribute can be applied to a property or field to suppress or enable default values being serialized.
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class JsonExDefaultAttribute : System.Attribute
    {
        private object defaultValue;
        private bool defaultValueSet;
        private DefaultValueOption defaultValueSetting = DefaultValueOption.SuppressDefaultValues;

        public JsonExDefaultAttribute()
        {
        }

        /// <summary>
        /// Apply default processing with the specified default value
        /// </summary>
        /// <param name="defaultValue">default value to set</param>
        public JsonExDefaultAttribute(object defaultValue)
        {
            this.defaultValue = defaultValue;
            this.defaultValueSet = true;
        }

        /// <summary>
        /// Gets or sets the default value for this item.  You should check the DefaultValueSet flag to see if the
        /// default value is set first
        /// </summary>
        public object DefaultValue
        {
            get {
                if (!DefaultValueSet)
                    throw new InvalidOperationException("DefaultValue has not been set");
                return this.defaultValue; 
            }
            set
            {
                this.defaultValue = value;
                this.defaultValueSet = true;
            }
        }

        /// <summary>
        /// Flag indicating whether the default value has been set. The DefaultValue property should
        /// not be read unless this returns true.
        /// </summary>
        public bool DefaultValueSet
        {
            get { return this.defaultValueSet; }
        }

        /// <summary>
        /// Get or set a value indicating default value processing option.  By default it is set to SuppressDefaultValues.
        /// </summary>
        public DefaultValueOption DefaultValueSetting
        {
            get { return this.defaultValueSetting; }
            set { this.defaultValueSetting = value; }
        }
    }
}
