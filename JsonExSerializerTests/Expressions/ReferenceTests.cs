using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer.Framework.Expressions;
using JsonExSerializer.Framework;
using JsonExSerializer.Framework.Visitors;

namespace JsonExSerializerTests.Expressions
{
    [TestFixture]
    public class ReferenceTests
    {
      
        [Test(Description="Resolving reference for this should return root")]
        public void RootObjectTest()
        {
            ObjectExpression rootAsObject = new ObjectExpression();
            Expression result = ResolveReference(rootAsObject, JsonPath.Root);

            Assert.AreSame(rootAsObject, result, "Resolve reference for this should return root");
        }

        [Test(Description = "Resolving reference for this should return root")]
        public void RootCollectionTest()
        {
            ArrayExpression rootAsList = new ArrayExpression();
            Expression result = ResolveReference(rootAsList, JsonPath.Root);

            Assert.AreSame(rootAsList, result, "Resolve reference for this should return root");
        }

        [Test(Description="Value types can't resolve references or be references")]
        [ExpectedException(typeof(Exception))]
        public void ValueTypeReferenceTest()
        {
            ValueExpression value = new ValueExpression("");
            Expression actual = ResolveReference(value, JsonPath.Root);
        }

        [Test(Description = "Reference types can't be references")]
        [ExpectedException(typeof(Exception))]
        public void ReferenceTypeReferenceTest()
        {
            ReferenceExpression refExp = new ReferenceExpression(new JsonPath(""));
            Expression actual = ResolveReference(refExp, JsonPath.Root);
        }

        [Test]
        public void ResolveObjectObjectTest()
        {
            ObjectExpression root = new ObjectExpression();
            ObjectExpression child1 = new ObjectExpression();
            root.Add(new ValueExpression("child1"), child1);
            ObjectExpression child2 = new ObjectExpression();
            root.Add(new ValueExpression("child2"), child2);
            Expression actual = ResolveReference(root, "$['child2']");
            Assert.AreSame(child2, actual, "this.child2 did not resolve correctly");
        }

        [Test]
        public void ResolveCollectionItemTest()
        {
            ArrayExpression root = new ArrayExpression();
            ObjectExpression child1 = new ObjectExpression();
            ObjectExpression child2 = new ObjectExpression();
            root.Add(child1);
            root.Add(child2);

            Expression actual = ResolveReference(root, "$[1]");
            Assert.AreSame(child2, actual, "$[1] did not resolve correctly");
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

            Expression actual = ResolveReference(root, "$['child2']['childB']");
            Assert.AreSame(childB, actual, "$['child2']['childB'] did not resolve correctly");
        }

        [Test]
        public void ResolveNestedCollectionTest()
        {
            ArrayExpression root = new ArrayExpression();
            ArrayExpression child1 = new ArrayExpression();
            ArrayExpression child2 = new ArrayExpression();
            root.Add(child1);
            root.Add(child2);
            ObjectExpression childA = new ObjectExpression();
            child1.Add(childA);
            ObjectExpression childB = new ObjectExpression();
            child1.Add(childB);


            Expression actual = ResolveReference(root, "$[0][1]");
            Assert.AreSame(childB, actual, "$[0][1] did not resolve correctly");
        }

        protected Expression ResolveReference(Expression Root, string reference)
        {
            ReferenceVisitor visitor = new ReferenceVisitor(new JsonPath(reference));
            Root.Accept(visitor);
            return visitor.ReferencedExpression;
        }
    }
}
