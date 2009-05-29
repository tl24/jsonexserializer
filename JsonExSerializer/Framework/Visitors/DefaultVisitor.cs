using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.Expressions;

namespace JsonExSerializer.Framework.Visitors
{
    /// <summary>
    /// Base adapter class for a visitor that walks the expression tree.  Subclasses
    /// can override only methods for expression types that they care about
    /// </summary>
    public abstract class ExpressionWalkerVisitor : VisitorBase, IVisitor
    {
        public virtual void Visit(ObjectExpression expression)
        {
            OnObjectStart(expression);
            Visit((ComplexExpressionBase)expression);
            foreach (KeyValueExpression kv in expression.Properties)
            {
                kv.Accept(this);
            }
            OnObjectEnd(expression);
        }

        public virtual void Visit(KeyValueExpression expression)
        {
            expression.KeyExpression.Accept(this);
            expression.ValueExpression.Accept(this);
        }

        public virtual void Visit(ComplexExpressionBase expression)
        {
            foreach (Expression expr in expression.ConstructorArguments)
            {
                expr.Accept(this);
            }
        }

        public virtual void Visit(ArrayExpression expression)
        {
            OnArrayStart(expression);
            Visit((ComplexExpressionBase)expression);
            foreach (Expression item in expression.Items)
                item.Accept(this);
            OnArrayEnd(expression);
        }

        public virtual void Visit(CastExpression expression)
        {
            expression.Expression.Accept(this);
        }

        public virtual void Visit(ReferenceExpression expression) { }

        public virtual void Visit(NumericExpression expression) {
            OnValue(expression);
        }

        public virtual void Visit(BooleanExpression expression)
        {
            OnValue(expression);
        }

        /// <summary>
        /// Visit a value expression.  NOTE: this method is not called for subclasses
        /// of ValueExpression.  To perform an operation on ValueExpression and all of its
        /// subclasses, override the OnValue method.
        /// </summary>
        /// <param name="expression">value expression</param>
        /// <seealso cref="OnValue"/>
        public virtual void Visit(ValueExpression expression)
        {
            OnValue(expression);
        }

        /// <summary>
        /// Called for ValueExpression and all subclasses
        /// </summary>
        /// <param name="expression"></param>
        public virtual void OnValue(ValueExpression expression)
        {
        }

        public virtual void Visit(NullExpression expression)
        {
        }

        /// <summary>
        /// Called on an object expression before the properties and constructor arguments are visited
        /// </summary>
        /// <param name="expression"></param>
        public virtual void OnObjectStart(ObjectExpression expression) { }

        /// <summary>
        /// Called on an object expression after the properties and constructor arguments are visited
        /// </summary>
        /// <param name="expression"></param>
        public virtual void OnObjectEnd(ObjectExpression expression) { }


        /// <summary>
        /// Called on an array expression before the items are visited
        /// </summary>
        /// <param name="expression"></param>
        public virtual void OnArrayStart(ArrayExpression expression) { }

        /// <summary>
        /// Called on an array expression after the items are visited
        /// </summary>
        /// <param name="expression"></param>
        public virtual void OnArrayEnd(ArrayExpression expression) { }

    }
}
