using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework;
using System.Reflection;

namespace JsonExSerializer.MetaData.Attributes
{
    public class JsonDefaultAttributeProcessor : AttributeProcessor
    {
        private Dictionary<Assembly, AssemblyCache> _assemblyCache;
        public override void Process(IMetaData metaData, ICustomAttributeProvider attributeProvider, ISerializerSettings config)
        {
            if (metaData is IPropertyData)
            {
                IPropertyData property = (IPropertyData)metaData;
                JsonExDefaultAttribute defaultAttr = ReflectionUtils.GetAttribute<JsonExDefaultAttribute>(attributeProvider, false);
                if (defaultAttr != null)
                {
                    switch (defaultAttr.DefaultValueSetting)
                    {
                        case DefaultValueOption.InheritParentSetting:
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
            else if (metaData is TypeData)
            {
                Type classType = metaData.ForType;
                TypeData typeData = (TypeData)metaData;
                // apply assembly defaults first
                AssemblyCache cache = GetAssemblyCache(classType, config);
                if (cache.defaultValues != null)
                {
                    typeData.DefaultValues = new DefaultValueCollection(cache.defaultValues);
                }
                if (cache.defaultOption != DefaultValueOption.InheritParentSetting)
                    typeData.DefaultValueSetting = cache.defaultOption;

                bool typeSet = false;
                foreach (JsonExDefaultValuesAttribute attr in attributeProvider.GetCustomAttributes(typeof(JsonExDefaultValuesAttribute), false))
                {
                    if (attr.DefaultValueSetting != DefaultValueOption.InheritParentSetting)
                        typeData.DefaultValueSetting = attr.DefaultValueSetting;
                    if (attr.Type != null)
                    {
                        typeData.DefaultValues[attr.Type] = attr.DefaultValue;
                        typeSet = true;
                    }
                }
                if (typeData.DefaultValueSetting == DefaultValueOption.InheritParentSetting && typeSet)
                    typeData.DefaultValueSetting = DefaultValueOption.SuppressDefaultValues;
            }
        }

        private AssemblyCache GetAssemblyCache(Type classType, ISerializerSettings config)
        {
            if (_assemblyCache == null)
            {
                _assemblyCache = new Dictionary<Assembly, AssemblyCache>();
            }
            AssemblyCache cache = null;
            if (!_assemblyCache.TryGetValue(classType.Assembly, out cache))
            {
                cache = new AssemblyCache(classType.Assembly);
                foreach (JsonExDefaultValuesAttribute attr in classType.Assembly.GetCustomAttributes(typeof(JsonExDefaultValuesAttribute), false))
                {
                    if (attr.DefaultValueSetting != DefaultValueOption.InheritParentSetting)
                        cache.defaultOption = attr.DefaultValueSetting;
                    if (attr.Type != null)
                    {
                        if (cache.defaultValues == null)
                            cache.defaultValues = new DefaultValueCollection(config.DefaultValues);
                        cache.defaultValues[attr.Type] = attr.DefaultValue;
                    }
                }
                if (cache.defaultOption == DefaultValueOption.InheritParentSetting && cache.defaultValues != null)
                    cache.defaultOption = DefaultValueOption.SuppressDefaultValues;
                _assemblyCache[classType.Assembly] = cache;
            }
            return cache;
        }

        /// <summary>
        /// Data holder class for assembly info
        /// </summary>
        private class AssemblyCache
        {
            public Assembly typeAssembly;
            public DefaultValueCollection defaultValues;
            public DefaultValueOption defaultOption;
            public AssemblyCache(Assembly typeAssembly)
            {
                this.typeAssembly = typeAssembly;
            }
        }
    }
}
