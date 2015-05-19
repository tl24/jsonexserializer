# Introduction #

Naming strategies allow you to customize the names of all properties when serializing by using a class to generate the output name or alias from the property name.


# Details #

Naming strategies can be useful when you need to interface with other teams or companies outside your own that may use different naming conventions.  For example, let's say you use standard C# naming conventions for properties (PascalCase).  You might have a class file like this:

```
public class BankingAccount {

    public decimal AccountBalance {
       get { return _acctBal; }
       set { _acctBal = value; }
    }

    public decimal InterestRate {
       get { return _intRate; }
       set { _intRate = value; }
    }
}
```

You need to interface with a Mainframe that has established naming conventions using all caps with underscores (e.g. ACCOUNT\_BALANCE).  Rather than change your property names to match what they expect, you can apply a naming strategy to do this.  There are several strategies that come with JsonExSerializer, one of them being the Underscore naming strategy.  It can be applied like this:

```

   Serializer s = new Serializer(typeof(BankingAccount));
   s.Config.TypeHandlerFactory.SetPropertyNamingStrategy(new UnderscoreNamingStrategy(UnderscoreNamingStrategy.UnderscoreCaseStyle.UpperCase));
```

When serializing, the output would then look like this:
```
  { "ACCOUNT_BALANCE": 100.0,
    "INTEREST_RATE": 1.5
  }
```

You can use one of the default strategies, or you can build your own.

# Available Strategies #
The existing strategies are located in the JsonExSerializer.MetaData namespace.

### DefaultPropertyNamingStrategy ###
This is the default if you do not specify a naming strategy.  It returns what is passed in to it unchanged.

### PascalCaseNamingStrategy ###
PascalCase is the recommended naming convention for .NET properties, classes, and interfaces, so if you are already naming your properties this way you won't see much difference with this strategy.  Example: AccountBalance.

### CamelCaseNamingStrategy ###
CamelCase lower-cases the first word in the name and capitalizes the first letter of each subsequent word.  This is the typical naming convention in .NET for method parameters and local variables.  It is also the recommended naming convention for method names in Java. Example: accountBalance

### UnderscoreNamingStrategy ###
The underscore naming strategy separates words using the underscore character "_".  There a few different styles to determine the case of individual words.  The default behavior if not specified is to use the existing case.  Here are the different styles and their effects:
  * OriginalCase - leaves case unchanged. e.g. AccountBalance => Acccount\_Balance
  * MixedCase - uppercases the first letter in each word. e.g. accountBalance =>  Account\_Balance
  * LowerCase - lowercases all words. e.g. AccountBalance => account\_balance
  * UpperCase - uppercases all words. e.g. AccountBalance => ACCOUNT\_BALANCE_

### DelegateNamingStrategy ###
This strategy takes a delegate to implement the strategy in the event that you don't want to use a full blown class for your naming strategy.  The delegate type is System.Converter<string, string>. Example:
```
  // 2.0 anonymous delegate syntax
  new DelegateNamingStrategy(delegate(string old) { return "json_" + old; });

  // 3.5 lambda syntax
  new DelegateNamingStrategy((x) => ( "json_" + x; ));
```

### CustomNamingStrategyBase ###
You can extend this class to create your own custom strategy.  It provides a helper method called GetNameParts that will split a name up into list of strings.  The split logic depends on mixed case and/or underscores to successfully delimit words in the name.