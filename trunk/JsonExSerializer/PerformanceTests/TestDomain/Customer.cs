using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceTests.TestDomain
{
    public class Customer
    {
        private string _firstName;
        private string _lastName;
        private string _phone;
        private string _ssn;
        private int _age;

        private List<Address> _addresses = new List<Address>();
        private List<Order> _orders = new List<Order>();

        public string FirstName
        {
            get { return this._firstName; }
            set { this._firstName = value; }
        }

        public string LastName
        {
            get { return this._lastName; }
            set { this._lastName = value; }
        }

        public string Phone
        {
            get { return this._phone; }
            set { this._phone = value; }
        }

        public string Ssn
        {
            get { return this._ssn; }
            set { this._ssn = value; }
        }

        public int Age
        {
            get { return this._age; }
            set { this._age = value; }
        }

        public System.Collections.Generic.List<PerformanceTests.TestDomain.Address> Addresses
        {
            get { return this._addresses; }
            set { this._addresses = value; }
        }

        public System.Collections.Generic.List<PerformanceTests.TestDomain.Order> Orders
        {
            get { return this._orders; }
            set { this._orders = value; }
        }

    }
}
