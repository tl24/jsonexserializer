using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using JsonExSerializer.Framework;
using JsonExSerializer.Collections;

namespace JsonExSerializer.MetaData.Attributes
{
    public class JsonCollectionAttributeProcessor : AttributeProcessor
    {
        public override void Process(IMetaData metaData, ICustomAttributeProvider attributeProvider, ISerializerSettings config)
        {
            TypeData typeData = metaData as TypeData;
            if (typeData == null)
                return;

            JsonExCollectionAttribute attr = ReflectionUtils.GetAttribute<JsonExCollectionAttribute>(attributeProvider, true);
            if (attr == null)
                return;

            if (!attr.IsValid())
                throw new Exception("Invalid JsonExCollectionAttribute specified for " + attributeProvider + ", either CollectionHandlerType or ItemType or both must be specified");

            

            Type collHandlerType = attr.GetCollectionHandlerType();
            Type itemType = attr.GetItemType();

            // Try exact type match first
            CollectionHandler handler = null;

            if (collHandlerType == null)
            {
                handler = typeData.FindCollectionHandler();
                handler = new CollectionHandlerWrapper(handler, typeData.ForType, itemType);
            }

            bool registerHandler = false;
            if (handler == null)
            {
                handler = ConstructOrFindHandler(config, collHandlerType, ref registerHandler);
            }

            typeData.CollectionHandler = handler;
            // install the handler
            if (registerHandler)
                config.RegisterCollectionHandler(handler);
            
        }

        protected virtual CollectionHandler ConstructOrFindHandler(ISerializerSettings config, Type collHandlerType, ref bool handlerConstructed)
        {
            handlerConstructed = false;
            CollectionHandler handler = config.CollectionHandlers.Find(delegate(CollectionHandler h) { return h.GetType() == collHandlerType; });
            if (handler != null)
                return handler;

            // try inherited type next
            handler = config.CollectionHandlers.Find(delegate(CollectionHandler h) { return collHandlerType.IsInstanceOfType(h); });
            if (handler != null)
                return handler;

            // create the handler
            handler = (CollectionHandler)Activator.CreateInstance(collHandlerType);
            handlerConstructed = true;
            return handler;
        }
    }
}
