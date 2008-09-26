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

        public CollectionObjectHandler(SerializationContext Context)
            : base(Context)
        {
        }

        public override bool CanHandle(Type ObjectType)
        {
            return Context.TypeHandlerFactory[ObjectType].IsCollection();
        }

        public override ExpressionBase GetExpression(object Data, JsonPath CurrentPath, ISerializerHandler Serializer)
        {
            TypeHandler handler = Context.GetTypeHandler(Data.GetType());

            CollectionHandler collectionHandler = handler.GetCollectionHandler();
            Type elemType = collectionHandler.GetItemType(handler.ForType);

            int index = 0;

            ListExpression expression = new ListExpression();
            foreach (object value in collectionHandler.GetEnumerable(Data))
            {
                ExpressionBase itemExpr = Serializer.Serialize(value, CurrentPath.Append(index));
                if (value != null && value.GetType() != elemType)
                {
                    itemExpr = new CastExpression(value.GetType(), itemExpr);
                }
                expression.Add(itemExpr);
                index++;
            }
            return expression;
        }

        public override object Evaluate(ExpressionBase Expression, IDeserializerHandler Deserializer)
        {
            return Evaluate(Expression, null, Deserializer);
        }
        public override object Evaluate(ExpressionBase Expression, object ExistingObject, IDeserializerHandler Deserializer)
        {
            Type ItemType;
            ICollectionBuilder builder = ConstructBuilder(ExistingObject, (ListExpression)Expression, out ItemType);
            object result = EvaluateItems((ListExpression)Expression, builder, ItemType, Deserializer);
            if (result is IDeserializationCallback)
            {
                ((IDeserializationCallback)result).OnAfterDeserialization();
            }
            return result;
        }

        protected virtual object EvaluateItems(ListExpression Expression, ICollectionBuilder Builder, Type ItemType, IDeserializerHandler Deserializer)
        {
            object result = null;
            bool constructedEventSent = false;
            try
            {
                result = Builder.GetReference();
                Expression.OnObjectConstructed(result);
                constructedEventSent = true;
            }
            catch
            {
                // this might fail if the builder's not ready
            }
            foreach (ExpressionBase item in Expression.Items)
            {
                item.ResultType = ItemType;
                object itemResult = Deserializer.Evaluate(item);
                Builder.Add(itemResult);
            }
            result = Builder.GetResult();
            if (!constructedEventSent)
                Expression.OnObjectConstructed(result);
            return result;
        }

        protected virtual ICollectionBuilder ConstructBuilder(object collection, ListExpression list, out Type ItemType)
        {
            TypeHandler typeHandler = Context.GetTypeHandler(list.ResultType);
            CollectionHandler collHandler = typeHandler.GetCollectionHandler();
            ItemType = collHandler.GetItemType(typeHandler.ForType);
            if (ItemType == null)
                throw new Exception("Null item type returned from " + collHandler.GetType() + " for Collection type: " + typeHandler.ForType);

            if (collection != null)
                return collHandler.ConstructBuilder(collection);
            else
                return collHandler.ConstructBuilder(typeHandler.ForType, list.Items.Count);
        }
    }
}
