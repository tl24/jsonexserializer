using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework;
using System.IO;
using JsonExSerializer.MetaData;

namespace JsonExSerializer.TypeConversion
{
    public class TypeToStringConverter : JsonConverterBase
    {
        public override Type GetSerializedType(Type sourceType)
        {
            return typeof(string);
        }

        public override object ConvertFrom(object item, ISerializerSettings serializationContext)
        {
            Type t = (Type)item;
            // TODO: Need a better way to get at this functionality
            StringWriter sw = new StringWriter();
            TypeJsonWriter jw = new TypeJsonWriter(sw, false, serializationContext.TypeAliases);
            jw.WriteTypeInfo(t);
            StringBuilder sb = sw.GetStringBuilder();
            if (sb.Length > 1 && sb[0] == '"' && sb[sb.Length - 1] == '"')
            {
                // remove double quotes
                sb.Remove(sb.Length - 1, 1);
                sb.Remove(0, 1);
            }
            return sw.ToString();
        }

        public override object ConvertTo(object item, Type sourceType, ISerializerSettings serializationContext)
        {
            string typeNameOrAlias = (string)item;
            Type typeResult = serializationContext.TypeAliases[typeNameOrAlias];
            if (typeResult == null)
                typeResult = Type.GetType(typeNameOrAlias);
            return typeResult;
        }

        private class TypeJsonWriter : JsonWriter
        {

            public TypeJsonWriter(TextWriter writer, bool indent, TypeAliasCollection typeAliases)
                : base(writer, indent, typeAliases)
            {
            }

            public new void WriteTypeInfo(Type dataType)
            {
                base.WriteTypeInfo(dataType);
            }
        }
    }
}
