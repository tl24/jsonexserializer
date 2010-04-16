using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using JsonExSerializer.Framework;

namespace JsonExSerializer.MetaData.Attributes
{
    public class ConstructorParameterAttributeProcessor : AttributeProcessor
    {
        public override void Process(IMetaData metaData, ICustomAttributeProvider attributeProvider, IConfiguration config)
        {
            if (metaData is IPropertyData)
            {
                IPropertyData property = (IPropertyData)metaData;
                ConstructorParameterAttribute ctorAttr = ReflectionUtils.GetAttribute<ConstructorParameterAttribute>(attributeProvider, false);
                if (ctorAttr != null)
                {
                    if (ctorAttr.Position >= 0)
                        property.Position = ctorAttr.Position;
                    else if (!string.IsNullOrEmpty(ctorAttr.Name))
                        property.ConstructorParameterName = ctorAttr.Name;
                    else
                        property.ConstructorParameterName = property.Name;

                    property.Ignored = false;
                }

            }
        }
    }
}
