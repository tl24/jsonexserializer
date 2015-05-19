# Summary
JsonExSerializer is a serializer/deserializer framework for C# that uses JSON format for its storage format.  
A small number of extensions are added to facilitate correct reconstruction of the object upon deserialization.  It is 
mainly intended to serialize objects to and from a storage medium, but it fully supports being used in an 
AJAX/web environment as well.  See the SetJsonStrictOptions on the extensions page for how to turn off
the extensions if transmitting to a browser. See the Tutorial and Usage pages for examples on how to use it.

# Why?
I wanted a framework that I could use to persist objects to disk.  I also wanted something that was human readable and easy 
to edit.  Binary serialization is not human readable, and I didn't want to use xml because I found it too verbose for
this purpose.  I examined a few other JSON packages, but found nothing that could recreate the object graph *exactly*
as it was deserialized.  Especially if normal OO techniques such as inheritance were used.  None of the 
implementations could handle references as well, which was pretty important to me.

# Features
  * Easy to use: Serialize an object in about 2-3 lines of code.
  * Deserialize objects exactly as you serialized them with [References] intact
  * Supports classes, structs, Generics, properties and public fields
  * Support for constructors with arguments
  * Serialization can be controlled programmatically, with .NET attributes, or using app.config
  * [TypeConversion Easy customization], just create a different object to serialize which could be a Dictionary, String, ArrayList or any other type that you choose
  * Formatted output by default so its easy to read, or you can serialize it compactly if you choose
  * Can be customized to use your own factories or dependency injection frameworks when constructing objects

# Issues
Bug reports can be submitted to the issue log.  Issues or questions about JsonExSerializer can be posted to the JsonExSerializer discussion group.

# News
  * 3.1.1.283 has just been released (Jan 24 2013). See ReleaseNotes for more details.  Contains minor debugging enhancements.
  * (Jan 24 2013) 4.0 release is in-process and will contain many new features to bring the codebase in-line with .NET 4.0 as well as other common frameworks.  See the issue list for details.

See the wiki for more [Usage] and explanation of features.


