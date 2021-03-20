# Handyman.DependencyInjection

This package provides `IServiceCollection` extensions that simplifies service configuration.

[changelog](./changelog.md)

## AddServiceProviderInsights

`AddServiceProviderInsights()` configures an instance of `IServiceProviderInsights` which has `IEnumerable<ServiceDescription> GetServiceDescriptions()` & `string ListServiceDescriptions()` methods for retrieving information about configured services.

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddServiceProviderInsights()
}
```

## Scan

`Scan(...)` enables convention based service configuration.  
The scanner provides methods for defining which types to scan, how to filter them and the configuration convention to use.

To specify the types to process use one of

* `IScanner.Assembly(Assembly)`
* `IScanner.AssemblyContaining(Type)`
* `IScanner.AssemblyContaining<T>()`
* `IScanner.AssemblyContainingTypeOf(object)`
* `IScanner.EntryAssembly()`
* `IScanner.Types(IEnumerable<Type>)`

To filter which types to process use

* `IScanner.Where(Func<Type, bool>)`

or decorate types to ignore with `ScannerIgnoreAttribute`.

``` csharp
[ScannerIgnore]
public class Foo { }
```

Conventions provided out of the box are

* `ConfigureClassesWithServiceAttribute()`
* `ConfigureConcreteClassesOf(...)`
* `ConfigureDefaultImplementations(...)`
* `ExecuteServiceConfigurationStrategies()`.

To use a custom convention use one of

* `IScanner.Using(IServiceConfigurationConvention)`
* `IScanner.Using<TConvention>() where TConvention : IServiceConfigurationConvention, new()`

### Classes with service attribute

This convention configures classes decorated with the `ServiceAttribute`.  

With the `ServiceAttribute` services doesn't need to be configured from the same place in the code but in stead types can be decorated with the configuration to use.

``` csharp
public interface IFoo { }

[Service(typeof(IFoo))]
public class Foo : IFoo { }

public void ConfigureServices(IServiceCollection services)
{
    services.Scan(x => x.EntryAssembly().ConfigureClassesWithServiceAttribute());
}
```

`ServiceAttribute` also has support for specifying the service lifetime.

``` csharp
public interface IBar { }

[Service(typeof(IBar), ServiceLifetime.Singleton)]
public class Bar : IBar { }
```

### Concrete classes of

This convention configures concrete classes matching a specific interface or class.

``` csharp
public interface IFoo<T> { }

public class Foo<T> : IFoo<T> { }

public class Foo : IFoo<string> { }

public void ConfigureServices(IServiceCollection services)
{
    services.Scan(x => x.EntryAssembly().ConfigureConcreteClassesOf(typeof(IFoo<>)));
}
```

### Default implementations

Default implemenations are classes implementing interfaces in the same namespace with the same name except the leading I.

``` csharp
public interface IFoo { }

public class Foo : IFoo { }

public void ConfigureServices(IServiceCollection services)
{
    services.Scan(x => x.EntryAssembly().ConfigureDefaultImplementations());
}
```

### Custom service configuration strategies

With this convention classes implementing `IServiceConfigurationStrategy` will be instantiated (requires a parameterless contructor) and invoked.

``` csharp
public interface IFoo { }

public class Foo : IFoo { }

public class CustomServiceConfigurationStrategy : IServiceConfigurationStrategy
{
    public void Execute(IServiceCollection services)
    {
        services.AddTransient<IFoo, Foo>();
    }
}

public void ConfigureServices(IServiceCollection services)
{
    services.Scan(x => x.EntryAssembly().ExecuteServiceConfigurationStrategies());
}
```

### Custom service lifetime

Both `ConfigureDefaultImplementations` and `ConfigureConcreteClassesOf` configures services as transients by default but provides an overload where the service lifetime can be specified.

``` csharp
services.Scan(x => x.EntryAssembly().ConfigureDefaultImplmentations(ServiceLifetime.Scoped)
```

Classes can also be decorated with the `ServiceLifetimeAttribute` where service lifetime can be specified.

``` csharp
[ServiceLifetime(ServiceLifetime.Scoped)]
public class Foo : IFoo { }
```
