using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using JsonExSerializer;
using JsonExSerializerTests.Mocks;

namespace JsonExSerializerTests.Expressions
{
    [TestFixture]
    public class ExpressionHandlerTests
    {
        [RowTest]
        [Row("{ SimpleObject: 'foo' }", typeof(SemiComplexObject), Description="Object Mismatch")]
        [Row("{ DateValue: '12/10/2011' }", typeof(DateTime), Description="Date Mismatch")]
        [Row("{ NumericValue: 12 }", typeof(int), Description = "Numeric Mismatch")]
        [Row("{ BooleanValue: true }", typeof(bool), Description = "Boolean Mismatch")]
        [Row("'key:value'", typeof(Dictionary<string, string>), Description = "Dictionary Mismatch")]
        public void Expression_Mismatch(string json, Type serializedType)
        {
            // In each of these tests, the expression an handler is expecting
            // is not what it gets and we want to throw meaningful error messages
            // in that case
            Serializer s = new Serializer();
            try
            {
                object result = s.Deserialize(json, serializedType);
                Assert.Fail("InvalidOperationException not thrown");
            }
            catch (InvalidOperationException)
            {
                
            }
        }

    }
}
