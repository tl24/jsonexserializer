using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    /// <summary>
    /// This attribute can be applied to an assembly or class and allows you to specify which default values to use
    /// for specific types or to disable default value processing for the type or assembly.
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class JsonExDefaultValuesAttribute : System.Attribute
    {
        private string typeName;
        private Type type;
        private object defaultValue;
        private DefaultValueOption defaultValueSetting;

        /// <summary>
        /// Turns default processing on for this item
        /// </summary>
        public JsonExDefaultValuesAttribute()
        {
            this.defaultValueSetting = DefaultValueOption.SuppressDefaultValues;
        }

        /// <summary>
        /// Specifies a default value for a type
        /// </summary>
        /// <param name="type">the type that the default value refers to</param>
        /// <param name="defaultValue">the default value for the type</param>
        public JsonExDefaultValuesAttribute(Type type, object defaultValue)
        {
            this.type = type;
            this.defaultValue = defaultValue;
        }

        public JsonExDefaultValuesAttribute(string typeName, object defaultValue)
        {
            this.typeName = typeName;
            this.defaultValue = defaultValue;
        }

        public JsonExDefaultValuesAttribute(DefaultValueOption defaultValueSetting)
        {
            this.defaultValueSetting = defaultValueSetting;
        }

        public System.Type Type
        {
            get
            {
                if (this.type != null)
                    return this.type;
                else if (!string.IsNullOrEmpty(this.typeName))
                    return Type.GetType(this.typeName);
                else
                    return null;
            }
        }

        public object DefaultValue
        {
            get { return this.defaultValue; }
        }

        public JsonExSerializer.DefaultValueOption DefaultValueSetting
        {
            get { return this.defaultValueSetting; }
        }


    }
}
