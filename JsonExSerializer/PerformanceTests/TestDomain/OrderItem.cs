using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceTests.TestDomain
{
    public class OrderItem
    {
        private double _quantity;
        private decimal _price;
        private string _itemNumber;
        private string _description;

        public double Quantity
        {
            get { return this._quantity; }
            set { this._quantity = value; }
        }

        public decimal Price
        {
            get { return this._price; }
            set { this._price = value; }
        }

        public string ItemNumber
        {
            get { return this._itemNumber; }
            set { this._itemNumber = value; }
        }

        public string Description
        {
            get { return this._description; }
            set { this._description = value; }
        }
    }
}
