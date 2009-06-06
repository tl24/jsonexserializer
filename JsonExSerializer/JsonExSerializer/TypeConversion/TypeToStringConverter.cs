using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.TypeConversion
{
    public class TypeToStringConverter : JsonConverterBase
    {
        public override Type GetSerializedType(Type sourceType)
        {
            return typeof(string);
        }

        public override object ConvertFrom(object item, SerializationContext serializationContext)
        {
            Type t = (Type)item;
            string typeName = serializationContext.TypeAliases[t];
            if (typeName != null)
                return typeName;
            else
                return t.AssemblyQualifiedName;
        }

        public override object ConvertTo(object item, Type sourceType, SerializationContext serializationContext)
        {
            string typeNameOrAlias = (string)item;
            Type typeResult = serializationContext.TypeAliases[typeNameOrAlias];
            if (typeResult == null)
                typeResult = Type.GetType(typeNameOrAlias);
            return typeResult;
        }
    }
}
