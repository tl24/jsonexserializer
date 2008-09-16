using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;

namespace JsonExSerializer.Framework.Visitors
{
    public class CollectReferencesVisitor : VisitorBase
    {
        private List<ReferenceExpression> _references = new List<ReferenceExpression>();

        /// <summary>
        /// The list of references that were collected
        /// </summary>
        public List<ReferenceExpression> References {
            get { return _references; }
        }

        public void Visit(ReferenceExpression reference) {
            References.Add(reference);
        }

        public void VisitComplex(ComplexExpressionBase ComplexExpression)
        {
            foreach (ExpressionBase expr in ComplexExpression.ConstructorArguments)
                Visit(expr);
        }

        public void Visit(ListExpression list)
        {
            VisitComplex(list);

            foreach (ExpressionBase item in list.Items)
                Visit(item);
        }

        public void Visit(ObjectExpression objExpr)
        {
            VisitComplex(objExpr);
            foreach (KeyValueExpression item in objExpr.Properties)
                Visit(item);
        }

        public void Visit(KeyValueExpression KeyValue)
        {
            Visit(KeyValue.ValueExpression);
        }

        public void Visit(CastExpression Cast)
        {
            Visit(Cast.Expression);
        }
    }
}
