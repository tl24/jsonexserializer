# Introduction #

You can control what names are written out in serialization by setting the Alias for a property.


# Details #

Normally the property name is the name used in the JSON output when serializing.  However you can change this by setting the Alias for a property in a couple of different ways.

## JsonExProperty Attribute ##
One way is to apply the JsonExProperty attribute to the property and set the alias on the attribute.

```
public class BankAccount {
   [JsonExProperty("CustomerAccountBalance")]
   public decimal AccountBalance {
      get { return _acctBal; }
      set { _acctBal = value; }
   }
}
```

Resulting in this output:
```
 { "CustomerAccountBalance": 100.0 }
```

## IPropertyData.Alias ##
Another way is to set it through code by retrieving the property metadata and setting the Alias.
```
   Serializer s = new Serializer(typeof(BankAccount));
   TypeData accountTypeData = s.Config.TypeHandlerFactory[typeof(BankAccount)];
   IPropertyData acctBalProp = accountTypeData.FindPropertyByName("AccountBalance");
   acctBalProp.Alias = "CustomerAccountBalance";
```


See Also: NamingStrategy