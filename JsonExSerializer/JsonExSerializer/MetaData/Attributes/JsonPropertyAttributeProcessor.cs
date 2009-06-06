using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace JsonExSerializer.MetaData.Attributes
{
    public class JsonPropertyAttributeProcessor : AttributeProcessor
    {
        public override void Process(MetaDataBase metaData, ICustomAttributeProvider attributeProvider, SerializationContext serializationContext)
        {
            if (metaData is IPropertyData)
            {
                IPropertyData property = (IPropertyData) metaData;
                if (attributeProvider.IsDefined(typeof(JsonExPropertyAttribute), false))
                    property.Ignored = false;
            }
        }
    }
}
