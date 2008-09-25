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

            if (Data is ISerializationCallback)
            {
                ((ISerializationCallback)Data).OnBeforeSerialization();
            }
            ListExpression expression = new ListExpression();
            try
            {
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
            finally
            {
                // make sure this is in a finally block in case the ISerializationCallback interface
                // is used to control thread locks
                if (Data is ISerializationCallback)
                {
                    ((ISerializationCallback)Data).OnAfterSerialization();
                }
            }
        }
    }
}
