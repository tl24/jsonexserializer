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
    /// <summary>
    /// A reference to another object
    /// </summary>
    public sealed class ReferenceExpression : ExpressionBase
    {
        private ExpressionBase _reference;   // the expression that is referenced
        private ReferenceIdentifier _refID; // the reference ID
        public ReferenceExpression(ReferenceIdentifier refID)
        {
            this._refID = refID;
        }

        public override object Evaluate(SerializationContext context) {
            if (_reference == null)
            {
                // find the root so we can resolve this reference
                ExpressionBase p = this.Parent;
                while (p != null)
                {
                    if (p.Parent == null)
                        break;

                    p = p.Parent;
                }
                if (p == null)
                    throw new Exception("Unable to find root element to resolve the reference");

                ReferenceVisitor visitor = new ReferenceVisitor(_refID);
                visitor.Visit(p);
                if (visitor.ReferencedExpression == null)
                    throw new Exception("Unable to resolve reference to " + _refID);
                //we found the root, resolve it
                _reference = visitor.ReferencedExpression;
            }
            return _reference.GetReference(context);
        }

        public override object GetReference(SerializationContext context)
        {
            throw new InvalidOperationException("A reference cannot be created from a reference");
        }

        public override ExpressionBase ResolveReference(ReferenceIdentifier refID)
        {
            throw new Exception("A reference cannot be resolved from a reference");
        }
    }
}
