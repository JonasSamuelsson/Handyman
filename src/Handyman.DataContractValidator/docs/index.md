# Handyman.DataContractValidator

[changelog](changelog.md)

Handyman.DataContractValidator is a library for data contract validation.

The idea behind this library is to validate the structure of the types.

Use `new DataContractValidator.Validate(type, dataContract)` to validate.  
The `type` parameter takes a `System.Type` and the dataContract parameter takes a `System.Object`.  
See below for how to validate different types.

## Data contract validation

Data contract validator can handle the following types and also handles nullable reference type annotations. 

* Value types (bool, int, string, etc)
* Objects
* Collections
* Dictionaries
* Enums (regular & flags)
* Anything
* Recursion

### Value types

``` csharp
public void Validate()
{
    var type = typeof(int);

    var dataContract = typeof(int);

    new DataContractValidator().Validate(type, dataContract);
}
```

### Objects

Properties decorated with an attribute with `Ignore` in the name will be ignored.

``` csharp
class MyParentClass
{
    public MyChildClass Child { get; set; }
    public bool Flag { get; set; }
    public int Number { get; set; }
}

class MyChildClass
{
    public string Text { get; set; }
}

[Fact]
public void Validate()
{
    var type = typeof(MyParentClass);

    var dataContract = new
    {
        Child = new
        {
            Text = typeof(string)
        },
        Flag = typeof(bool),
        Number = typeof(int)
    };

    new DataContractValidator().Validate(type, dataContract);
}
```

### Collections

If validated against a type it must implement `IEnumerable<T>`, `typeof(T)` will be used as item data contract.  
If validated against an instance the instance type must implement `IEnumerable<T>`, the single item in the collection will be used as item data contract.

``` csharp
class MyClass
{
    public IEnumerable<int> SimpleCollection1 { get; set; }
    public IEnumerable<int> SimpleCollection2 { get; set; }
    public IEnumerable<Child> ComplexCollection { get; set; }
}

class Child
{
    public string Text { get; set; }
}

[Fact]
public void Validate()
{
    var type = typeof(MyClass);

    var dataContract = new
    {
        // there are multiple ways to declare collections of simple types
        SimpleCollection1 = typeof(IEnumerable<int>),
        SimpleCollection2 = new[] { typeof(int) },
        ComplexCollection = new[]
        {
            new
            {
                Text = typeof(string)
            }
        }
    };

    new DataContractValidator().Validate(type, dataContract);
}
```

### Dictionaries

If validated against a type it must implement `IDictionary<TKey, TValue>`.  
If validated against an instance use the helper type `Dictionary<TKey>`.

``` csharp
class MyClass
{
    public Dictionary<int, string> DictionaryWithSimpleValue1 { get; set; }
    public Dictionary<int, string> DictionaryWithSimpleValue2 { get; set; }
    public Dictionary<int, ComplexClass> DictionaryWithComplexValue { get; set; }
}

class ComplexClass
{
    public string Text { get; set; }
}

[Fact]
public void Validate()
{
    var type = typeof(MyClass);

    var dataContract = new
    {
        // there are multiple ways to declare dictionaries with values of simple types
        DictionaryWithSimpleValue1 = typeof(Dictionary<int, string>),
        DictionaryWithSimpleValue2 = new Dictionary<int>
        {
            typeof(string)
        },
        DictionaryWithComplexValue = new Dictionary<int>
        {
            new
            {
                Text = typeof(string)
            }
        }
    };

    new DataContractValidator().Validate(type, dataContract);
}
```

### Nullable reference types

``` csharp
class MyClass
{
    public string? Text { get; set; }
}

[Fact]
public void Validate()
{
    var type = typeof(MyClass);

    var dataContract = new
    {
        Text = new CanBeNull(typeof(string))
    };

    new DataContractValidator().Validate(type, dataContract);
}
```

### Enums

``` csharp
[Flags]
enum Options
{
    None = 0,
    First = 1,
    Second = 2,
    Both = First | Second
}

[Fact]
public void Validate()
{
    var type = typeof(Options?);

    var dataContract = new Enum
    {
        Flags = true,
        Nullable = true,
        Values =
        {
            { 0, "None" },
            { 1, "First" },
            { 2, "Second" },
            { 3, "Both" }
        }
    };

    new DataContractValidator().Validate(type, dataContract);
}
}
```

### Anything

``` csharp
class MyParent
{
    public string Name { get; set; }
    public MyChild Child { get; set; }
}

class MyChild
{
    public string Text { get; set; }
}

[Fact]
public void Validate()
{
    var type = typeof(MyParent);

    var dataContract = typeof(Any);

    new DataContractValidator().Validate(type, dataContract);
}
```

### Recursion

``` csharp
class Directory
{
    public string Name { get; set; }
    public IEnumerable<Directory> Directories { get; set; }
    public IEnumerable<File> Files { get; set; }
}

class File
{
    public string Name { get; set; }
}

[Fact]
public void Validate()
{
    var type = typeof(Directory);

    var dataContractStore = new DataContractStore();

    var dataContract = new
    {
        Name = typeof(string),
        Directories = new[] { dataContractStore.Get("dir") },
        Files = new[]
        {
            new
            {
                Name = typeof(string)
            }
        }
    };

    dataContractStore.Add("dir", dataContract);

    new DataContractValidator().Validate(type, dataContract);
}
```

## Data contract generation

It is possible to generate data contract code from existing types.  
Code generation does not support recursive types.

``` csharp
string code = new DataContractGenerator().GenerateFor<MyClass>();
```