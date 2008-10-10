using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.Expressions;
using System.Collections;

namespace JsonExSerializer.Framework.ExpressionHandlers.Collections
{
    public abstract class CollectionObjectHandlerBase : ExpressionHandlerBase
    {
        protected CollectionObjectHandlerBase()
        {
        }
        protected CollectionObjectHandlerBase(SerializationContext Context)
            : base(Context)
        {
        }

        protected virtual Type GetItemType(Type CollectionType)
        {
            return typeof(object);
        }

        public override ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler serializer)
        {
            return GetExpression((IEnumerable)data, GetItemType(data.GetType()), CurrentPath, serializer);
        }

        protected virtual ExpressionBase GetExpression(IEnumerable Items, Type ItemType, JsonPath CurrentPath, ISerializerHandler serializer)
        {
            int index = 0;

            ArrayExpression expression = new ArrayExpression();
            foreach (object value in Items)
            {
                ExpressionBase itemExpr = serializer.Serialize(value, CurrentPath.Append(index));
                if (value != null && value.GetType() != ItemType)
                {
                    itemExpr = new CastExpression(value.GetType(), itemExpr);
                }
                expression.Add(itemExpr);
                index++;
            }
            return expression;
        }

        public override object Evaluate(ExpressionBase Expression, IDeserializerHandler deserializer)
        {
            object collection = ConstructCollection((ArrayExpression)Expression, deserializer);
            return Evaluate(Expression, collection, deserializer);
        }

        public override object Evaluate(ExpressionBase Expression, object existingObject, IDeserializerHandler deserializer)
        {
            Expression.OnObjectConstructed(existingObject);
            Type itemType = GetItemType(existingObject.GetType());
            EvaluateItems((ArrayExpression) Expression, existingObject, itemType, deserializer);
            if (existingObject is IDeserializationCallback)
                ((IDeserializationCallback)existingObject).OnAfterDeserialization();
            return existingObject;
        }

        protected virtual object ConstructCollection(ArrayExpression Expression, IDeserializerHandler deserializer)
        {
            object result = Activator.CreateInstance(Expression.ResultType);
            return result;
        }

        protected virtual void EvaluateItems(ArrayExpression Expression, object Collection, Type ItemType, IDeserializerHandler deserializer)
        {
            foreach (ExpressionBase item in Expression.Items)
            {
                item.ResultType = ItemType;
                object itemResult = deserializer.Evaluate(item);
                AddItem(Collection, itemResult);
            }
        }

        /// <summary>
        /// Adds the item to the collection
        /// </summary>
        /// <param name="Collection"></param>
        /// <param name="itemResult"></param>
        protected abstract void AddItem(object Collection, object itemResult);
    }
}
