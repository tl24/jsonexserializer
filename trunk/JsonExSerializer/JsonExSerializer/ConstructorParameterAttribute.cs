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
    [global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ConstructorParameterAttribute : Attribute
    {
        readonly int _position;
        
        /// <summary>
        /// Indicates that this property will be passed to the constructor.  The position property
        /// specifies where the property occurs in the constructor's arguments, e.g. position=0 is the first
        /// constructor argument, position=1 is the second argument, etc.
        /// </summary>
        /// <param name="index">the position of the property within the constructor's arguments</param>
        public ConstructorParameterAttribute(int position)
        {
            this._position = position;
        }

        /// <summary>
        /// The constructor argument index
        /// </summary>
        public int Position
        {
            get
            {
                return this._position;
            }
        }
    }

}
