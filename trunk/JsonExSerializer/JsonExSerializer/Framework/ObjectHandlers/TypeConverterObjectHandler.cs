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



        public override ExpressionBase GetExpression(object Value, JsonPath CurrentPath, ISerializerHandler serializer)
        {
            TypeHandler handler = Context.TypeHandlerFactory[Value.GetType()];
            IJsonTypeConverter converter = (handler.HasConverter ? handler.TypeConverter : (IJsonTypeConverter)Value);
            return GetExpression(Value, converter, CurrentPath, serializer);
        }

        public ExpressionBase GetExpression(object Value, IJsonTypeConverter Converter, JsonPath CurrentPath, ISerializerHandler serializer)
        {
            object convertedObject = Converter.ConvertFrom(Value, Context);
            // call serialize again in case the new type has a converter
            ExpressionBase expr = serializer.Serialize(convertedObject, CurrentPath, null);
            serializer.SetCanReference(Value);   // can't reference inside the object
            return expr;
        }

        public override bool CanHandle(Type ObjectType)
        {
            TypeHandler handler = Context.TypeHandlerFactory[ObjectType];
            return handler.HasConverter || typeof(IJsonTypeConverter).IsAssignableFrom(ObjectType);
        }

        public override object Evaluate(ExpressionBase Expression, IDeserializerHandler deserializer)
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

            return Evaluate(Expression, deserializer, converter);
        }

        public object Evaluate(ExpressionBase Expression, IDeserializerHandler deserializer, IJsonTypeConverter Converter)
        {
            Type sourceType = Expression.ResultType;
            Type destType = Converter.GetSerializedType(sourceType);
            Expression.ResultType = destType;
            object tempResult = deserializer.Evaluate(Expression);
            object result = Converter.ConvertTo(tempResult, sourceType, Context);
            Expression.OnObjectConstructed(result);
            if (result is IDeserializationCallback)
            {
                ((IDeserializationCallback)result).OnAfterDeserialization();
            }
            return result;
        }

        public override object Evaluate(ExpressionBase Expression, object existingObject, IDeserializerHandler deserializer)
        {
            //TODO: possibly allow this if the type implements IJsonTypeConverter itself
            throw new Exception("Cannot convert an existing object.");
        }

        public virtual object Evaluate(ExpressionBase Expression, object existingObject, IDeserializerHandler deserializer, IJsonTypeConverter Converter)
        {
            //TODO: possibly allow this if the type implements IJsonTypeConverter itself
            throw new Exception("Cannot convert an existing object.");
        }

    }
}
