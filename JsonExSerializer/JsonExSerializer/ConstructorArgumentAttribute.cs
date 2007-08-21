using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
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
