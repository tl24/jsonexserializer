using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;
using System.Collections;

namespace JsonExSerializer.Framework.ObjectHandlers.Collections
{
    public abstract class CollectionObjectHandlerBase : ObjectHandlerBase
    {
        public CollectionObjectHandlerBase()
        {
        }
        public CollectionObjectHandlerBase(SerializationContext Context) : base(Context)
        {
        }

        protected virtual Type GetItemType(Type CollectionType)
        {
            return typeof(object);
        }

        public override ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler Serializer)
        {
            return GetExpression((IEnumerable)data, GetItemType(data.GetType()), CurrentPath, Serializer);
        }

        protected virtual ExpressionBase GetExpression(IEnumerable Items, Type ItemType, JsonPath CurrentPath, ISerializerHandler Serializer)
        {
            int index = 0;

            ListExpression expression = new ListExpression();
            foreach (object value in Items)
            {
                ExpressionBase itemExpr = Serializer.Serialize(value, CurrentPath.Append(index));
                if (value != null && value.GetType() != ItemType)
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
            object collection = ConstructCollection((ListExpression)Expression, Deserializer);
            return Evaluate(Expression, collection, Deserializer);
        }

        public override object Evaluate(ExpressionBase Expression, object ExistingObject, IDeserializerHandler Deserializer)
        {
            Expression.OnObjectConstructed(ExistingObject);
            Type itemType = GetItemType(ExistingObject.GetType());
            EvaluateItems((ListExpression) Expression, ExistingObject, itemType, Deserializer);
            if (ExistingObject is IDeserializationCallback)
                ((IDeserializationCallback)ExistingObject).OnAfterDeserialization();
            return ExistingObject;
        }

        protected virtual object ConstructCollection(ListExpression Expression, IDeserializerHandler Deserializer)
        {
            object result = Activator.CreateInstance(Expression.ResultType);
            return result;
        }

        protected virtual void EvaluateItems(ListExpression Expression, object Collection, Type ItemType, IDeserializerHandler Deserializer)
        {
            foreach (ExpressionBase item in Expression.Items)
            {
                item.ResultType = ItemType;
                object itemResult = Deserializer.Evaluate(item);
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
