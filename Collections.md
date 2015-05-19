Collections are pretty inconsistent in the .NET framework.  For more info on this see [here](http://blogs.msdn.com/madst/archive/2006/10/10/What-is-a-collection_3F00_.aspx).
The problem is you have ICollection, which is great for iterating over, but it doesn't have an Add method, so you can't deserialize the collection using the ICollection interface.  IList does have an add method, but not everything implements IList.  Classes like Stack, Queue don't implement IList.  .NET 2.0 brought us ICollection

&lt;T&gt;

 which does have add, but again not everything implements it.

The following collection types are handled automatically:
  1. Arrays of any type
  1. System.Collections.IList
  1. System.Collections.Generic.ICollection

&lt;T&gt;


  1. Anything implementing ICollection that also has a constructor that takes a single parameter of type ICollection, ICollection

&lt;T&gt;

, IEnumerable, or IEnumerable

&lt;T&gt;

.
  1. All collection types in System.Collections and System.Collections.Generic namespaces.

If your collection type does not fit into one of those cases, you can extend CollectionHandler and then register the handler with the SerializationContext.


Collection support is provided by CollectionHandler and ICollectionBuilder.
The CollectionHandler class provides type information for collections as well as providing a test method to indicate which classes are collections.  The CollectionHandler is also used to get a reference to an implementation of ICollectionBuilder which handles adding methods to the collection. The ICollectionBuilder interface also has GetResult and GetReference methods which returns a reference to the built collection.  Most of the time both of the methods should return the same object, however GetReference may be called at any time before the collection is fully filled, GetResult is always called after the collection is fully filled.  An exception should be thrown by GetReference if the collection cannot be returned until all items are filled.


Example:
```
public class MyCollectionHandler : CollectionHandler {
   public override bool IsCollection(Type collectionType) {
      if (collectionType == typeof(MyCollection)) {
          return true;
      }
      else 
      {
          return false;
      }
   }
   
   public override ICollectionBuilder ConstructBuilder(Type collectionType, int ItemCount) {
      return new MyCollectionBuilder();
   }

   // return the type of items contained in the collection
   public override Type GetItemType(Type collectionType) {
      return typeof(MyCollectionItem);
   }

   public override IEnumerable GetEnumerable(object collection) {
      // if your collection implements IEnumerable you can skip overriding this method
      return collection;
   }
}

public class MyCollectionBuilder : ICollectionBuilder {
   private MyCollection _instance;

   public MyCollectionBuilder() {
      _instance = new MyCollection();
   }

   public void Add(object item) {
      _instance.MyAdd(item);
   }

   public object GetResult() {
      return _instance;
   }

   public object GetReference() {
      return _instance;
   }
}

...
//Register the collection handler so its available to use by the Serializer
Serializer s = Serializer.GetSerializer(typeof(MyCollection));
s.Context.RegisterCollectionHandler(new MyCollectionHandler());

```