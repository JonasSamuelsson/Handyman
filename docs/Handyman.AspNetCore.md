# Handyman.AspNetCore

This package provides an api version validation implementation for asp.net core.

Validation is performed per endpoint.  
It supports multiple validation strategies, `semver` and `exact match` comes out of the box with semver as the default but with the ability to provide a custom implementation if required.  
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
[ApiController]
[Route("api/values")]
public class ValuesController : ControllerBase
{
    [ApiVersion("1.0")]
    [HttpGet]
    public IEnumerable<string> GetValues()
    {
        ...
    }
}
```