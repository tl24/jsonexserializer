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
        private bool _ignored;
        /// <summary>
        /// Constructs a FieldHandler for a field on a type that is not a constructor
        /// parameter
        /// </summary>
        /// <param name="field">field info</param>
        public FieldHandler(FieldInfo field)
            : base(field)
        {
            Initialize();
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
            Initialize();
            _position = position;
        }

        private void Initialize()
        {
            if (Field.IsDefined(typeof(ConstructorParameterAttribute), false))
            {
                ConstructorParameterAttribute ctorAttr = (ConstructorParameterAttribute)Field.GetCustomAttributes(typeof(ConstructorParameterAttribute), false)[0];
                _position = ctorAttr.Position;
            }
            if (Field.IsDefined(typeof(JsonExIgnoreAttribute), false))
                _ignored = true;

            if (Field.IsDefined(typeof(JsonPropertyAttribute), false))
                _ignored = false;
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

        /// <summary>
        /// Gets or sets the value indicating whether this property is ignored or not.  If the property
        /// is ignored it will not be serialized.
        /// </summary>
        public virtual bool Ignored
        {
            get { return _ignored; }
            set { _ignored = value; }
        }
    }
}
