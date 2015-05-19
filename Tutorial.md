# Introduction #

This tutorial walks you through serializing a simple class.  First we'll start with a sample Customer class.  The source is shown here:

# Sample Class #
```
    public class Customer
    {
        private int _id;
        private string _firstName;
        private string _lastName;
        private string _phoneNumber;
        private List<Order> _orders;

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

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

        public string PhoneNumber
        {
            get { return this._phoneNumber; }
            set { this._phoneNumber = value; }
        }
    }
```

Next we'll populate the class with values and serialize it.  To do that we'll need to get an instance of a serializer for the Customer class, and call the Serialize method.
```
   // customer
   Customer customer = new Customer();
   customer.Id = 1;
   customer.FirstName = "Bob";
   customer.LastName = "Smith";
   customer.PhoneNumber = "(222)444-9987";

   // serialize to a string
   Serializer serializer = new Serializer(typeof(Customer));
   string jsonText = serializer.Serialize(customer);
   Console.WriteLine(jsonText);
```

The output should look something like this:
```
/*
  Created by JsonExSerializer
  Assembly: Samples, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
  Type: Sample01.Customer
*/
{
    "Id":1,
    "FirstName":"Bob",
    "LastName":"Smith",
    "PhoneNumber":"(222)444-9987"
}
```

To deserialize the customer object is just as easy.  We'll use the same serializer and the string that we wrote the previous result to to deserialize the class.
```
  Customer deserializedCustomer = (Customer) serializer.Deserialize(jsonText);
```

Next we'll add an orders collection to the customer object.  Collections are serialized as javascript arrays.  The Order class looks like this:
```
    public class Order
    {
        private decimal _amount;
        private string _orderNumber;
        private int _itemCount;

        public decimal Amount
        {
            get { return this._amount; }
            set { this._amount = value; }
        }

        public string OrderNumber
        {
            get { return this._orderNumber; }
            set { this._orderNumber = value; }
        }

        public int ItemCount
        {
            get { return this._itemCount; }
            set { this._itemCount = value; }
        }
    }
```

And we'll declare it in the customer object like this:

```
   public class Customer {

     /* Other properties omitted ... */

     private List<Order> _orders;
     public List<Order> Orders
     {
         get { return this._orders; }
         set { this._orders = value; }
     }
   }
```

Let's populate the Orders collection and add it to the Customer
```
    // orders
    Order order1 = new Order();
    order1.Amount = new decimal(54.99);
    order1.OrderNumber = "ORD123";
    order1.ItemCount = 3;

    Order order2 = new Order();
    order2.Amount = new decimal(99.99);
    order2.OrderNumber = "ORD235";
    order2.ItemCount = 10;

    List<Order> orders = new List<Order>();
    orders.Add(order1);
    orders.Add(order2);

    customer.Orders = orders;
```

Now the output should look like this:
```
/*
  Created by JsonExSerializer
  Assembly: Samples, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
  Type: Sample01.Customer
*/
{
    "Id":1,
    "FirstName":"Bob",
    "LastName":"Smith",
    "PhoneNumber":"(222)444-9987",
    "Orders":
    [
        {
            "Amount":54.99,
            "OrderNumber":"ORD123",
            "ItemCount":3
        },
        {
            "Amount":99.99,
            "OrderNumber":"ORD235",
            "ItemCount":10
        }
    ]
}

```

Now we realize that during processing we need to know the Customer's Login information so someone adds the Login info to the Customer class like so:
```
   public class Customer {

     /* Other properties omitted ... */

     private ILogin _loginInfo;
     public ILogin LoginInfo
     {
         get { return this. _loginInfo; }
         set { this. _loginInfo = value; }
     }
   }

   public interface ILogin {
      public string UserName { get; set; }
      public string Password { get; set; }
   }
```

Now suddenly we get this in serialized text:
```
    "Id":1,
    "FirstName":"Bob",
    "LastName":"Smith",
    "PhoneNumber":"(222)444-9987",
    "LoginInfo":
    {
        "UserName": "bob",
        "Password": "secret"
    }
    /* rest of the text */
```

We probably don't want the login information to be serialized.  So we add the JsonExIgnore attribute to suppress it.

```
   [JsonExIgnore]
     public ILogin LoginInfo
     {
         get { return this. _loginInfo; }
         set { this. _loginInfo = value; }
     }
```

Now the LoginInfo will no longer show up in the file.

There's the basics of using the Serializer.  In later installments we'll look at more advanced scenarios.