using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonExSerializer
{
    public enum MissingPropertyOptions
    {
        Ignore,
        ThrowException
    }

    public enum ReferenceOption
    {
        WriteIdentifier,
        IgnoreCircularReferences,
        ErrorCircularReferences
    }

    /// <summary>
    /// Set of options for handling Ignored properties encountered upon Deserialization
    /// </summary>
    public enum IgnoredPropertyOption
    {
        Ignore,
        SetIfPossible,
        ThrowException
    }

    public enum DefaultValueOption
    {
        /// <summary>
        /// The default value setting, which is usually inherited from the parent
        /// </summary>
        InheritParentSetting = 0,

        /// <summary>
        /// Default values are suppress for this item
        /// </summary>
        SuppressDefaultValues = 1,

        /// <summary>
        /// All values are written for this item, default values are not suppressed
        /// </summary>
        WriteAllValues = 2
    }

}
