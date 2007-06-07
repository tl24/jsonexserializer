using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;

namespace JsonExSerializerTests.Mocks
{

    [JsonConvert(typeof(string),Context="Name")]
    public class ConvertedObject
    {
    }
}
