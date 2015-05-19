### Usage: ###
To serialize and deserialize objects, first you must get an instance of a serializer.
```
  Serializer serializer = Serializer.GetSerializer(typeof(MyClass));
```

Then to serialize use the Serialize method, passing in your object to serialize.  You can either serialize to a string, or write the serialization to a TextWriter if you're serializing to a file or socket perhaps.

Serialize to String:
```
   MyClass myClass = new MyClass();
   ... // set myClass properties

   string result = serializer.Serialize(myClass);
```

Serialize to TextWriter:
```
   MyClass myClass = new MyClass();
   ... // set myClass properties
   
   TextWriter writer = new TextWriter(new FileStream("somefile.jsx", FileMode.Create));
   serializer.Serialize(myClass, writer);
   writer.Close();
```


Then to Deserialize, just reverse the process calling the Deserialize method.

Deserialize from String:
```
   string result = ... Populate the string with previously serialized content
   MyClass myClass = (MyClass) serializer.Deserialize(result);
```

Deserialize from TextReader:
```
   TextReader reader = new TextReader(new FileStream("somefile.jsx", FileMode.Open));
   MyClass myClass = (Myclass) serializer.Deserialize(reader);
   reader.Close();
```