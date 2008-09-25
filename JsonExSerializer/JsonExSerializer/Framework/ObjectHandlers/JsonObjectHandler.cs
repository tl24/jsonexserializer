using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;
using JsonExSerializer.MetaData;
using System.Collections;

namespace JsonExSerializer.Framework.ObjectHandlers
{
    /// <summary>
    /// Serialize a non-primitive non-scalar object.  Will use the
    /// following notation:
    /// <c>
    ///  { prop1: "value1", prop2: "value2" }
    /// </c>
    /// </summary>
    /// <param name="obj">the object to serialize</param>
    /// <param name="currentPath">object's path</param>
    public class JsonObjectHandler : ObjectHandlerBase
    {
        public JsonObjectHandler()
        {
        }

        public JsonObjectHandler(SerializationContext Context)
            : base(Context)
        {
        }

        public override ExpressionBase GetExpression(object Data, JsonPath CurrentPath, ISerializerHandler Serializer)
        {
             TypeHandler handler = Context.GetTypeHandler(Data.GetType());

            ObjectExpression expression = new ObjectExpression();
            if (handler.ConstructorParameters.Count > 0)
            {
                expression.ResultType = Data.GetType();
                foreach (IPropertyHandler ctorParm in handler.ConstructorParameters)
                {
                    object value = ctorParm.GetValue(Data);
                    ExpressionBase argExpr;
                    // TODO: Improve reference support when constructor arguments are refactored
                    if (ctorParm.HasConverter)
                    {
                        argExpr = Serializer.Serialize(value, new JsonPath(""), ctorParm.TypeConverter);
                    }
                    else
                    {
                        argExpr = Serializer.Serialize(value, new JsonPath(""));
                    }
                    if (value != null && value.GetType() != ctorParm.PropertyType)
                    {
                        argExpr = new CastExpression(value.GetType(), argExpr);
                    }
                    expression.ConstructorArguments.Add(argExpr);
                }
            }


            foreach (IPropertyHandler prop in handler.Properties)
            {
                object value = prop.GetValue(Data);
                ExpressionBase valueExpr;
                if (prop.HasConverter)
                {
                    valueExpr = Serializer.Serialize(value, CurrentPath.Append(prop.Name), prop.TypeConverter);
                }
                else
                {
                    valueExpr = Serializer.Serialize(value, CurrentPath.Append(prop.Name));
                }
                if (value != null && value.GetType() != prop.PropertyType)
                {
                    valueExpr = new CastExpression(value.GetType(), valueExpr);
                }
                expression.Add(prop.Name, valueExpr);
            }
            return expression;
        }

        public override bool CanHandle(Type ObjectType)
        {
            return true;
        }
    }
}
