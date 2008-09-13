/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Expression
{
    // object types, collection types
    /// <summary>
    /// Base class for complex types: objects and collections
    /// </summary>
    public abstract class ComplexExpressionBase : ExpressionBase {
        private IList<ExpressionBase> _constructorArguments;

        public ComplexExpressionBase()
        {
            _constructorArguments = new List<ExpressionBase>();
        }

        /// <summary>
        /// Arguments to the constructor if any
        /// </summary>
        public IList<ExpressionBase> ConstructorArguments
        {
            get { return this._constructorArguments; }
            set { 
                this._constructorArguments = value;
                if (value != null)
                {
                    foreach (ExpressionBase exp in value)
                    {
                        exp.Parent = this;
                    }
                }
            }
        }
    }
}
