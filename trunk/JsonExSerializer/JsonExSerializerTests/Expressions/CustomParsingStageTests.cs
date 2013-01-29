using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer.Framework.Parsing;
using JsonExSerializer.Framework.Visitors;
using JsonExSerializer.Framework.Expressions;
using JsonExSerializer;
using System.Collections;

namespace JsonExSerializerTests.Expressions
{
    [TestFixture]
    public class CustomParsingStageTests
    {

        [Test]
        public void TestCustomTypeResolving()
        {
            Serializer serializer = new Serializer();
            string json = @"{obj1: { type:'Type1', value: {A:1, B: 2} }, obj2: { type:'Type2', value: { X:12.34, S:'test'} } }";
            serializer.Settings.ParsingStages.Add(new CustomTypeResolver());
            Hashtable values = serializer.Deserialize<Hashtable>(json);
            object obj1 = ((Hashtable)values["obj1"])["value"];
            object obj2 = ((Hashtable)values["obj2"])["value"];
            Assert.IsNotNull(obj1, "object 1 not deserialized");
            Assert.IsNotNull(obj2, "object 2 not deserialized");
            Assert.IsInstanceOfType(typeof(Type1), obj1, "Incorrect type on object1");
            Assert.IsInstanceOfType(typeof(Type2), obj2, "Incorrect type on object2");
            Assert.AreEqual(1, ((Type1)obj1).A, "obj1.A");
            Assert.AreEqual(2, ((Type1)obj1).B, "obj1.B");
            Assert.AreEqual(12.34, ((Type2)obj2).X, "obj2.X");
            Assert.AreEqual("test", ((Type2)obj2).S, "obj2.S");
        }

        [Test]
        public void TestResolveConcreteTypeToInterfaceProperty()
        {
            Serializer serializer = new Serializer();
            string json = @"{ type:'Type1', value: {A:1, B: 2} }";
            serializer.Settings.ParsingStages.Add(new CustomTypeResolver());
            Message result = serializer.Deserialize<Message>(json);
            IType value = result.value;
            Assert.IsNotNull(value, "value not deserialized");
            Assert.IsInstanceOfType(typeof(Type1), value, "Incorrect type on value");
            Assert.AreEqual(1, ((Type1)value).A, "obj1.A");
            Assert.AreEqual(2, ((Type1)value).B, "obj1.B");
        }

        public class Message
        {
            public string type;
            public IType value;
        }

        public interface IType { }

        public class Type1 : IType
        {
            private int _a;
            private int _b;

            public Type1()
            {
            }

            public Type1(int a, int b)
            {
                this.A = a;
                this.B = b;
            }

            public int A
            {
                get { return this._a; }
                set { this._a = value; }
            }

            public int B
            {
                get { return this._b; }
                set { this._b = value; }
            }

        }

        public class Type2 : IType
        {
            private float _x;
            private string _s;

            public Type2()
            {
            }

            public Type2(float x, string s)
            {
                this.X = x;
                this.S = s;
            }

            public float X
            {
                get { return this._x; }
                set { this._x = value; }
            }

            public string S
            {
                get { return this._s; }
                set { this._s = value; }
            }
        }

        public class CustomTypeResolver : IParsingStage
        {
            public Expression Execute(Expression root)
            {
                CustomTypeResolverVisitor visitor = new CustomTypeResolverVisitor();
                root.Accept(visitor);
                return root;
            }
        }

        public class CustomTypeResolverVisitor : ExpressionWalkerVisitor
        {
            public override void OnObjectStart(ObjectExpression expression)
            {
                // inspect the "type" property if available and set the correct type on the "value"
                if (expression["type"] != null && expression["value"] != null)
                {
                    string typeIndc = ((ValueExpression)expression["type"]).StringValue;
                    Type newType = null;
                    if (typeIndc == "Type1")
                        newType = typeof(Type1);
                    else if (typeIndc == "Type2")
                        newType = typeof(Type2);
                    if (newType != null)
                        expression["value"] = new CastExpression(newType, expression["value"]);
                }
            }
        }
    }
}
