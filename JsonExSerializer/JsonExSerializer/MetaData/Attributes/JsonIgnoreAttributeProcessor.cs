using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace JsonExSerializer.MetaData.Attributes
{
    public class JsonIgnoreAttributeProcessor : AttributeProcessor
    {
        public override void Process(MetaDataBase metaData, ICustomAttributeProvider attributeProvider, SerializationContext serializationContext)
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
