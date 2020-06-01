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

### Versioning schemes

#### Major.Minor-PreRelease (default)

This scheme requires all declared and requested versions to be in one of the following formats `<major>`, `<major>.<minor>`, `<major>-<prerelease>`, `<major>.<minor>-<prerelease>`.  
`<major>` and `<minor>` must be numerical while `<prerelease>` can contain characters `.0-9a-z`.

Requests with an api version that doesn't conform to the version format will result in a `400 Bad Request` response.

#### Custom scheme

Register a custom `IApiVersionParser` implementation to use a custom version scheme.

``` csharp
public class LiteralApiVersionParser : IApiVersionParser
{
    public bool TryParse(string candidate, out IApiVersion version)
    {
        if (string.IsNullOrWhiteSpace(candidate))
        {
            version = null;
            return false;
        }

        version = new LiteralApiVersion { Text = candidate };
        return true;
    }
}

public class LiteralApiVersion : IApiVersion
{
    public string Text { get; set; }

    public bool IsMatch(IApiVersion other)
    {
        return other is LiteralApiVersion o && Text == o.Text;
    }
}
```

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IApiVersionParser, LiteralApiVersionParser>();
    services.AddApiVersioning();
}
```

### Customize where to read requested version

Register a custom `IApiVersionReader` implementation in the service collection to change how to read the requested version.

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

It is possible to have a single endpoint support multiple versions by passing an array of strings to the `ApiVersionAttribute` constructor.

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

Versioning can be configured as optional per endpoint.

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

### Setup

Add required services and middleware.

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddETags();
}

public void Configure(IApplicationBuilder app)
{
    app.UseRouting();
    app.UseETags();
    app.UseEndpoints(endpoints => endpoints.MapControllers());
}
```

:information_source: The e-tags middleware must be added after any exception handling middleware (like [Hellang.Middleware.ProblemDetails](https://www.nuget.org/packages/Hellang.Middleware.ProblemDetails/)) to work.

### Access request e-tag

Add a `string` parameter named `ifMatch`, `ifMatchETag`, `ifNoneMatch` or `ifNoneMatchETag` to read from the corresponding header.

``` csharp
[HttpPut]
public void Store(Payload payload, string ifMatch)
{
    ...
}
```

### Compare e-tags

Use `IETagComparer` to see if the incoming e-tag is up to date.

``` csharp
public class Repository
{
    private readonly DbContext _dbContext;
    private readonly IETagComparer _eTagComparer;

    public Repository(DbContext dbContext, IETagComparer eTagComparer)
    {
        _dbContext = dbCOntext;
        _eTagComparer = eTagComparer;
    }

    public async Task SaveItem(int id, Item item, string ifMatchETag)
    {
        var dbItem = await _dbContext.Items.SingleAsync(x => x.Id == id);

        _eTagComparer.EnsureEquals(ifMatchETag, item.RowVersion);

        // update dbItem from item

        await _dbContext.SaveChangesAsync();
    }
}
```

### Generate e-tag string from byte array (sql server row version).

Use `IETagConverter` to convert byte arrays to string.

``` csharp
var converter = serviceProvider.GetRequiredService<IETagConverter>();
byte[] bytes = ...;
string eTag = converter.FromByteArray(bytes);
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
