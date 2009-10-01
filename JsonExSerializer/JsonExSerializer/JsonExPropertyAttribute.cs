using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    /// <summary>
    /// Force serialization of a property that would otherwise be ignored, or change attributes about the property 
    /// such as it's alias
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class JsonExPropertyAttribute : Attribute
    {
        private string _alias;

        /// <summary>
        /// Forces the property to be serialized if it would otherwise be ignored or suppressed
        /// </summary>
        public JsonExPropertyAttribute()
        {
        }


        /// <summary>
        /// Changes the alias of the property to be used in serialization.
        /// </summary>
        public JsonExPropertyAttribute(string alias)
        {
            _alias = alias;
        }

        /// <summary>
        /// Changes the alias of the property to be used in serialization.
        /// </summary>
        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }
    }

}
