using System;
using System.Collections.Generic;
using JsonExSerializer.MetaData.Attributes;
using System.Reflection;
using JsonExSerializer.TypeConversion;
using System.Linq.Expressions;
namespace JsonExSerializer.MetaData
{
    public interface ITypeSettings
    {
        IList<AttributeProcessor> AttributeProcessors { get; }
        IPropertyNamingStrategy PropertyNamingStrategy { get; set; }
        void RegisterTypeConverter(Type forType, IJsonTypeConverter converter);
        void RegisterTypeConverter(Type forType, string propertyName, IJsonTypeConverter converter);
        ITypeData this[Type forType] { get; }
        ITypeData<T> Type<T>();
    }
}
