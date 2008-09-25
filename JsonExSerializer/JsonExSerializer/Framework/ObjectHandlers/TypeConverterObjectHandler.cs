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
    }
}
