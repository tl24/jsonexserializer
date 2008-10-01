using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public class DictionaryObjectHandler : JsonObjectHandler
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
        public override ExpressionBase GetExpression(object data, JsonPath CurrentPath, ISerializerHandler serializer)
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
                ExpressionBase valueExpr = serializer.Serialize(value, CurrentPath.Append(pair.Key.ToString()));
                if (value != null && value.GetType() != itemType)
                {
                    valueExpr = new CastExpression(value.GetType(), valueExpr);
                }
                expression.Add(pair.Key.ToString(), valueExpr);
            }
            return expression;
        }

        public override bool CanHandle(Type ObjectType)
        {
            return typeof(IDictionary).IsAssignableFrom(ObjectType);
        }

        public override object Evaluate(ExpressionBase Expression, object existingObject, IDeserializerHandler deserializer)
        {
            Type _dictionaryKeyType = typeof(string);
            Type _dictionaryValueType = null;
            Type genDict = existingObject.GetType().GetInterface(typeof(IDictionary<,>).Name);
            // attempt to figure out what the types of the values are, if no type is set already
            if (genDict != null)
            {
                Type[] genArgs = genDict.GetGenericArguments();
                _dictionaryKeyType = genArgs[0];
                _dictionaryValueType = genArgs[1];
            }

            ObjectExpression objectExpression = (ObjectExpression)Expression;
            foreach (KeyValueExpression keyValue in objectExpression.Properties)
            {
                // if no type set, set one
                keyValue.KeyExpression.ResultType = _dictionaryKeyType;
                if (_dictionaryValueType != null)
                    keyValue.ValueExpression.ResultType = _dictionaryValueType;

                object keyObject = deserializer.Evaluate(keyValue.KeyExpression);
                object result = deserializer.Evaluate(keyValue.ValueExpression);
                ((IDictionary)existingObject)[keyObject] = result;
            }
            return existingObject;
        }
    }
}
