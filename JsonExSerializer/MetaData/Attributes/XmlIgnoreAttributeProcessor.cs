using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using JsonExSerializer.Framework;
using System.Xml.Serialization;

namespace JsonExSerializer.MetaData.Attributes
{
    public class XmlIgnoreAttributeProcessor : AttributeProcessor
    {
        public override void Process(IMetaData metaData, ICustomAttributeProvider attributeProvider, IConfiguration config)
        {
            if (metaData is IPropertyData)
            {
                IPropertyData property = (IPropertyData)metaData;
                if (attributeProvider.IsDefined(typeof(XmlIgnoreAttribute), false))
                    property.Ignored = true;
            }
        }
    }
}
