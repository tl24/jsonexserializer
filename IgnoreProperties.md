# Introduction #

Occasionally you don't want certain properties to be serialized on your objects.  Any property that does not have both a getter and a setter is ignored by default.  For your other properties and public fields there are several ways of excluding them from serialization.


## [[JsonExIgnore](JsonExIgnore.md)] ##

The first way is to attach the JsonExIgnore attribute to your property or field.

```

namespace MyNamespace {
   public class MyClass {

      [JsonExIgnore]
      public int MyProperty {
        get { return _myProperty; }
        set { _myProperty = value; }
      }
   }
}

```

## SerializationContext ##

If you don't or can't modify your class, you can also ignore properties through the Serialization context.

```
   Serializer serializer = new Serializer(typeof(MyClass));
   serializer.Context.IgnoreProperty(typeof(MyClass), "MyProperty");
```

## Config file ##

The final way is through the app.config file.

```

<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="JsonExSerializer" type="JsonExSerializer.XmlConfigSection, JsonExSerializer"/>
  </configSections>
  <JsonExSerializer>
    <IgnoreProperties>
      <add type="MyNamespace.MyClass, MyAssembly" property="MyProperty" />
    </IgnoreProperties>
  </JsonExSerializer>
</configuration>

```