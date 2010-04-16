using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace JsonExSerializerTests.Mocks
{
    public class XmlIgnoreMock 
    {
        private string _name;
        private double _salary;

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        [XmlIgnore]
        public double Salary
        {
            get { return this._salary; }
            set { this._salary = value; }
        }

    }
}
