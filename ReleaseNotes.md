## Release 3.1.1.283  (Jan 24 2013) ##
  * [Issue 61](https://code.google.com/p/jsonexserializer/issues/detail?id=61) - Enhancement: Better error messages for expression to expression handler mismatch
  * [Issue 62](https://code.google.com/p/jsonexserializer/issues/detail?id=62) - Enhancement: Better error messages including line number and position for parsing errors
## Release 3.1.1.279  (Jun 9 2011) ##
  * [Issue 58](https://code.google.com/p/jsonexserializer/issues/detail?id=58) - Bug: Nullable doubles and floats do not serialize correctly in non-english locales
  * [Issue 57](https://code.google.com/p/jsonexserializer/issues/detail?id=57) - Bug: Decimal types do not serialize correctly in non-english locales
## Release 3.1.0.269  (May 8 2010) ##
  * [Issue 55](https://code.google.com/p/jsonexserializer/issues/detail?id=55) - Bug: Having multiple complex nodes (IgnoredProperties,TypeConverters,etc) was causing an exception
## Release 3.1.0.267  (Apr 15 2010) ##
  * [Issue 23](https://code.google.com/p/jsonexserializer/issues/detail?id=23) - Enhancement: Option to omit properties that have the default value
  * [Issue 36](https://code.google.com/p/jsonexserializer/issues/detail?id=36) - Bug: Properties with private setters are not defaulted to Ignored
  * [Issue 38](https://code.google.com/p/jsonexserializer/issues/detail?id=38) - Bug: ExpressionHandlersCollection doesn't honor IContextAware when setting default or null handler properties
  * [Issue 39](https://code.google.com/p/jsonexserializer/issues/detail?id=39) - Enhancement: Usability enhancements to ExpressionHandlersCollection
  * [Issue 40](https://code.google.com/p/jsonexserializer/issues/detail?id=40) - Enhancement: Properties where the actual property type is System.Type are handled now
  * [Issue 41](https://code.google.com/p/jsonexserializer/issues/detail?id=41) - Enhancement: Multiple instances of the same System.Type no longer written as references
  * [Issue 42](https://code.google.com/p/jsonexserializer/issues/detail?id=42) - Enhancement: Attribute service.  Custom actions can be applied by decorating properties/classes with attributes.
  * [Issue 44](https://code.google.com/p/jsonexserializer/issues/detail?id=44) - Bug: Property converters not used when placed on a constructor parameter
  * [Issue 49](https://code.google.com/p/jsonexserializer/issues/detail?id=49) - Enhancement: Property names in the json text can be customized using a strategy.
  * [Issue 50](https://code.google.com/p/jsonexserializer/issues/detail?id=50) - Enhancement: You can now deserialize into an existing object
  * [Issue 51](https://code.google.com/p/jsonexserializer/issues/detail?id=51) - Bug: Fixed issue when aliasing an open generic type
  * [Issue 52](https://code.google.com/p/jsonexserializer/issues/detail?id=52) - Bug: Fixed issue where the type info was ignored when using the new operator in the Json text
  * [Issue 54](https://code.google.com/p/jsonexserializer/issues/detail?id=54) - Enhancement: Extra properties within the json text that don't exist on the object can now be ignored. See MissingProperties.
## Release 3.0  (Jan 25 2009) ##
  * [Issue 24](https://code.google.com/p/jsonexserializer/issues/detail?id=24) - Bug: Something wrong in DictionaryToListConverter
  * [Issue 25](https://code.google.com/p/jsonexserializer/issues/detail?id=25) - Enhancement: Better Support for constructor-less classes
  * [Issue 28](https://code.google.com/p/jsonexserializer/issues/detail?id=28) - Enhancement: Make GetTypeBinding and GetTypeAlias virtual
  * [Issue 29](https://code.google.com/p/jsonexserializer/issues/detail?id=29) - Enhancement: Reference syntax now uses a subset of [JsonPATH](http://goessner.net/articles/JsonPath/). e.g. $['foo']['bar']
  * [Issue 30](https://code.google.com/p/jsonexserializer/issues/detail?id=30) - Enhancement: Rewrote much of the internals to allow for more customization
  * [Issue 32](https://code.google.com/p/jsonexserializer/issues/detail?id=32) - Bug: Nullable values serialized with casting prefix unnecessarily
  * [Issue 33](https://code.google.com/p/jsonexserializer/issues/detail?id=33) - Bug: Serializing dates depends on current culture
  * [Issue 34](https://code.google.com/p/jsonexserializer/issues/detail?id=34) - Bug: Cast prefix on references
  * [Issue 35](https://code.google.com/p/jsonexserializer/issues/detail?id=35) - Enhancement: Metadata for properties is now shared between base class and child classes, mimicking how reflection works
  * [Issue 37](https://code.google.com/p/jsonexserializer/issues/detail?id=37) - Bug: Deserialisation of Double is CultureSpecific
  * Assembly is now strongly-named
  * Renamed classes and restructured framework-level code for better clarity and separation

## Release 2.1.1  (Jul 19 2008) ##
  * [Issue 16](https://code.google.com/p/jsonexserializer/issues/detail?id=16) - Enhancement: Allow you to insert CollectionHandlers before the end of the list
  * [Issue 18](https://code.google.com/p/jsonexserializer/issues/detail?id=18) - Bug: Boolean properties serialized with Proper Case (True/False)
  * Fixed some internal issues around finding ignored properties

## Release 2.1   (Jul 02 2008) ##
  * [Issue 14](https://code.google.com/p/jsonexserializer/issues/detail?id=14) - Bug: Wrong constructor used in certain circumstances
  * [Issue 15](https://code.google.com/p/jsonexserializer/issues/detail?id=15) - Bug: Validation prevented serializing a property with no setter


## Release 2.0   (Feb 15 2008) ##
  * Major performance improvements
  * Support for constructor arguments
  * Support for public fields
  * Added Configuration via config file

## Release 1.0   (Aug 08 2007) ##
  * Initial Release