using System;
using System.Collections;
using System.Collections.Generic;
using PerformanceTests.TestDomain;

namespace PerformanceTests
{
    public class TestObjectFactory
    {
        private int _maxObjectCount;
        private int _objectCount;
        private RandomGenerator _testDataGen;
        public TestObjectFactory(int maxObjectCount)
        {
            _maxObjectCount = maxObjectCount;
            _testDataGen = new RandomGenerator();
        }

        public object ProduceObjects()
        {
            _objectCount = 0;
            return ProduceCustomer();
        }

        private object ProduceCustomer()
        {
            Customer customer = new Customer();
            _objectCount++;
            customer.FirstName = _testDataGen.RandomWord();
            customer.LastName = _testDataGen.RandomWord();
            customer.Age = _testDataGen.RandomInt(1, 100);
            customer.Phone = _testDataGen.RandomInt(1111111111, 1888888888).ToString();
            customer.Ssn = _testDataGen.RandomInt(111111111, 999999999).ToString();
            
            customer.Addresses.Add(ProduceAddress());
            customer.Addresses[0].AddressType = 'B';
            customer.Addresses[0].IsPrimary = true;
            customer.Addresses.Add(ProduceAddress());
            customer.Addresses[1].AddressType = 'S';
            
            while (_objectCount < _maxObjectCount)
            {
                Order ord = ProduceOrder();
                customer.Orders.Add(ord);
            }
              
            return customer;
        }

        private Order ProduceOrder()
        {
            Order ord = new Order();
            _objectCount++;
            ord.OrderAmount = (decimal)(_testDataGen.RandomInt(1, 1500) * .99);
            ord.OrderNumber = _objectCount;
            ord.CcNumber = _testDataGen.RandomInt(1111111111, 1999999999).ToString();
            ord.CvvNumber = _testDataGen.RandomInt(111, 999);
            int itemsLeft = _maxObjectCount - _objectCount;
            if (itemsLeft > 0) {
                int itemCount = _testDataGen.RandomInt(1, Math.Min(itemsLeft, _maxObjectCount / 2));
                for (int i = 0; i < itemCount; i++)
                {
                    ord.OrderItems.Add(ProduceOrderItem());
                }
            }
            return ord;
        }

        private OrderItem ProduceOrderItem()
        {
            OrderItem oi = new OrderItem();
            _objectCount++;
            oi.ItemNumber = "SKU" + _testDataGen.RandomInt(100, 2000);
            oi.Price = (decimal)(_testDataGen.RandomInt(1, 1000) / (double)_testDataGen.RandomInt(1, 100));
            oi.Quantity = _testDataGen.RandomInt(1, 50);
            oi.Description = _testDataGen.RandomWord(20);
            return oi;
        }

        private Address ProduceAddress()
        {
            Address addr = new Address();
            _objectCount++;
            addr.StreetAddress = _testDataGen.RandomInt(1, 900) + " " + _testDataGen.RandomWord(2);
            addr.City = _testDataGen.RandomWord();
            addr.State = "YY";
            addr.ZipCode = _testDataGen.RandomInt(11111, 99999).ToString();
            return addr;
        }

    }

}
