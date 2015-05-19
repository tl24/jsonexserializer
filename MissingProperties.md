When your Json text contains properties that do not exist on your class, you can set the MissingPropertyAction to determine how to handle this.

The code for that is
```
  Serializer s = new Serializer(typeof(MyClass));
  s.Config.MissingPropertyAction = MissingPropertyOptions.Ignore;
```

Here is an example to demonstrate:

**Class object**
```
public class Foo {
   public int ID;
}
```
**Json Text**
```
{
   ID: 1,
   Balance: 10.00
}
```

In this situation, the Balance property in the Json text does not exist on the Foo class.

## Ignore ##

This is the default option, missing properties are ignored.

## ThrowException ##

With this option if a missing property is encountered an exception is thrown.