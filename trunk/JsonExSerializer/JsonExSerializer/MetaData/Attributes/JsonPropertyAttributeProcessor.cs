using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using JsonExSerializer.Framework;

namespace JsonExSerializer.MetaData.Attributes
{
    public class JsonPropertyAttributeProcessor : AttributeProcessor
    {
        public override void Process(IMetaData metaData, ICustomAttributeProvider attributeProvider, ISerializerSettings config)
        {
            if (metaData is IPropertyData)
            {
                IPropertyData property = (IPropertyData) metaData;
                JsonExPropertyAttribute attr = ReflectionUtils.GetAttribute<JsonExPropertyAttribute>(attributeProvider, false);
                if (attr != null)
                {
                    property.Ignored = false;
                    if (!string.IsNullOrEmpty(attr.Alias))
                    {
                        property.Alias = attr.Alias;
                    }
                }
            }
        }
    }
}
