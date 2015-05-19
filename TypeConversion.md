# Introduction #

During Serialization it may be necessary to convert one type to another that is more suitable for serialization.  The TypeConversion namespace contains classes for doing this. _Note: Only object types can use type conversion.  Type conversion for primitive types and strings is ignored.  However, the final converted type can be a primitive or string._

## IJsonTypeConverter Interface ##

The main interface for handling conversion is the **IJsonTypeConverter** interface.
The ConvertFrom and the ConvertTo methods are the two main methods in this interface.  The ConvertFrom method is called during serialization and should return the converted object that you want serialized.  The ConvertTo method is called upon deserialization and should transform the object back into its original form.

This can be used in situations where custom serialization is needed.  Just convert the object into another format that can be easily serialized.  A good example of this is just converting an object to a hashtable or other implementation of IDictionary.  The fields that you want to serialize with the object are just put as keys in the hashtable.

Here's an example, similar to one of the unit tests.  MyPoint is an immutable class with 2 fields x, y that can only be set via the constructor.  So normal serialization would not work in this case.  So the MyPoint class is converted to an int array of length 2 for serialization.  On Deserialization, the 2 ints in the array are passed to the constructor to recreate the object. Notice the JsonConvert attribute on the MyPoint class, this is one way of registering your type converter with the class.  (_Note: a future release will probably allow for deserializing without requiring no-arg constructors_).

```
    /// <summary>
    /// A point class to test type conversion, values can only
    /// be set through the constructor.
    /// </summary>
    [JsonConvert(typeof(MyPointConverter))]
    public class MyPoint
    {
        private int _x;
        private int _y;

        public MyPoint(int x, int y)
        {
            this._x = x;
            this._y = y;
        }

        public int X { get { return _x; } }

        public int Y { get { return _y; } }

    }
  
   /// <summary>
    /// A converter for MyPoint
    /// </summary>
    public class MyPointConverter : IJsonTypeConverter
    {

        // convert to an int array
        public object ConvertFrom(object item, SerializationContext serializationContext)
        {
            MyPoint pt = (MyPoint) item;
            return new int[] { pt.X, pt.Y };
        }

        // convert to MyPoint
        public object ConvertTo(object item, Type sourceType, SerializationContext serializationContext)
        {
            int[] data = (int[])item;
            return new MyPoint(int[0], int[1]); 
        }

        public object Context
        {
            set { throw new Exception("The method or operation is not implemented."); }
        }

        public Type GetSerializedType(Type sourceType)
        {
            return typeof(int[]);
        }

        public SerializationContext SerializationContext
        {
            set { return; }
        }

    }

```

## Registering your Converter ##
### JsonConvert Attribute ###
There are multiple ways of registering your converter to go with your class.  You've already seen one way, the JsonConvert attribute.  This attribute can also be applied to a property.  When applied to a property, the converter will override any converters defined for the property type.

### Implement IJsonTypeConverter interface ###
Another option is to implement the IJsonTypeConverter directly in the class that needs to be converted.  In this case it will just use the instance of your class to do the conversion.

### RegisterTypeConverter ###
In cases where you can't modify the serialized class or don't want to, you can register a converter through the Serialization Context.
```
    Serializer s = Serializer.GetSerializer(...);
    s.Context.RegisterTypeConverter(typeof(MyPoint), new MyPointConverter());
```
This also works for properties as well, you just need to register the converter with the PropertyInfo object from the System.Reflection namespace for the property to serialize.


### System.ComponentModel.TypeConverter ###
And finally, JsonEx will also use the .NET framework System.ComponentModel.TypeConverter if there is one available and it allows conversion to and from a string.
