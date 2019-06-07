# Handyman.DataContractValidator

Handyman.DataContractValidtor is a library for data contract validation.

The idea behind this library is to validate the structure of the types.

## Supported types

Data contract validator can handle the following types;

* Enums (regular & flags)
* Value types (bool, int, string, etc)
* Objects
* Collections
* Dictionaries

### Enums

``` csharp
typeof(MyEnum);
new Enum(0, 1, 2)
new Enum(EnumKind.Flags, new [] { 0, 1, 2 })
new Enum(EnumKind.Nullable, new [] { 0, 1, 2 })
new Enum(EnumKind.Flags | EnumKind.Nullable, new [] { 0, 1, 2 })
```

### Values

``` csharp
typeof(int)
new Value<int>()
```

### Objects

Properties of the actual type decorated with `JsonIgnoreAttribute` will be ignored.

``` csharp
typeof(MyObject)
new
{
    Flag = typeof(bool),
    Number = typeof(int),
    Text = typeof(string)
}
```

### Collections

If validated against a type it must implement `IEnumerable<T>`.  
If validated against an instance the instance type must implement `IEnumerable<T>`.  
If the collection is emtpy the type of `T` will be used, if it contains any elements the first element will be used.

``` csharp
typeof(int[])
typeof(IEnumerable<int>)
typeof(List<int>)
new Collection<int>()
new List<int>()
new int[] { }
new [] { typeof(int) }
new [] { new { Text = typeof(string) } }
```

### Dictionaries

If validated against a type it must implement `IDictionary<TKey, TValue>`.  
If validated against an instance it must implement `IDictionary<TKey, TValue>` or `Hashtable`.  
If the instance implements `IDictionary<TKey, TValue>` and is empty `TKey` & `TValue` will be used.  
If the instance implements `IDictionary<TKey, TValue>` or is a subtype of `Hashtable` and contains any elements the first element will be used.  
To reduce the amount of typing required when specifying complex objects there is a dedicated `Dictionary<TKey>` class that can be used that takes an `object valueDataContract` as contructor parameter.

``` csharp
typeof(IDictionary<int, string>)
typeof(Dictionary<int, string>)
new Dictionary<int, string>()
new Dictionary<object, object> { { typeof(int), typeof(string) } }
new Hashtable { { typeof(int), typeof(int) } }
new Dictionary<int>(typeof(string))
```

## Sample

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
                Type = new Enum(0, 1, 2)
            })
        };

        new DataContractValidator().Validate(type, dataContract);
    }
}
```

### Recursive types

The `DataContractValidator` class has support for registering data contracts by name.  
These contracts can then be used to validate against, for instance in the case of recursive types or if the same structure is repeated in multiple places of the object graph.

``` csharp
public class Node
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<Node> Nodes { get; set; }
}

public class Test
{
    [Fact]
    public void TypesShouldMatchDataContract()
    {
        var validator = new DataContractValidator();

        validator.AddDataContract("node", new 
        {
            Id = typeof(int),
            Name = typeof(string),
            Nodes = new [] { validator.GetDataContract("node") }
        })

        validator.Validate(typeof(Node), validator.GetDataContract("node"))
    }
}
```