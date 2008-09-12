using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.Visitors;

namespace JsonExSerializer.Expression
{
    public class ReferenceVisitor : VisitorBase
    {
        ReferenceIdentifier _refID;
        ExpressionBase _expr;

        public ReferenceVisitor(ReferenceIdentifier RefID)
        {
            _refID = RefID;
        }

        public ExpressionBase ReferencedExpression
        {
            get { return _expr; }
        }

        public void VisitObject(ObjectExpression expression)
        {
            Visit(typeof(ComplexExpressionBase), expression);
            if (_expr != null)
                return; // found it

            string key = _refID.Current;
            foreach (KeyValueExpression exp in expression.Properties)
            {
                if (exp.Key == key)
                {
                    Visit(exp);
                    return;
                }
            }
            // if we get here we didn't find it
            throw new Exception("Unable to resolve reference: " + _refID);

        }

        public void VisitKeyValue(KeyValueExpression keyValue)
        {
            if (_refID.Current == keyValue.Key)
            {
                Visit(keyValue.ValueExpression);
            }
            else
            {
                throw new Exception("Unable to resolve reference: " + _refID);
            }
        }

        /// <summary>
        /// Resolve a reference to an item within the collection
        /// </summary>
        /// <param name="refID">the reference to resolve</param>
        /// <returns>the referenced expression</returns>
        public void VisitListExpression(ListExpression expression)
        {
            Visit(typeof(ComplexExpressionBase), expression);
            if (_expr != null)
                return; // found it

            int index = _refID.CurrentIndex;
            if (index < 0 || index >= expression.Items.Count)
            {
                throw new ArgumentOutOfRangeException("Reference to collection item out of range: " + _refID);
            }
            ExpressionBase expr = expression.Items[index];
            Visit(expr);
        }

        public void VisitComplexBase(ComplexExpressionBase expression)
        {
            if (_refID.Current == "this")
            {
                if (expression.Parent != null)
                {
                    throw new ArgumentException("Reference for this passed to object that is not at the root", "refID");
                }
            }
            else
            {
                // have to assume that the parent checked that we were the right reference
                // should only get here if we have a parent, if no parent we're not valid
                if (expression.Parent == null)
                    throw new ArgumentException("Invalid reference", "refID");
            }
            // it is this object, check if we need to go further
            _refID = _refID.ChildReference();
            if (_refID.IsEmpty) {
                _expr = expression;
            }
        }

        protected override void VisitorNotFound(Type t, object o)
        {
           throw new ArgumentException("Invalid reference, no handler found for " + o.GetType(), "refID");
        }
    }
}
