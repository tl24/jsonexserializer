/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using JsonExSerializer.MetaData;

namespace JsonExSerializer.Expression
{
    /// <summary>
    /// Evaluator for Object expressions
    /// </summary>
    sealed class ObjectEvaluator : ComplexEvaluatorBase
    {

        public ObjectEvaluator(ObjectExpression expression)
            : base(expression)
        {
        }

        protected override object Construct()
        {
            // set the default type if none set
            if (Expression.ConstructorArguments.Count > 0)
            {
                TypeHandler handler = Context.GetTypeHandler(Expression.ResultType);
                Type[] definedTypes = GetConstructorParameterTypes(handler.ConstructorParameters);

                CtorArgTypeResolver resolver = new CtorArgTypeResolver(Expression, this.Context, definedTypes);
                Type[] resolvedTypes = resolver.ResolveTypes();
                for (int i = 0; i < resolvedTypes.Length; i++)
                {
                    if (resolvedTypes[i] != null)
                        Expression.ConstructorArguments[i].ResultType = resolvedTypes[i];
                }
            }
            return base.Construct();
        }

        private Type[] GetConstructorParameterTypes(IList<IPropertyHandler> ConstructorParameters)
        {
            Type[] types = new Type[ConstructorParameters.Count];
            for (int i = 0; i < ConstructorParameters.Count; i++)
            {
                types[i] = ConstructorParameters[i].PropertyType;
            }
            return types;
        }
        /// <summary>
        /// Populate the list with its values
        /// </summary>
        protected override void UpdateResult()
        {            
            foreach (KeyValueExpression Item in Expression.Properties)
            {
                // evaluate the item and let it assign itself?
                Item.Evaluate(Context);
            }
        }

        public new ObjectExpression Expression
        {
            get { return (ObjectExpression)_expression; }
            set { _expression = value; }
        }
    }
}
