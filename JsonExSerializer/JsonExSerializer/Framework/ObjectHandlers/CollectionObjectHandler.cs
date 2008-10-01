using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;
using JsonExSerializer.MetaData;
using JsonExSerializer.Collections;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public class CollectionObjectHandler : ObjectHandlerBase
    {
        public CollectionObjectHandler()
        {
        }

        public CollectionObjectHandler(SerializationContext context)
            : base(context)
        {
        }

        public override bool CanHandle(Type objectType)
        {
            return Context.TypeHandlerFactory[objectType].IsCollection();
        }

        public override ExpressionBase GetExpression(object data, JsonPath currentPath, ISerializerHandler serializer)
        {
            TypeHandler handler = Context.GetTypeHandler(data.GetType());

            CollectionHandler collectionHandler = handler.GetCollectionHandler();
            Type elemType = collectionHandler.GetItemType(handler.ForType);

            int index = 0;

            ListExpression expression = new ListExpression();
            foreach (object value in collectionHandler.GetEnumerable(data))
            {
                ExpressionBase itemExpr = serializer.Serialize(value, currentPath.Append(index));
                if (value != null && value.GetType() != elemType)
                {
                    itemExpr = new CastExpression(value.GetType(), itemExpr);
                }
                expression.Add(itemExpr);
                index++;
            }
            return expression;
        }

        public override object Evaluate(ExpressionBase expression, IDeserializerHandler deserializer)
        {
            return Evaluate(expression, null, deserializer);
        }
        public override object Evaluate(ExpressionBase expression, object existingObject, IDeserializerHandler deserializer)
        {
            Type ItemType;
            ListExpression list = (ListExpression)expression;
            ICollectionBuilder builder = ConstructBuilder(existingObject, list, out ItemType);
            object result = EvaluateItems(list, builder, ItemType, deserializer);
            if (result is IDeserializationCallback)
            {
                ((IDeserializationCallback)result).OnAfterDeserialization();
            }
            return result;
        }

        protected virtual object EvaluateItems(ListExpression expression, ICollectionBuilder builder, Type itemType, IDeserializerHandler deserializer)
        {
            object result = null;
            bool constructedEventSent = false;
            try
            {
                result = builder.GetReference();
                expression.OnObjectConstructed(result);
                constructedEventSent = true;
            }
            catch
            {
                // this might fail if the builder's not ready
            }
            foreach (ExpressionBase item in expression.Items)
            {
                item.ResultType = itemType;
                object itemResult = deserializer.Evaluate(item);
                builder.Add(itemResult);
            }
            result = builder.GetResult();
            if (!constructedEventSent)
                expression.OnObjectConstructed(result);
            return result;
        }

        protected virtual ICollectionBuilder ConstructBuilder(object collection, ListExpression list, out Type itemType)
        {
            TypeHandler typeHandler = Context.GetTypeHandler(list.ResultType);
            CollectionHandler collHandler = typeHandler.GetCollectionHandler();
            itemType = collHandler.GetItemType(typeHandler.ForType);
            if (itemType == null)
                throw new Exception("Null item type returned from " + collHandler.GetType() + " for Collection type: " + typeHandler.ForType);

            if (collection != null)
                return collHandler.ConstructBuilder(collection);
            else
                return collHandler.ConstructBuilder(typeHandler.ForType, list.Items.Count);
        }
    }
}
