# Handyman.DependencyInjection

This package provides `IServiceCollection` extensions that simplifies service configuration.

[changelog](./changelog.md)

## AddDiagnostics

`AddDiagnostics()` configures an instance of `IDiagnostics` whitch has two methods for getting information about configured services (`GetServiceDescriptionsString()` & `GetServiceDescriptionsString()`).

## `Scan`

`Scan(...)` enables convention based service configuration.  
The scanner provides methods for defining which types to scan, how to filter them and the registration convention to use.

These conventions are provided out of the box `ConfigureClassesWithServiceAttribute`, `ConfigureConcreteClassesOf`, `ConfigureDefaultImplementations` & `UsingServiceConfigurationPolicies`.

### Classes with service attribute

This convention configures classes decorated with the `ServiceAttribute`.  

With the `ServiceAttribute` services doesn't need to be configured from the same place in the code but in stead types can be decorated with the configuration to use.

```
public interface IFoo { }

[Service(typeof(IFoo))]
public class Foo : IFoo { }

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
```

### Default implementations

Default implemenations are classes implementing interfaces in the same namespace with the same name except the leading I.

``` csharp
namespace Company.Product.Feature
{
    public interface IFoo { }
    public class Foo { }
}
```

### Service configuration policies

With this convention types can be decorated with the `ServiceConfigurationPolicyAttribute` specifying an implementation of `IServiceConfigurationPolicy` that will be invoked.

### Custom service lifetime

Both `ConfigureDefaultImplementations` and `ConfigureConcreteClassesOf` configures services as transients by default but provides an overload where the service lifetime can be specified.  
Classes can also be decorated with the `ServiceLifetimeAttribute` where service lifetime can be specified.
