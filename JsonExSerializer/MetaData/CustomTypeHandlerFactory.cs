using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// TypeHandlerFactory implementation that allows you to specify the TypeHandler
    /// class that gets created when the factory creates ITypeHandler implementations
    /// </summary>
    public class CustomTypeHandlerFactory : TypeHandlerFactory
    {
        private Type _typeHandlerType = typeof(TypeHandler);
        private ConstructorInfo ctor;

        public CustomTypeHandlerFactory(Type TypeHandlerType, SerializationContext context)
            : base(context)
        {
            _typeHandlerType = TypeHandlerType;
        }

        protected override TypeHandler CreateNew(Type forType)
        {
            ConstructorInfo constructor = GetConstructor();
            ParameterInfo[] parms = constructor.GetParameters();
            object[] args = new object[parms.Length];
            for (int i = 0; i < parms.Length; i++)
            {
                if (parms[i].ParameterType.IsAssignableFrom(typeof(Type)))
                    args[i] = forType;
                else if (parms[i].ParameterType.IsAssignableFrom(typeof(SerializationContext)))
                    args[i] = this.Context;
            }
            return (TypeHandler)constructor.Invoke(args);
        }

        private ConstructorInfo GetConstructor()
        {
            if (ctor == null)
            {
                ConstructorInfo[] ctors = _typeHandlerType.GetConstructors();
                // sort constructors by arg length desc so we can find the most relevant
                // constructor first
                Array.Sort<ConstructorInfo>(ctors,
                    delegate(ConstructorInfo a, ConstructorInfo b)
                    {
                        return b.GetParameters().Length - a.GetParameters().Length;
                    }
                );

                // Check parameter types looking for Type or SerializationContext
                foreach (ConstructorInfo constructor in ctors)
                {
                    bool cantUse = false;
                    ParameterInfo[] parms = constructor.GetParameters();
                    if (parms.Length > 2)
                        continue;

                    foreach (ParameterInfo p in parms)
                    {
                        if (p.ParameterType != typeof(Type)
                            && !p.ParameterType.IsAssignableFrom(typeof(SerializationContext)))
                        {
                            // contains a type we don't recognize, keep looking
                            cantUse = true;
                            break;
                        }
                    }
                    if (!cantUse)
                    {
                        ctor = constructor;
                        break;
                    }
                }
            }
            return ctor;
        }
    }
}
