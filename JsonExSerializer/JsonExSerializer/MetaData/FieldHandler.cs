using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Handles a public field on an object
    /// </summary>
    public class FieldHandler : PropertyHandlerBase, IPropertyHandler
    {
        /// <summary>
        /// Constructs a FieldHandler for a field on a type that is not a constructor
        /// parameter
        /// </summary>
        /// <param name="field">field info</param>
        public FieldHandler(FieldInfo field)
            : base(field)
        {
        }

        /// <summary>
        /// Constructs a FieldHandler for a field on a type that is also a constructor
        /// parameter at the specified position in the constructor arguments.
        /// </summary>
        /// <param name="field">field info</param>
        /// <param name="position">its position in the constructor</param>
        public FieldHandler(FieldInfo field, int position)
            : base(field, position)
        {
        }

        /// <summary>
        /// The fieldInfo object for this FieldHandler
        /// </summary>
        private FieldInfo Field
        {
            get { return (FieldInfo)_member; }
        }

        /// <summary>
        /// Gets the type of the field
        /// </summary>
        public Type PropertyType
        {
            get { return Field.FieldType; }
        }

        /// <summary>
        /// Gets the value of the field from an object instance
        /// </summary>
        /// <param name="instance">the object instance to retrieve the field value from</param>
        /// <returns>field value</returns>
        public object GetValue(object instance)
        {
            return Field.GetValue(instance);
        }

        /// <summary>
        /// Sets the value of the field on the object
        /// </summary>
        /// <param name="instance">the object instance to retrieve the field value from</param>
        /// <param name="value">field value</param>
        public void SetValue(object instance, object value)
        {
            Field.SetValue(instance, value);
        }
    }
}
