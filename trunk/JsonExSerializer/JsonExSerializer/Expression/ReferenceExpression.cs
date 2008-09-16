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
        private object result;

        public ReferenceExpression(string ReferenceIdentifier)
        {
            this._refID = new ReferenceIdentifier(ReferenceIdentifier);
        }

        public ReferenceExpression(ReferenceIdentifier refID)
        {
            this._refID = refID;
        }

        public ReferenceIdentifier ReferenceIdentifier
        {
            get { return _refID; }
        }

        public ExpressionBase ReferencedExpression
        {
            get { return _reference; }
            set
            {
                if (_reference == value)
                    return;
                if (_reference != null)
                    throw new InvalidOperationException("Attempt to change referenced expression after its already been set");
                _reference = value;
                _reference.ObjectConstructed += new EventHandler<ObjectConstructedEventArgs>(Reference_ObjectConstructed);
            }
        }

        void Reference_ObjectConstructed(object sender, ObjectConstructedEventArgs e)
        {
            result = e.Result;
        }

        public override object Evaluate(SerializationContext context) {
            if (result == null)
                throw new InvalidOperationException("Attempt to reference " + ReferenceIdentifier + " before its constructed");
            return result;
        }
    }
}
