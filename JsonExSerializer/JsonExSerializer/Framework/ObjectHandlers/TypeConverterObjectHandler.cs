using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;
using JsonExSerializer.MetaData;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    public class TypeConverterObjectHandler : ObjectHandlerBase
    {
        public TypeConverterObjectHandler()
            : base()
        {
        }

        public TypeConverterObjectHandler(SerializationContext Context)
            : base(Context)
        {
        }



        public override ExpressionBase GetExpression(object Value, JsonPath CurrentPath, ISerializerHandler Serializer)
        {
            TypeHandler handler = Context.TypeHandlerFactory[Value.GetType()];
            IJsonTypeConverter converter = (handler.HasConverter ? handler.TypeConverter : (IJsonTypeConverter)Value);
            return GetExpression(Value, converter, CurrentPath, Serializer);
        }

        public ExpressionBase GetExpression(object Value, IJsonTypeConverter Converter, JsonPath CurrentPath, ISerializerHandler Serializer)
        {
            object convertedObject = Converter.ConvertFrom(Value, Context);
            // call serialize again in case the new type has a converter
            ExpressionBase expr = Serializer.Serialize(convertedObject, CurrentPath, null);
            Serializer.SetCanReference(Value);   // can't reference inside the object
            return expr;
        }

        public override bool CanHandle(Type ObjectType)
        {
            TypeHandler handler = Context.TypeHandlerFactory[ObjectType];
            return handler.HasConverter || typeof(IJsonTypeConverter).IsAssignableFrom(ObjectType);
        }

        public override object Evaluate(ExpressionBase Expression, IDeserializerHandler Deserializer)
        {
            Type sourceType = Expression.ResultType;
            TypeHandler handler = Context.TypeHandlerFactory[sourceType];
            IJsonTypeConverter converter;
            if (typeof(IJsonTypeConverter).IsAssignableFrom(sourceType))
            {
                converter = (IJsonTypeConverter) Activator.CreateInstance(sourceType);
            }
            else
            {
                converter = handler.TypeConverter;
            }

            return Evaluate(Expression, Deserializer, converter);
        }

        public object Evaluate(ExpressionBase Expression, IDeserializerHandler Deserializer, IJsonTypeConverter Converter)
        {
            Type sourceType = Expression.ResultType;
            Type destType = Converter.GetSerializedType(sourceType);
            Expression.ResultType = destType;
            object tempResult = Deserializer.Evaluate(Expression);
            object result = Converter.ConvertTo(tempResult, sourceType, Context);
            Expression.OnObjectConstructed(result);
            if (result is IDeserializationCallback)
            {
                ((IDeserializationCallback)result).OnAfterDeserialization();
            }
            return result;
        }

        public override object Evaluate(ExpressionBase Expression, object ExistingObject, IDeserializerHandler Deserializer)
        {
            //TODO: possibly allow this if the type implements IJsonTypeConverter itself
            throw new Exception("Cannot convert an existing object.");
        }

        public virtual object Evaluate(ExpressionBase Expression, object ExistingObject, IDeserializerHandler Deserializer, IJsonTypeConverter Converter)
        {
            //TODO: possibly allow this if the type implements IJsonTypeConverter itself
            throw new Exception("Cannot convert an existing object.");
        }

    }
}
