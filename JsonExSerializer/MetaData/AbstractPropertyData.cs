using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Base Implementation of an object property that does not require a member of a Type, such as a Property or Field,
    /// for implementation.
    /// </summary>
    public abstract class AbstractPropertyData : MetaDataBase, IPropertyData
    {
        /// <summary>
        /// Ignored flag
        /// </summary>
        protected bool ignored;

        /// <summary>
        /// Index in constructor arguments for a constructor parameter
        /// </summary>
        protected int position = -1;

        /// <summary>
        /// Initializes an instance for the specific declaring type
        /// </summary>
        /// <param name="forType">the declaring type for this property</param>
        protected AbstractPropertyData(Type forType)
            : base(forType)
        {
        }

        /// <summary>
        /// Gets the name of the property
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Returns the 0-based index in the constructor arguments for a constructor parameter
        /// </summary>
        public virtual int Position {
            get { return position; }
        }

        /// <summary>
        /// Gets the type of the property
        /// </summary>
        public abstract Type PropertyType { get; }

        /// <summary>
        /// Gets the value of the property from the specified object instance
        /// </summary>
        /// <param name="instance">the instance of an object for the declaring type of this property</param>
        /// <returns>the value of the property</returns>
        public abstract object GetValue(object instance);

        /// <summary>
        /// Sets the value of the property on the specified object instance
        /// </summary>
        /// <param name="instance">the instance of an object for the declaring type of this property</param>
        ///<param name="value">the value to set the property to</param>
        public abstract void SetValue(object instance, object value);

        /// <summary>
        /// Gets a value indicating whether this property is a Constructor argument.
        /// </summary>
        public virtual bool IsConstructorArgument
        {
            get { return this.Position != -1; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this property is ignored.  Ignored properties
        /// do not get written during serialization, and may not be read during deserialization depending
        /// on the <see cref="JsonExSerializer.SerializationContext.IgnoredPropertyOption" />
        /// </summary>
        public virtual bool Ignored
        {
            get { return this.ignored; }
            set { this.ignored = value; }
        }

        /// <summary>
        /// Returns true if this member can be written to
        /// </summary>
        public virtual bool CanWrite
        {
            get { return true; }
        }
    }
}
