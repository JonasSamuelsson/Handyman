# Handyman.AspNetCore

[changelog](./changelog.md)

This package provides api versioning & e-tag support for asp.net core.

## Api versioning

Validation is performed per endpoint.  
It supports multiple validation strategies, `major-minor-prerelease` and `exact match` comes out of the box with `major-minor-prerelease` as the default but with the ability to provide a custom implementation if required.  
By default the request version is read from a `api-version` query string parameter but this can also be customized.

## Configuration

Configure the required types in the service provider.

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddApiVersioning();
}
```

Add `ApiVersionAttribute` to your endpoints to apply validation.

``` csharp
[ApiController, Route("api/values")]
public class ValuesController : ControllerBase
{
    [HttpGet, ApiVersion("1.0")]
    public IEnumerable<string> GetValues()
    {
        ...
    }
}
```

The package integrates with asp.net core routing so multiple endpoints with the sample route template as long as the have different api versions.

``` csharp
[ApiController, Route("api/values")]
public class ValuesController : ControllerBase
{
    [HttpGet, ApiVersion("1.0")]
    public IEnumerable<string> GetValues()
    {
        ...
    }

    [HttpGet, ApiVersion("2.0")]
    public IEnumerable<Value> GetValues()
    {
        ...
    }
}
```

## ETags

todo
