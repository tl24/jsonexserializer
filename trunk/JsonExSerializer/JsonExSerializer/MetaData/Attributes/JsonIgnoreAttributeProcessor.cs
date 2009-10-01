using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace JsonExSerializer.MetaData.Attributes
{
    public class JsonIgnoreAttributeProcessor : AttributeProcessor
    {
        public override void Process(IMetaData metaData, ICustomAttributeProvider attributeProvider, IConfiguration config)
        {
            if (metaData is IPropertyData)
            {
                IPropertyData property = (IPropertyData) metaData;
                if (attributeProvider.IsDefined(typeof(JsonExIgnoreAttribute), false))
                    property.Ignored = true;
            }
        }
    }
}
