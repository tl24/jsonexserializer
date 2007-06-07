using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Enum | AttributeTargets.Struct,AllowMultiple=true,Inherited=false)]
    public class JsonConvertAttribute : System.Attribute
    {
        private Type _converter;
        private object _context;

        public JsonConvertAttribute(Type converter)
        {
            _converter = converter;
        }

        public Type Converter
        {
            get { return this._converter; }
        }

        public object Context
        {
            get { return this._context; }
            set { this._context = value; }
        }


    }
}
