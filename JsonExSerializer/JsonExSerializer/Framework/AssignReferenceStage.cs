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

        public AssignReferenceStage(SerializationContext context)
        {
        }

        public ExpressionBase Execute(ExpressionBase root)
        {
            IList<ReferenceExpression> references = CollectReferences(root);
            foreach (ReferenceExpression reference in references)
                ResolveReference(reference, root);
            return root;
        }

        private static List<ReferenceExpression> CollectReferences(ExpressionBase root)
        {
            CollectReferencesVisitor visitor = new CollectReferencesVisitor();
            root.Accept(visitor);
            return visitor.References;
        }

        private static void ResolveReference(ReferenceExpression reference, ExpressionBase root)
        {
            ReferenceVisitor visitor = new ReferenceVisitor(reference.Path);
            visitor.Visit(root);
            if (visitor.ReferencedExpression == null)
                throw new ParseException("Unable to resolve reference to " + reference.Path);
            reference.ReferencedExpression = visitor.ReferencedExpression;
        }

    }
}