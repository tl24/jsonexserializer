using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Expression;
using JsonExSerializer.Framework.Visitors;

namespace JsonExSerializer.Framework
{
    /// <summary>
    /// Resolves references to other expressions
    /// </summary>
    public class AssignReferenceStage : IParsingStage
    {
        SerializationContext _context;

        public AssignReferenceStage(SerializationContext Context)
        {
            _context = Context;
        }

        public ExpressionBase Execute(ExpressionBase Root)
        {
            IList<ReferenceExpression> references = CollectReferences(Root);
            foreach (ReferenceExpression reference in references)
                ResolveReference(reference, Root);
            return Root;
        }

        private List<ReferenceExpression> CollectReferences(ExpressionBase Root)
        {
            CollectReferencesVisitor visitor = new CollectReferencesVisitor();
            Root.Accept(visitor);
            return visitor.References;
        }

        private void ResolveReference(ReferenceExpression reference, ExpressionBase Root)
        {
            ReferenceVisitor visitor = new ReferenceVisitor(reference.ReferenceIdentifier);
            visitor.Visit(Root);
            if (visitor.ReferencedExpression == null)
                throw new Exception("Unable to resolve reference to " + reference.ReferenceIdentifier);
            reference.ReferencedExpression = visitor.ReferencedExpression;
        }

    }
}
