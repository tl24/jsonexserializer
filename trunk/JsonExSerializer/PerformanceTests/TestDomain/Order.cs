using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceTests.TestDomain
{
    public class Order
    {
        private int _orderNumber;
        private decimal _orderAmount;
        private string _ccNumber;
        private DateTime _expireDate;
        private int _cvvNumber;

        private List<OrderItem> _orderItems = new List<OrderItem>();

        public decimal OrderAmount
        {
            get { return this._orderAmount; }
            set { this._orderAmount = value; }
        }

        public string CcNumber
        {
            get { return this._ccNumber; }
            set { this._ccNumber = value; }
        }

        public System.DateTime ExpireDate
        {
            get { return this._expireDate; }
            set { this._expireDate = value; }
        }

        public int CvvNumber
        {
            get { return this._cvvNumber; }
            set { this._cvvNumber = value; }
        }

        public System.Collections.Generic.List<PerformanceTests.TestDomain.OrderItem> OrderItems
        {
            get { return this._orderItems; }
            set { this._orderItems = value; }
        }

        public int OrderNumber
        {
            get { return this._orderNumber; }
            set { this._orderNumber = value; }
        }


    }
}
