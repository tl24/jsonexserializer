using System;
using JsonExSerializer.TypeConversion;
namespace JsonExSerializer.MetaData
{
    public interface ITypeHandlerFactory
    {

        ITypeHandler this[Type forType] { get; }
	
        /// <summary>
        /// Register a converter for a type, this will override any previous converters including those
        /// defined using attributes
        /// </summary>
        /// <param name="forType">the type to undergo conversion</param>
        /// <param name="converter">the converter instance</param>
        void RegisterTypeConverter(Type forType, IJsonTypeConverter converter);

        /// <summary>
        /// Register a converter for a property on a type, this will override any previous converters including those
        /// defined using attributes
        /// </summary>
        /// <param name="forType">the declaring type for the property</param>
        /// <param name="PropertyName">the property to undergo conversion</param>
        /// <param name="converter">the converter instance</param>
        void RegisterTypeConverter(Type forType, string PropertyName, IJsonTypeConverter converter);
    }
}
