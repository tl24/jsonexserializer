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
    public class ObjectEvaluator : ComplexEvaluatorBase
    {

        public ObjectEvaluator(ObjectExpression expression)
            : base(expression)
        {
        }

        protected override object Construct()
        {
            // set the default type if none set
            Expression.SetResultTypeIfNotSet(typeof(Hashtable));
            if (Expression.ConstructorArguments.Count > 0)
            {
                ITypeHandler handler = Context.GetTypeHandler(Expression.ResultType);
                if (handler.ConstructorParameters.Count == Expression.ConstructorArguments.Count)
                {
                    for (int i = 0; i < handler.ConstructorParameters.Count; i++)
                    {
                        Expression.ConstructorArguments[i].SetResultTypeIfNotSet(handler.ConstructorParameters[i].PropertyType);
                    }
                }
                else
                {
                    // no constructor parameters defined or not enough defined, try to find one that matches
                    //TODO: Using too much reflection info, needs to be moved to ITypeHandler somehow
                    DetermineConstructorArgTypes(handler.ConstructorParameters);
                }
            }
            return base.Construct();
        }
        /// <summary>
        /// Populate the list with its values
        /// </summary>
        protected override void InitializeResult()
        {            
            foreach (KeyValueExpression Item in Expression.Properties)
            {
                // evaluate the item and let it assign itself?
                Item.Evaluate(Context, this._result);
            }
        }

        public new ObjectExpression Expression
        {
            get { return (ObjectExpression)_expression; }
            set { _expression = value; }
        }

        /// <summary>
        /// Determines the constructor argument types when there are no mappings for them.
        /// it does this by searching the constructors on the created type for a compatible match.
        /// </summary>
        private void DetermineConstructorArgTypes(IList<IPropertyHandler> definedArguments)
        {

            int argCount = Expression.ConstructorArguments.Count;

            if (definedArguments.Count > 0)
            {
                // if we have some definitions, try to use them to determine type info
                for (int i = 0; i < Math.Min(definedArguments.Count, argCount); i++)
                {
                    Expression.ConstructorArguments[i].SetResultTypeIfNotSet(definedArguments[i].PropertyType);
                }
            }
            List<ConstructorInfo> ctors = new List<ConstructorInfo>();
            foreach (ConstructorInfo ctorInfo in Expression.ResultType.GetConstructors())
            {
                if (ctorInfo.GetParameters().Length == argCount)
                    ctors.Add(ctorInfo);
            }

            if (ctors.Count > 1)
            {
                // try to narrow it down
                int i = ctors.Count - 1;
                while (i >= 0 && ctors.Count > 1)
                {
                    ConstructorInfo ctor = ctors[i];
                    if (!IsConstructorCompatible(ctor))
                    {
                        ctors.RemoveAt(i);
                    }
                    i--;
                }
            }

            if (ctors.Count == 1)
            {
                ParameterInfo[] parms = ctors[0].GetParameters();

                for (int j = 0; j < parms.Length; j++)
                {
                    Expression.ConstructorArguments[j].SetResultTypeIfNotSet(parms[j].ParameterType);
                }
            }
            else
            {
                throw new ParseException(string.Format("Unable to find a suitable constructor for type: {0} with {1} arguments.",
                    Expression.ResultType, Expression.ConstructorArguments.Count));
            }
        }

        private bool IsConstructorCompatible(ConstructorInfo constructor)
        {
            ParameterInfo[] parms = constructor.GetParameters();
            // should have already been checked, but just double-check
            if (parms.Length != Expression.ConstructorArguments.Count)
                return false;

            // check to make sure each arg type is compatible, if its set
            for (int i = 0; i < parms.Length; i++)
            {
                Type exprType = null;
                if (Expression.ConstructorArguments[i].ResultType != null)
                    exprType = Expression.ConstructorArguments[i].ResultType;

                if (exprType != null && !parms[i].ParameterType.IsAssignableFrom(exprType))
                    return false;
            }
            return true;
        }
    }
}
