using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;
using System.Reflection;
using System.ComponentModel;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Base class for <see cref="TypeData" /> and <see cref="PropertyData"/>
    /// </summary>
    public abstract class MetaDataBase : IMetaData
    {
        protected DefaultValueOption defaultValueSetting;

        /// <summary>
        /// The declaring type for this member
        /// </summary>
        protected Type forType;

        /// <summary>
        /// The converter for this MetaData if applicable
        /// </summary>
        protected IJsonTypeConverter converterInstance;

        /// <summary>
        /// Flag to indicate whether an attempt to create the converter has occurred.  If
        /// the converterInstance is null and converterCreated is true then this instance does
        /// not have a converter.
        /// </summary>
        protected bool converterCreated;

        /// <summary>
        /// Initialize an instance for a specific type
        /// </summary>
        /// <param name="forType">the containing type</param>
        protected MetaDataBase(Type forType)
        {
            this.forType = forType;
        }

        /// <summary>
        /// The declaring type for this member
        /// </summary>
        public Type ForType
        {
            get { return this.forType; }
        }

        /// <summary>
        /// Gets or sets the TypeConverter defined for this object
        /// </summary>
        public virtual IJsonTypeConverter TypeConverter
        {
            get
            {
                if (converterInstance == null && !converterCreated)
                {
                    converterInstance = CreateTypeConverter();
                    converterCreated = true;
                }
                return converterInstance;
            }
            set { converterInstance = value; }
        }

        /// <summary>
        /// Returns true if there is a TypeConverter defined for this object
        /// </summary>
        public virtual bool HasConverter
        {
            get { return TypeConverter != null; }
        }

        /// <summary>
        /// Constructs the type converter instance for this instance, if applicable
        /// </summary>
        /// <returns>constructed type converter, or null if no converter defined</returns>
        protected virtual IJsonTypeConverter CreateTypeConverter()
        {
            return null;
        }

        public virtual DefaultValueOption DefaultValueSetting
        {
            get
            {
                return this.defaultValueSetting;
            }
            set
            {
                this.defaultValueSetting = value;
            }
        }

        public virtual DefaultValueOption GetEffectiveDefaultValueSetting()
        {
            return this.DefaultValueSetting;
        }
    }
}
