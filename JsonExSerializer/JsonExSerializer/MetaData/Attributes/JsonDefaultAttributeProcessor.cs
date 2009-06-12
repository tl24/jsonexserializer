using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework;

namespace JsonExSerializer.MetaData.Attributes
{
    public class JsonDefaultAttributeProcessor : AttributeProcessor
    {
        public override void Process(MetaDataBase metaData, System.Reflection.ICustomAttributeProvider attributeProvider, SerializationContext serializationContext)
        {
            if (metaData is IPropertyData)
            {
                IPropertyData property = (IPropertyData)metaData;
                JsonExDefaultAttribute defaultAttr = ReflectionUtils.GetAttribute<JsonExDefaultAttribute>(attributeProvider, false);
                if (defaultAttr != null)
                {
                    switch (defaultAttr.DefaultValueSetting)
                    {
                        case DefaultValueOption.Default:
                        case DefaultValueOption.SuppressDefaultValues:
                            property.DefaultValueSetting = defaultAttr.DefaultValueSetting;
                            if (defaultAttr.DefaultValueSet)
                                property.DefaultValue = defaultAttr.DefaultValue;
                            break;
                        case DefaultValueOption.WriteAllValues:
                            property.DefaultValueSetting = defaultAttr.DefaultValueSetting;
                            break;
                    }
                }
            }
        }
    }
}
