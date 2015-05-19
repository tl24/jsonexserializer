# Introduction #

There are 3 options for handling references in your object graph that is being serialized, which is controlled by the ReferenceWritingType property on the SerializationContext.  The three options are:
  * ErrorCircularReferences
  * IgnoreCircularReferences
  * WriteIdentifier
Unless your option type is WriteIdentifier, any non-circular references are just copied, i.e. upon deserialization a ReferenceEquals test on the items will return false.

# Details #

### ErrorCircularReferences ###
This is the default option.  With this option set if any circular references are detected an exception will be thrown.  Non-circular references are copied.

### IgnoreCircularReferences ###
With this option, the first time the object is seen it will be serialized as normal, any subsequent references to the object will be written as null with no exception thrown.  Non-circular references are copied, same as with ErrorCircularReferences.

### WriteIdentifier ###
WriteIdentifier will keep references in tact and can handle circular references.  It does not use standard JSON syntax and will be turned off if [setJsonStrictOptions](Extensions.md) is called.  The first time an object instance is encountered it is serialized as normal.  Any subsequent times a reference identifier will be written with a path to the first occurrence in the object.  It looks like the same code you would use to reference the object, e.g. "`this.Customer.Orders[0].Address`".  There are some restrictions on its usage which will cause an exception.  A collection can not be referenced from within the collection and an object can not be referenced from inside itself it has a converter.  Any references outside the objects themselves are allowed.

Example:
```
// Not allowed, assuming the Item class exposes a property that references the array list
ArrayList a = new ArrayList();
a.Add(new Item(a));
```