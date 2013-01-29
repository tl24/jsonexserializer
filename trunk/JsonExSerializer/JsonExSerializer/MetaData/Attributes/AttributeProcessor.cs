using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace JsonExSerializer.MetaData.Attributes
{
    public abstract class AttributeProcessor
    {
        public abstract void Process(IMetaData metaData, ICustomAttributeProvider attributeProvider, ISerializerSettings config);
    }
}
