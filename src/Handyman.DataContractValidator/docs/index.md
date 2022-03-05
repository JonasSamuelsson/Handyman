# Handyman.DataContractValidator

[changelog](changelog.md)

Handyman.DataContractValidator is a library for data contract validation.

The idea behind this library is to validate the structure of the types.

Use `new DataContractValidator.Validate(type, dataContract)` to validate.  
The `type` parameter takes a `System.Type` and the dataContract parameter takes a `System.Object`.  
See below for how to validate different types.

## Data contract validation

Data contract validator can handle the following types and also handles nullable reference type annotations. 

* Objects
* Value types (bool, int, string, etc)
* Collections
* Dictionaries
* Enums (regular & flags)
* Anything

### Objects

Properties decorated with an attribute with `Ignore` in the name will be ignored.

``` csharp
// type definition
class MyClass
{
    public bool Flag { get; set; }
    public int Number { get; set; }
    public string Text { get; set; }
}

// validation
new DataContractValidator().Validate(typeof(MyClass), new
{
    Flag = typeof(bool),
    Number = typeof(int),
    Text = typeof(string)
})
```

#### Nullable reference types

``` csharp
// type definition
class MyClass
{
    public string? Text {get; set; }
}

// validation
new DataContractValidator().Validate(typeof(MyClass), new
{
    Text = new CanBeNull(typeof(string))
})
```

### Value types

``` csharp
// type definition
class Item
{
    public int Value { get; set; }
}

// validation
new DataContractValidator().Validate(typeof(Item), typeof(int));
```

### Collections

If validated against a type it must implement `IEnumerable<T>`, `typeof(T)` will be used as item data contract.  
If validated against an instance the instance type must implement `IEnumerable<T>`, the single item in the collection will be used as item data contract.

``` csharp
// type declaration
class MyClass
{
    public IEnumerable<int> Numbers { get; set; }
}

// valiation
new DataContractValidator().Validate(typeof(MyClass),
    new
    {
        Numbers = typeof(Ienumerable<int>)
    });

// or
new DataContractValidator().Validate(typeof(MyClass),
    new
    {
        Numbers = new [] { typeof(int) }
    });
```

### Dictionaries

If validated against a type it must implement `IDictionary<TKey, TValue>`.  
If validated against an instance use the helper type `Dictionary<TKey>`.

``` csharp
// dictionary with only value types
class MyClass
{
    public Dictionary<int, string> Lookup { get; set; }
}

new DataContractValidator().Validate(typeof(MyClass),
    new
    {
        Lookup = typeof(Dictionary<int, string>)
    })

// dictionary with complex value
class MyClass
{
    public Dictionary<int, Item> Lookup { get; set; }
}

class Item
{
    public string Text { get; set; }
}

new DataContractValidator().Validate(typeof(MyClass),
    new
    {
        Lookup = new Dictionary<int>(new
        {
            Text = typeof(string)
        })
    })
```

### Enums

``` csharp
// type definition
enum Number
{
    Zero = 0,
    One = 1,
    Two = 2
}

// validation
new DataContractValidator().Validate(
    typeof(Number), 
    new Enum
    {
        Flags = false, // default is false, can be omitted in this case
        Nullable = true, // default is false, can be omitted in this case
        Values = 
        {
            { 0, "Zero" },
            { 1, "One" },
            { 2, "Two" }
        }
    })
```

### Anything

``` csharp
// type definition
class Item
{
    public int Value { get; set; }
}

// validation
new DataContractValidator().Validate(typeof(Item), new { Value = typeof(Any) })
```

### Data contract validation sample

This sample demonstrates how to do inline validation of the following object graph.

``` csharp
public class Root
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<Item> Items { get; set; }
    public IDictionary<Guid, Resource> Resources { get; set; }
}

public class Item
{
    public string Text { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}

public class Resource
{
    public bool Enabled { get; set; }
    public string Info { get; set; }
    public ResourceType Type { get; set; }
}

public enum ResourceType
{
    Undefined = 0,
    Foo = 1,
    Bar = 2
}

public class Test
{
    [Fact]
    public void TypesShouldMatchDataContract()
    {
        var type = typeof(Root);

        var dataContract = new
        {
            Id = typeof(int),
            Name = typeof(string),
            Items = new []
            {
                new
                {
                    Text = typeof(string),
                    Timestamp = typeof(DateTimeOffset)
                }
            },
            Resources = new Dictionary<Guid>(new
            {
                Enabled = typeof(bool),
                Info = typeof(string),
                Type = new Enum
                {
                    Values = 
                    {
                        { 0, "Undefined" },
                        { 1, "Foo" },
                        { 2, "Bar" }
                    }
                }
            })
        };

        new DataContractValidator().Validate(type, dataContract);
    }
}
```

## Data contract generation

It is possible to generate data contract code from existing types.

``` csharp
string code = new DataContractGenerator().GenerateFor<MyClass>();
```