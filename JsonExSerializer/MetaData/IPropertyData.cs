using System;
using JsonExSerializer.TypeConversion;
namespace JsonExSerializer.MetaData
{
    public interface IPropertyData
    {
        object GetValue(object instance);
        bool Ignored { get; set; }
        bool IsConstructorArgument { get; }
        string Name { get; }
        int Position { get; }
        Type PropertyType { get; }
        void SetValue(object instance, object value);
        bool CanWrite { get; }
        Type ForType { get; }
        IJsonTypeConverter TypeConverter { get; set; }
        bool HasConverter { get; }
    }
}
