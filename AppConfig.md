# Introduction #

You may setup commonly used settings for the serializer in your app.config or web.config file.  The various settings in the Serializer config section all correspond to properties or methods on the Context property of the Serializer.


# Details #

Declare the section handler in the configSections like this:
```
  <configSections>
    <section name="JsonExSerializer" type="JsonExSerializer.XmlConfigSection, JsonExSerializer"/>
  </configSections>
```

By default, when creating the Serializer it will look for a config section named "JsonExSerializer" for any default settings.  You can also pass in the name of a section to use a different section.
```
   // uses default JsonExSerializer section if it exists
   Serializer s = new Serializer(typeof(MyClass));  

   // uses the MyClassConfig section of the config file
   Serializer s = new Serializer(typeof(MyClass), "MyClassConfig");
```

The following options can be set in the config file:
| **Item** | **Description** | **Values** |
|:---------|:----------------|:-----------|
| IsCompact | Turns indenting on/off | true / false |
| OutputTypeComment | Puts a comment before the serialized text | true / false |
| OutputTypeInformation | Turns off printing of type information during Serialization | true / false |
| ReferenceWritingType | Controls how between objects are handled | WriteIdentifier / IgnoreCircularReferences / ErrorCircularReferences |
| TypeBindings | Add/Remove type bindings | See below  |
| TypeConverters | Configure Type Converters | See below  |
| CollectionHandlers | Configure Collection handlers | See below  |
| IgnoreProperties | Ignore properties for serialization | See below  |

### Simple Options ###
These settings will turn off all non-standard JSON extensions.

```
  <JsonExSerializer>
    <IsCompact>false</IsCompact>
    <OutputTypeComment>false</OutputTypeComment>
    <OutputTypeInformation>false</OutputTypeInformation>
    <ReferenceWritingType>ErrorCircularReferences</ReferenceWritingType>
  </JsonExSerializer>
```

### TypeBindings ###
TypeBindings are only relevant when OutputTypeInformation is turned on.  They shorten the full type information down to an alias.  In the TypeBindings section you may add, remove, or clear all type bindings.

```
  <JsonExSerializer>
     <TypeBindings>
        <!-- clears all default bindings (Not recommended) -->
        <clear />
        <add alias="int" type="System.Int32, mscorlib" />
        <!-- remove by Type -->
        <remove type="System.Int32, mscorlib" />
        <!-- remove by Alias -->
        <remove alias="float" />
     </TypeBindings>
  </JsonExSerializer>
```

### TypeConverters ###
You can add type converters in the TypeConverters section.
```
  <JsonExSerializer>
    <TypeConverters>
      <!-- converter for type -->
      <add type="JsonExSerializerTests.Mocks.SimpleObject, JsonExSerializerTests" converter="JsonExSerializerTests.Mocks.SelfConverter, JsonExSerializerTests" />
      <!-- converter for a property on a type -->
      <add type="JsonExSerializerTests.Mocks.SimpleObject, JsonExSerializerTests" property="BoolValue" converter="JsonExSerializerTests.Mocks.BoolToIntConverter, JsonExSerializerTests" />
    </TypeConverters>
  </JsonExSerializer>
```

### [CollectionHandlers](Collections.md) ###
[CollectionHandlers](Collections.md) are added in the CollectionHandlers section.
```
  <JsonExSerializer>
    <CollectionHandlers>
      <add type="JsonExSerializerTests.Mocks.MockCollectionHandler, JsonExSerializerTests" />
    </CollectionHandlers>
  </JsonExSerializer>
```