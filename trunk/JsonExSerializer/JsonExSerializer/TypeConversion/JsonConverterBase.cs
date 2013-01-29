using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework;

namespace JsonExSerializer.TypeConversion
{
    public abstract class JsonConverterBase : IJsonTypeConverter
    {
        protected object _context;
       
        public abstract Type GetSerializedType(Type sourceType);

        public abstract object ConvertFrom(object item, ISerializerSettings serializationContext);

        public abstract object ConvertTo(object item, Type sourceType, ISerializerSettings serializationContext);

        /// <summary>
        /// Provides an optional parameter to the converter to control some of its functionality.   The Context
        /// is Converter-dependent.
        /// </summary>
        public virtual object Context
        {
            get { return _context; }
            set { _context = value; }
        }

        public virtual bool SupportsReferences(Type sourceType, ISerializerSettings serializationContext)
        {
            Type targetType = GetSerializedType(sourceType);
            return serializationContext.IsReferenceableType(targetType);
        }
    }
}
