# Handyman.AspNetCore

This package provides api versioning & e-tag support for asp.net core 3.x applications.

[changelog](./changelog.md)

## Api versioning

Api versioning is enabled by adding the `ApiVersionAttribute` at the controller or action level.   It integrates with the asp.net core routing engine so it's possible to declare multiple endpoint with the same http method & route template as long as they have different api versions. The requested api version is read from a `api-version` query string parameter but can be customized. Multiple versioning schemes are supported, `Major.Minor-PreRelease` and `Literal` are provided out of the box with `Major.Minor-PreRelease` as the default but with the ability to provide a custom implementation if required.  

### Usage

Configure the required types in the service provider.

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddApiVersioning();
}
```

Decorate endpoints with the `ApiVersionAttribute` to enable validation.

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
    public IEnumerable<int> GetValues()
    {
        ...
    }
}
```

### Version schemes

#### Major.Minor-PreRelease scheme

This is the default versioning scheme. It requires all declared and requested versions to be in one of the following formats `<major>`, `<major>.<minor>`, `<major>-<prerelease>`, `<major>.<minor>-<prerelease>`.  
`<major>` and `<minor>` must be numerical while `<prerelease>` can contain any character.  
`<major>` is interperted as `<major>.0` and `<major>-<prerelease>` as `<major>.0-<prerelease>`.

Requests with an api version that doesn't conform to the version format will result in a `400 Bad Request` response.

#### Literal scheme

This scheme accepts versions in any format.

Use the following approach to enable this scheme.

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddApiVersioning(options => options.UseLiteralScheme());
}
```

#### Custom scheme

Register a custom `IApiVersionParser` implementation to use a custom version scheme.

``` csharp
public class IntApiVersionParser : IApiVersionParser
{
    public bool TryParse(string candidate, out IApiVersion version)
    {
        version = null;
        
        if (!int.TryParse(candidate, out var i))
            return false;

        version = new IntApiVersion { Version = i };
        return true;
    }
}

public class IntApiVersion : IApiVersion
{
    public string Text => Version.ToString();

    public int Version { get; set; }

    public bool IsMatch(IApiVersion other)
    {
        return other is IntApiVersion o && Version == o.Version;
    }
}
```

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IApiVersionParser, IntApiVersionParser>();
    services.AddApiVersioning();
}
```

### Customize where to read requested version

Register a custom `IApiVersionReader` implementation in the service collection to change where the requested version is read from.

``` csharp
public class HeaderApiVersionReader : IApiVersionReader
{
    private const string HeaderName = "X-Api-Version";

    public bool TryRead(HttpRequest httpRequest, out StringValues values)
    {
        return httpRequest.Headers.TryGetValues(HeaderName, out values);
    }
}
```

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IApiVersionReader, HeaderApiVersionReader>();
    services.AddApiVersioning();
}
```

### Multiple versions per endpoint

It is possible to have a single endpoint support multiple versions. In that case use the overloaded `ApiVersionAttribute` constructor that takes multple versions.

``` csharp
[HttpGet, ApiVersion(new[] { "1.0", "1.1" })]
public IEnumerable<string> GetValues()
{
    ...
}
```

### Model binding

Add a `string` parameter named `apiVersion` to the action method to access the requested version.

``` csharp
[HttpGet, ApiVersion(new[] { "1.0", "1.1" })]
public IEnumerable<string> GetValues(string apiVersion)
{
    ...
}
```

### Optional versioning

If versioning is added to an existing api it might not be possible to require a version on every request without breaking existing consumers. For those cases it is possible to configure the versioning to be optional.

``` csharp
[HttpGet, ApiVersion("1.0", Optional = true)]
public IEnumerable<string> GetValues()
{
    ...
}
```

Endpoints with optional versioning can specify a default version to use.

``` csharp
[HttpGet, ApiVersion(new[] { "1.0", "1.1" }, Optional = true, DefaultVersion = "1.1")]
public IEnumerable<string> GetValues(string apiVersion)
{
    ...
}
```

## ETags

This feature simplifies accessing the `e-tag` from `If-Match` and `If-None-Match` headers.  
It will also make sure that the e-tag conforms to the e-tag format.

### Access request e-tag

Add the required services.

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddETags();
}
```

Add a `string` parameter named `ifMatch` or `ifNoneMatch`.

``` csharp
[HttpPut]
public void Store(Payload payload, string ifMatch)
{
    ...
}
```

### Write response e-tag header

Use the `SetETagHeader` extension method on `HttpResponse` to set the e-tag header.

``` csharp
[HttpGet]
public void Get()
{
    var eTag = GetETag();
    Response.SetETagHeader(eTag);
}
```
