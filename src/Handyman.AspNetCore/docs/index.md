# Handyman.AspNetCore

This package provides api versioning & e-tag support for asp.net core 3.x applications.

[changelog](./changelog.md)

## Api versioning

Api versioning is enabled by adding the `ApiVersionAttribute` at the action method level.   It integrates with the asp.net core routing engine so it's possible to declare multiple endpoint with the same http method & route template as long as they have different api versions. The requested api version is read from a `api-version` query string parameter but can be customized. Multiple versioning schemes are supported, `Major.Minor-PreRelease` and `Literal` are provided out of the box with `Major.Minor-PreRelease` as the default but with the ability to provide a custom implementation if required.  

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

### Validate http request e-tag header format

Http request headers `If-Match` or `If-None-Match` will automatically be inspected and if it isn't a valid e-tag (wildcards are supported), it will respond with `400 Bad request`.

### Access request e-tag

Action method `string` parameters named `ifMatch`, `ifMatchETag`, `ifNoneMatch`, `ifNoneMatchETag` or decorated with `FromIfMatchHeaderAttribute` or `FromIfNoneMatchHeaderAttribute` will be populated with the value from the corresponding header.  
If the request doesn't contain a matching e-tag header the service will respond with a `428 Precondition required`.

``` csharp
[HttpPut]
public void Store(Payload payload, string ifMatchETag)
{
    ...
}

[HttpPut]
public void Store(Payload payload, [FromIfMatchHeader] string eTag)
{
    ...
}
```

### Compare e-tags

Use `IETagUtilities.Comparer` to see if the incoming e-tag is up to date.  
The `EnsureEquals` methods will throw a `PreconditionFailedException` if the values don't match. The exception will be caught by the middleware and converted to a `412 Precondition failed` response.

``` csharp
public class Repository
{
    private readonly DbContext _dbContext;
    private readonly IETagUtilities _eTagUtilities;

    public Repository(DbContext dbContext, IETagUtilities eTagUtilities)
    {
        _dbContext = dbContext;
        _eTagUtilities = eTagUtilities;
    }

    public async Task UpdateItem(Item item, string eTag, CancellationToken cancellationToken)
    {
        var dbItem = await _dbContext.Items.SingleAsync(x => x.Id == item.Id, cancellationToken);

        _eTagUtilities.Comparer.EnsureEquals(eTag, item.RowVersion);

        // update dbItem from item

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
```

### Generate e-tags

Use `IETagUtilities.Converter` to convert byte arrays (sql server _row versions_) to string.

``` csharp
var eTagUtilities = serviceProvider.GetRequiredService<IETagUtilities>();
byte[] bytes = ...;
string eTag = eTagUtilities.Converter.FromByteArray(bytes);
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
