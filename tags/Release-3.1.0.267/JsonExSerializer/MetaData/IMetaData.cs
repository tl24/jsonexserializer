using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer.MetaData
{
    public interface IMetaData
    {
        /// <summary>
        /// The declaring type for this member
        /// </summary>
        Type ForType { get; }

        /// <summary>
        /// Gets or sets the TypeConverter defined for this object
        /// </summary>
        IJsonTypeConverter TypeConverter { get; set; }

        /// <summary>
        /// Returns true if there is a TypeConverter defined for this object
        /// </summary>
        bool HasConverter { get; }

        /// <summary>
        /// Options for using default values
        /// </summary>
        DefaultValueOption DefaultValueSetting { get; set; }

        /// <summary>
        /// Gets the effective setting of the DefaultValueSetting of this object combined
        /// with its parent.
        /// </summary>
        /// <returns>effective default value setting</returns>
        DefaultValueOption GetEffectiveDefaultValueSetting();
    }
}
