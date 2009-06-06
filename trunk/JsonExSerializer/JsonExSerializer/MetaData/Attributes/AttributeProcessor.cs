using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace JsonExSerializer.MetaData.Attributes
{
    public abstract class AttributeProcessor
    {
        public abstract void Process(MetaDataBase metaData, ICustomAttributeProvider attributeProvider, SerializationContext serializationContext);
    }
}
