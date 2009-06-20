using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializerTests.Mocks;
using JsonExSerializer;
using JsonExSerializer.Framework.Expressions;
using JsonExSerializer.Framework;
using JsonExSerializer.Framework.Parsing;

namespace JsonExSerializerTests
{    
    [TestFixture]
    public class SerializeCtorTests
    {

        [Test]
        public void SimpleConstructorNoInitTest()
        {
            MyPointConstructor pt = new MyPointConstructor(3, 9);
            Serializer s = new Serializer(pt.GetType());
            string result = s.Serialize(pt);
            MyPointConstructor actual = (MyPointConstructor)s.Deserialize(result);
            Assert.AreEqual(pt, actual, "Simple Constructor with no initializer failed");
        }

        [Test]
        public void Deserialize_When2CtorsWithSameArgCount_EvaluateCorrectOne()
        {
            Serializer s = new Serializer(typeof(CtorMock));
            ObjectExpression expr = new ObjectExpression();
            expr.ResultType = typeof(CtorMock);
            NumericExpression IDExpr = new NumericExpression("10");
            ValueExpression StrExpr = new ValueExpression("name");
            expr.ConstructorArguments.Add(IDExpr);
            expr.ConstructorArguments.Add(StrExpr);
            CtorArgTypeResolver resolver = new CtorArgTypeResolver(expr,s.Config);
            Type[] argTypes = resolver.ResolveTypes();            
            CollectionAssert.AreElementsEqual(new Type[] { typeof(int), typeof(string) }, argTypes);
        }

        [Test]
        public void Deserialize_UseCorrectTypes_WhenParametersDefined()
        {
            Serializer s = new Serializer(typeof(CtorMock2));
            ObjectExpression expr = new ObjectExpression();
            expr.ResultType = typeof(CtorMock2);
            NumericExpression IDExpr = new NumericExpression("10");
            ObjectExpression objExpr = new ObjectExpression();
            objExpr.ConstructorArguments.Add(new ValueExpression("name"));
            expr.ConstructorArguments.Add(IDExpr);
            expr.ConstructorArguments.Add(objExpr);
            Type[] definedTypes = new Type[] { typeof(int), typeof(MyObject2) };
            CtorArgTypeResolver resolver = new CtorArgTypeResolver(expr, s.Config, definedTypes);
            Type[] argTypes = resolver.ResolveTypes();
            CollectionAssert.AreElementsEqual(new Type[] { typeof(long), typeof(MyObject2) }, argTypes);

            // Try to construct
            IDExpr.ResultType = typeof(long);
            objExpr.ResultType = typeof(MyObject2);
            Evaluator eval = new Evaluator(s.Config);
            object result = eval.Evaluate(expr);

            
        }
    }

    public class MyObject {
        private string _name;
        public MyObject(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }

    public class MyObject2 : MyObject
    {
        public MyObject2(string name)
            : base(name)
        {
        }

    }

    public class CtorMock
    {
        private int _id;
        private MyObject _objectName;

        public CtorMock(int id, MyObject objectName)
        {
            _id = id;
            _objectName = objectName;
        }

        public CtorMock(int id, string objectName) :
            this(id, new MyObject(objectName))
        {
        }

        public virtual int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public virtual JsonExSerializerTests.MyObject ObjectName
        {
            get { return this._objectName; }
            set { this._objectName = value; }
        }
    }

    public class CtorMock2 : CtorMock
    {
        public CtorMock2(long id, MyObject objectName)
            : base((int) id, objectName)
        {
        }

        public CtorMock2(long id, string objectName)
            :
            base((int) id, objectName)
        {
        }

        [ConstructorParameter(0)]
        public int IntID
        {
            get { return Id; }
        }

        [ConstructorParameter(1)]
        public MyObject2 ObjectName2
        {
            get { return (MyObject2) base.ObjectName; }
            set { base.ObjectName = value; }
        }
    }
}
