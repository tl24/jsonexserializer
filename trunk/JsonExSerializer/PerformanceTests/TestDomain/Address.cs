using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceTests.TestDomain
{
    public class Address
    {
        private string _streetAddress;
        private string _city;
        private string _state;
        private string _zipCode;
        private char _addressType;
        private bool _isPrimary;

        public string StreetAddress
        {
            get { return this._streetAddress; }
            set { this._streetAddress = value; }
        }

        public string City
        {
            get { return this._city; }
            set { this._city = value; }
        }

        public string State
        {
            get { return this._state; }
            set { this._state = value; }
        }

        public string ZipCode
        {
            get { return this._zipCode; }
            set { this._zipCode = value; }
        }

        public char AddressType
        {
            get { return this._addressType; }
            set { this._addressType = value; }
        }

        public bool IsPrimary
        {
            get { return this._isPrimary; }
            set { this._isPrimary = value; }
        }

    }
}
