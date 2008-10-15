using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace JsonExSerializer.MetaData
{
    public class ConstructorAutoWireUp
    {
        TypeData typeData;
        IList<IPropertyData> properties;

        private ConstructorAutoWireUp(TypeData typeData, IList<IPropertyData> properties)
        {
            this.typeData = typeData;
            this.properties = properties;
        }

        public static IList<IPropertyData> WireUpConstructor(TypeData typeData, IList<IPropertyData> properties)
        {
            ConstructorAutoWireUp wireup = new ConstructorAutoWireUp(typeData, properties);
            return wireup.WireUp();
        }

        private IList<IPropertyData> WireUp()
        {
            ConstructorInfo[] constructors = typeData.ForType.GetConstructors();
            Array.Sort<ConstructorInfo>(constructors, delegate(ConstructorInfo a, ConstructorInfo b) { return a.GetParameters().Length - b.GetParameters().Length; });

            foreach (ConstructorInfo constructor in constructors)
            {
                IList<IPropertyData> result = EvalConstructor(constructor);
                if (result != null)
                    return result;
            }
            return new IPropertyData[0];
        }

        private IList<IPropertyData> EvalConstructor(ConstructorInfo constructor)
        {
            ParameterInfo[] parameters = constructor.GetParameters();
            IPropertyData[] matchingProperties = new IPropertyData[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                IPropertyData property = FindProperty(parameter.Name, parameter.ParameterType);
                if (property == null)
                    return null;

                matchingProperties[i] = property;
            }
            // everything matched, so set the constructor properties
            for (int i = 0; i < matchingProperties.Length; i++)
            {
                matchingProperties[i].ConstructorParameterName = parameters[i].Name;
            }
            return matchingProperties;
        }

        /// <summary>
        /// Finds a property with the specified name and property type
        /// </summary>
        /// <param name="name">the name of the property</param>
        /// <param name="type">the property type</param>
        /// <returns></returns>
        private IPropertyData FindProperty(string name, Type type)
        {
            foreach (IPropertyData property in properties)
            {
                if (property.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)
                    && property.PropertyType == type)
                    return property;
            }
            return null;
        }
    }
}
