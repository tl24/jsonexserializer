using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public class DictionaryObjectHandler : ObjectHandlerBase
    {

        public DictionaryObjectHandler()
        {
        }

        public DictionaryObjectHandler(SerializationContext Context)
            : base(Context)
        {
        }
        /// <summary>
        /// Serialize an object implementing IDictionary.  The serialized data is similar to a regular
        /// object, except that the keys of the dictionary are used instead of properties.
        /// </summary>
        /// <param name="dictionary">the dictionary object</param>
        /// <param name="currentPath">object's path</param>
        public override JsonExSerializer.Expression.ExpressionBase GetExpression(object data, JsonExSerializer.Expression.JsonPath CurrentPath, ISerializerHandler Serializer)
        {
            IDictionary dictionary = (IDictionary)data;
            Type itemType = typeof(object);
            Type genericDictionary = null;

            if ((genericDictionary = dictionary.GetType().GetInterface(typeof(IDictionary<,>).Name)) != null)
            {
                itemType = genericDictionary.GetGenericArguments()[1];
            }

            ObjectExpression expression = new ObjectExpression();
            foreach (DictionaryEntry pair in dictionary)
            {
                //Serialize(pair.Key, subindent, "", null);
                //may not work in all cases
                object value = pair.Value;
                ExpressionBase valueExpr = Serializer.Serialize(value, CurrentPath.Append(pair.Key.ToString()));
                if (value != null && value.GetType() != itemType)
                {
                    valueExpr = new CastExpression(value.GetType(), valueExpr);
                }
                expression.Add(pair.Key.ToString(), valueExpr);
            }
            return expression;
        }

        public override bool CanHandle(object Data)
        {
            return (Data is IDictionary);
        }
    }
}
