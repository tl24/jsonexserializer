using System;
namespace JsonExSerializer.MetaData
{
    public interface ITypeHandlerFactory
    {
        ITypeHandler CreateTypeHandler(Type forType);
    }
}
