using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer.Expression;
using JsonExSerializer.Framework;

namespace JsonExSerializerTests.Expression
{
    [TestFixture]
    public class ReferenceTests
    {
      
        [Test(Description="Resolving reference for this should return root")]
        public void RootObjectTest()
        {
            ObjectExpression rootAsObject = new ObjectExpression();
            ExpressionBase result = ResolveReference(rootAsObject, "this");

            Assert.AreSame(rootAsObject, result, "Resolve reference for this should return root");
        }

        [Test(Description = "Resolving reference for this should return root")]
        public void RootCollectionTest()
        {
            ListExpression rootAsList = new ListExpression();
            ExpressionBase result = ResolveReference(rootAsList, "this");

            Assert.AreSame(rootAsList, result, "Resolve reference for this should return root");
        }

        [Test(Description="Value types can't resolve references or be references")]
        [ExpectedException(typeof(Exception))]
        public void ValueTypeReferenceTest()
        {
            ValueExpression value = new ValueExpression("");
            ExpressionBase actual = ResolveReference(value, "this");
        }

        [Test(Description = "Reference types can't be references")]
        [ExpectedException(typeof(Exception))]
        public void ReferenceTypeReferenceTest()
        {
            ReferenceExpression refExp = new ReferenceExpression(new ReferenceIdentifier(""));
            ExpressionBase actual = ResolveReference(refExp, "this");
        }

        [Test]
        public void ResolveObjectObjectTest()
        {
            ObjectExpression root = new ObjectExpression();
            ObjectExpression child1 = new ObjectExpression();
            root.Add(new ValueExpression("child1"), child1);
            ObjectExpression child2 = new ObjectExpression();
            root.Add(new ValueExpression("child2"), child2);
            ExpressionBase actual = ResolveReference(root, "this.child2");
            Assert.AreSame(child2, actual, "this.child2 did not resolve correctly");
        }

        [Test]
        public void ResolveCollectionItemTest()
        {
            ListExpression root = new ListExpression();
            ObjectExpression child1 = new ObjectExpression();
            ObjectExpression child2 = new ObjectExpression();
            root.Add(child1);
            root.Add(child2);

            ExpressionBase actual = ResolveReference(root, "this.1");
            Assert.AreSame(child2, actual, "this.1 did not resolve correctly");
        }

        [Test]
        public void ResolveNestedObjectTest()
        {
            ObjectExpression root = new ObjectExpression();
            ObjectExpression child1 = new ObjectExpression();
            root.Add(new ValueExpression("child1"), child1);
            ObjectExpression child2 = new ObjectExpression();
            root.Add(new ValueExpression("child2"), child2);
            ObjectExpression childA = new ObjectExpression();
            child1.Add(new ValueExpression("childA"), childA);
            ObjectExpression childB = new ObjectExpression();
            child2.Add(new ValueExpression("childB"), childB);

            ExpressionBase actual = ResolveReference(root, "this.child2.childB");
            Assert.AreSame(childB, actual, "this.child2.childB did not resolve correctly");
        }

        [Test]
        public void ResolveNestedCollectionTest()
        {
            ListExpression root = new ListExpression();
            ListExpression child1 = new ListExpression();
            ListExpression child2 = new ListExpression();
            root.Add(child1);
            root.Add(child2);
            ObjectExpression childA = new ObjectExpression();
            child1.Add(childA);
            ObjectExpression childB = new ObjectExpression();
            child1.Add(childB);


            ExpressionBase actual = ResolveReference(root, "this.0.1");
            Assert.AreSame(childB, actual, "this.1.0 did not resolve correctly");
        }

        protected ExpressionBase ResolveReference(ExpressionBase Root, string reference)
        {
            ReferenceVisitor visitor = new ReferenceVisitor(new ReferenceIdentifier(reference));
            Root.Accept(visitor);
            return visitor.ReferencedExpression;
        }
    }
}
