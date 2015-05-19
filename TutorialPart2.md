# Introduction #

In Part1 of the Tutorial we covered the basic scenarios for using the serializer.  Now we'll cover some more advanced scenarios.  We will refer to the classes we're working with in the first part  So refer to the first part of the [Tutorial](Tutorial.md) if you need to.


# Details #

At the end of the first part of the Tutorial we suppressed the login information completely.  But let's say we want to store the UserName and retrieve the rest of the login information from the database when we deserialize.  For that we can use a converter.  We extend the JsonConverterBase which is an abstract class implementing IJsonTypeConverter interface.
```

  public class LoginConverter : JsonConverterBase {

      private UserStore _userStore;

      public LoginConverter(UserStore userStore) {
         this._userStore = userStore;
      }

      public Type GetSerializedType(Type sourceType)
      {
         return typeof(string);
      }

      public object ConvertFrom(object item, SerializationContext serializationContext)
      {
          ILogin login = (ILogin) item;
          return login.UserName;
      }

      public object ConvertTo(object item, Type sourceType, SerializationContext serializationContext)
      {
            string userName = (string)item;
            return _userStore.GetLogin(userName);
      }
  }
```

The GetSerializedType method tells the serializer what type of object we will be converting to.  In this case we are converting ILogin to string.  ConvertFrom is called during serialization.  We will take the ILogin instance and store the UserName in the file.  The ConvertTo is called during deserialization.  We will be passed back the user name string that we serialized.  We pass that into our UserStore object which retrieves the user information.

Now we will register the converter like this:
```
   Serializer serializer = new Serializer(typeof(Customer));
   LoginConverter converter = new LoginConverter(new UserStore());
   serializer.Context.RegisterTypeConverter(typeof(LoginInfo), converter);

   string jsonText = serializer.Serialize(customer);
```

We call the RegisterTypeConverter method and register the typeconverter for a type.  We must specify the concrete type for the LoginInfo class as it will not pick up an interface.  This should generate the following text:
```
/*
  Created by JsonExSerializer
  Assembly: Samples, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
  Type: Sample01.Customer
*/
{
    "Id":1,
    "FirstName":"Bob",
    "LastName":"Smith",
    "PhoneNumber":"(222)444-9987",
    "LoginInfo":"bob"
}
```

When we deserialize, we will look up user "bob" in the UserStore and return the Login info.

There is a second way to register the converter, via class and property attributes.  With this method however, our converter must have a parameter-less constructor.  In this example we would have to construct the UserStore object some other way.

```
   /// by class
   [JsonConvert(typeof(LoginConverter))]
   public class Login : ILogin {
      /* ... */
   }

   /// by property
   public class Customer {

     /* Other properties omitted ... */

     private ILogin _loginInfo;
     [JsonConvert(typeof(LoginConverter))]
     public ILogin LoginInfo
     {
         get { return this. _loginInfo; }
         set { this. _loginInfo = value; }
     }
   }
```