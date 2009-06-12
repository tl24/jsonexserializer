using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;

namespace JsonExSerializerTests.Mocks
{
    public class MockDefaultValues
    {
        private int intDefault;
        private int intCustomDefault = 10;
        private string stringDefaultDisabled;

        [JsonExDefault]
        public int IntDefault
        {
            get { return this.intDefault; }
            set { this.intDefault = value; }
        }

        [JsonExDefault(10)]
        public int IntCustomDefault
        {
            get { return this.intCustomDefault; }
            set { this.intCustomDefault = value; }
        }

        [JsonExDefault(DefaultValueSetting=DefaultValueOption.WriteAllValues)]
        public string StringDefaultDisabled
        {
            get { return this.stringDefaultDisabled; }
            set { this.stringDefaultDisabled = value; }
        }

        
    }
}
