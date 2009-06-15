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
        private short convertedValue = 32;

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

        [JsonExDefault(32)]
        public short ConvertedValue
        {
            get { return this.convertedValue; }
            set { this.convertedValue = value; }
        }     
   

    }

    [JsonExDefaultValues(typeof(string), "")]
    [JsonExDefaultValues(typeof(short), 32)]
    public class MockDefaultValuesCascade
    {
        private string emptyString = "";
        private short convertedDefault = 32;

        public string EmptyString
        {
            get { return this.emptyString; }
            set { this.emptyString = value; }
        }

        public short ConvertedDefault
        {
            get { return this.convertedDefault; }
            set { this.convertedDefault = value; }
        }
    }
}
