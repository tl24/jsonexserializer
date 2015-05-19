# Introduction #

The JsonExSerializer has similar performance characteristics to the XmlSerializer and BinaryFormatter available with the .NET framework.

# Details #

The performance tests serialize an object graph consisting of about 100 objects, made up of a "Customer" object containing "Address" and "Order" collections.  Each "Order" in the "Orders" collection contains 0 or more "OrderItem" objects.  The results are shown below.

Each test is 2500 iterations.

| **Type** | **Serialization** | **Deserialization** |
|:---------|:------------------|:--------------------|
| JsonExSerializer | 9.385ms per iteration | 13.689ms per iteration |
| BinaryFormmatter | 5.069ms per iteration | 2.809ms per iteration |
| XMLSerializer | 2.966ms per iteration | 1.816ms per iteration |