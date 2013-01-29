/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    /// <summary>
    /// This attribute is used to decorate a property that will be used as an argument to the
    /// constructor rather than written out as a normal property.
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Property|AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class ConstructorParameterAttribute : Attribute
    {
        private readonly string name;

        /// <summary>
        /// Indicates that this property will be passed to the constructor with a constructor argument with the same name as
        /// the property
        /// </summary>
        public ConstructorParameterAttribute()
        {
        }

        /// <summary>
        /// Indicates that this property will be passed to the constructor with a constructor argument with the 
        /// specified <param name="name" />
        /// </summary>
        /// <param name="name">The name of the constructor argument</param>
        public ConstructorParameterAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }
    }

}
