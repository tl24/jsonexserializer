using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;
using System.Reflection;
using System.ComponentModel;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Base class for TypeHandler and PropertyHandler
    /// </summary>
    public abstract class MemberHandlerBase
    {
        protected Type _forType;
        protected IJsonTypeConverter _converterInstance;
        protected bool _converterCreated;
        public MemberHandlerBase(Type ForType)
        {
            _forType = ForType;
        }

        public Type ForType
        {
            get { return this._forType; }
        }

        /// <summary>
        /// Gets or sets the TypeConverter defined for this object
        /// </summary>
        public virtual IJsonTypeConverter TypeConverter
        {
            get
            {
                if (_converterInstance == null && !_converterCreated)
                {
                    _converterInstance = CreateTypeConverter();
                    _converterCreated = true;
                }
                return _converterInstance;
            }
            set { _converterInstance = value; }
        }

        /// <summary>
        /// Returns true if there is a TypeConverter defined for this object
        /// </summary>
        public virtual bool HasConverter
        {
            get { return TypeConverter != null; }
        }

        protected abstract IJsonTypeConverter CreateTypeConverter();

        protected IJsonTypeConverter CreateTypeConverter(ICustomAttributeProvider provider)
        {
            if (provider.IsDefined(typeof(JsonConvertAttribute), false)) {
                JsonConvertAttribute convAttr = (JsonConvertAttribute)provider.GetCustomAttributes(typeof(JsonConvertAttribute), false)[0];
                return CreateTypeConverter(convAttr);
            }
            return null;
        }

        protected IJsonTypeConverter GetDefaultTypeConverter(Type type)
        {
            return TypeConverterAdapter.GetAdapter(type);
        }
        /// <summary>
        /// Constructs a converter from the convert attribute
        /// </summary>
        /// <param name="attribute">the JsonConvertAttribute decorating a property or class</param>
        /// <returns>converter</returns>
        private IJsonTypeConverter CreateTypeConverter(JsonConvertAttribute attribute)
        {
            IJsonTypeConverter converter = (IJsonTypeConverter)Activator.CreateInstance(attribute.Converter);
            if (attribute.Context != null)
            {
                converter.Context = attribute.Context;
            }            
            return converter;
        }

        public virtual bool CanWrite
        {
            get { return true; }
        }
    }
}
