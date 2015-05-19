# Introduction #

JsonExSerializer has added extensions to the JSON specification in order to facilitate using the JSON format for serialization.  These extensions are defined here.  These extensions can be suppressed when serializing by calling the SetJsonStrictOptions() method on the SerializationContext object.  In addition each extension may have its own option to suppress that extension.  These options can be accessed through the SerializationContext as well.

Example:
```
   Serializer serializer = new Serializer(typeof(MyClass));
   serializer.Context.SetJsonStrictOptions();
```


# Details #

## Comments ##
> The JsonExSerializer can output comments to help identify sections of the serialized text that is written out.  The JSON specification does not account for comments.  If the **OutputTypeComment** option is set to true, then a comment will be written before an object to indicate its type and indicate that the JsonExSerializer framework performed the serialization.

Example:
```
   Serializer serializer = new Serializer(typeof(int));
   serializer.Context.OutputTypeComment = true;  // on by default
   string result = serializer.Serialize(32);
```

Result:
```
/*
  Created by JsonExSerializer
  Assembly: mscorlib.dll
  Type: System.Int32
*/
32
```

## Type Casts ##
> If it is determined that the type of an object can not be sufficiently determined on deserialization based on the metadata of the object being serialized, then a cast will be written to the serialized stream to indicate the type upon deserialization.  This can occur when using interfaces, non-generic collections such as ArrayList or HashTable, or typing a property with a base class type, but the value of the property is a subclass of the base class.  The type information will either be a simple type reference like you would see in normal C# code, or quoted with an assembly reference.  The OutputTypeInformation option controls the writing of type information.

Example:
```
   ArrayList al = new ArrayList();
   al.Add("this string");
   al.Add(true);
   al.Add(324);
   al.Add(new Guid("4c14dd8c-2351-4bee-bda9-c33e61d64330"));
   Serializer serializer = new Serializer(typeof(ArrayList));
   serializer.Context.OutputTypeInformation = true;  // on by default
   string result = serializer.Serialize(al);
```

Result:
```
[
   (string) "this string",
   (bool) true,
   (int) 324,
   ("System.Guid, System") "4c14dd8c-2351-4bee-bda9-c33e61d64330"
]
```