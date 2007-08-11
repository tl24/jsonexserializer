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
            set { this._constructorArguments = value; }
        }

        public override ExpressionBase ResolveReference(ReferenceIdentifier refID)
        {
            if (refID.Current == "this")
            {
                if (Parent != null)
                {
                    throw new ArgumentException("Reference for this passed to object that is not at the root", "refID");
                }
            }
            else
            {
                // have to assume that the parent checked that we were the right reference
                // should only get here if we have a parent, if no parent we're not valid
                if (Parent == null)
                    throw new ArgumentException("Invalid reference", "refID");
            }
            // it is this object, check if we need to go further
            refID = refID.ChildReference();
            if (refID.IsEmpty)
                return this;
            else
                return ResolveChildReference(refID);
        }

        /// <summary>
        /// Resolve a child reference
        /// </summary>
        /// <param name="refID">the referenced id to resolve</param>
        /// <returns>referenced expression</returns>
        protected abstract ExpressionBase ResolveChildReference(ReferenceIdentifier refID);
    }
}
