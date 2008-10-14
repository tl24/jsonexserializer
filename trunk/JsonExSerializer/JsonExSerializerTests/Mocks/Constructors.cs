using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;

namespace JsonExSerializerTests.Mocks
{
    public class NamedOnlyExactConstructor
    {
        private string stringPropA;
        private string stringPropB;

        public NamedOnlyExactConstructor()
        {
        }

        public NamedOnlyExactConstructor(int StringPropA, int PropB)
        {
            this.stringPropA = StringPropA.ToString();
            this.stringPropB = PropB.ToString();
        }

        public NamedOnlyExactConstructor(string StringPropA, string PropB)
        {
            this.stringPropA = StringPropA;
            this.stringPropB = PropB;
        }

        [ConstructorParameter("PropB")]
        public string StringPropB
        {
            get { return this.stringPropB; }
        }

        [ConstructorParameter]
        public string StringPropA
        {
            get { return this.stringPropA; }
        }

    }

    public class NamedOnlyIgnoreCaseConstructor
    {
        private string stringPropA;
        private string stringPropB;

        public NamedOnlyIgnoreCaseConstructor(int STRINGPROPA, int propb)
        {
            this.stringPropA = STRINGPROPA.ToString();
            this.stringPropB = propb.ToString();
        }

        public NamedOnlyIgnoreCaseConstructor(string stringpropa, string PROPB)
        {
            this.stringPropA = stringpropa;
            this.stringPropB = PROPB;
        }

        [ConstructorParameter("propb")]
        public string StringPropB
        {
            get { return this.stringPropB; }
        }

        [ConstructorParameter("STRINGPROPA")]
        public string StringPropA
        {
            get { return this.stringPropA; }
        }
    }

    public class MixedExactConstructor : NamedOnlyExactConstructor
    {
        private int intProp;

        public MixedExactConstructor(string StringPropA, string PropB, int IntProp)
            : base(StringPropA, PropB)
        {
            this.intProp = IntProp;
        }

        [ConstructorParameter(2)]
        public int IntProp
        {
            get { return intProp; }
        }
    }
}
