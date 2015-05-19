# Introduction #

An Expression object is an in memory representation of the JSON structure.  The Expression classes reside in the JsonExSerializer.Framework.Expressions namespace.  An object graph is converted into an Expression during serialization before being written to the stream.  JSON text is also parsed into an Expression during deserialization before being converted back into an object instance.  This allows possibilities for customizing serialization or deserialization without having to know about parsing or writing JSON text.  The different expression classes represent the available JSON constructs.

# Expression Types #
## Expression ##
This is the base class for all expressions.  It contains common properties for expressions such as Parent, DefaultType, and ResultType.

| **Property or Method** | **Description** |
|:-----------------------|:----------------|
| Parent                 | Returns the parent of the expression if it has one |
| DefaultType            | Returns the type to be used if no ResultType is set |
| ResultType             | The corresponding type to be used when deserializing the expression |

## ObjectExpression ##
The ObjectExpression class represents a JSON object.
{ "key": "value" }

> The ObjectExpression contains a Properties collection of KeyValueExpression object which represent the key-value pairs that is has.

| **Property or Method** | **Description** |
|:-----------------------|:----------------|
| Item[string](string.md) | Retrieve the value expression for a given key |
| Properties             | List of KeyValueExpression |
| IndexOf                | Returns the index of a KeyValueExpression in the Properties by key or -1 if it doesn't exist |
| Add                    | Adds a KeyValueExpression to the Properties collection |

## KeyValueExpression ##
The KeyValueExpression represents the key-value pairs contained in an ObjectExpression.
| **Property or Method** | **Description** |
|:-----------------------|:----------------|
| KeyExpression          | Contains the expression representing the key. For Dictionary objects this does not have to be a string |
| Key                    | Returns the KeyExpression as a string.  This will fail if the KeyExpression is not a ValueExpression |
| ValueExpression        | The expression representing the value |

## ArrayExpression ##
The ArrayExpression class represents a JSON array.  The Items collection contains the items within the array.
| **Property or Method** | **Description** |
|:-----------------------|:----------------|
| Items                  | A collection of Expression objects representing the items in the array |
| Add                    | Adds an item to the Items collection |

## ValueExpression ##
Represents a string, number, boolean JSON value.  Number and Boolean types have specific sub-classes of ValueExpression: NumberExpression and BooleanExpression respectively.
| **Property or Method** | **Description** |
|:-----------------------|:----------------|
| Value                  | The value of the expression |
| StringValue            | Returns the value as a string |

## NumericExpression ##
A subclass of ValueExpression for representing numeric types including int, long, double and float.

## BooleanExpression ##
A subclass of ValueExpression for representing boolean values.

## CastExpression ##
The CastExpression does not correspond to a JSON construct but represents the added type information that JsonExSerializer adds when extra type information is needed.  It wraps other expressions which is contained in its Expression property.

## ReferenceExpression ##
The ReferenceExpression does not correspond to a JSON construct but represents a reference to another part of the JSON expression.  References appear in the serialized text as "$.parent.child" using JSONPath syntax.